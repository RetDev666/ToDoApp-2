using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToDoApp.Core.Application.Wrappers;
using System.Net;
using System.Text.Json;
using ToDoApp.Core.Application.Intefaces.Services;

namespace ToDoApp.WebApi.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITelegramNotificationService _telegramService;

        public ErrorHandlerMiddleware(RequestDelegate next, ITelegramNotificationService telegramService)
        {
            _next = next;
            _telegramService = telegramService;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                var statusCode = (int)HttpStatusCode.InternalServerError;
                var responseModel = new Response<string>() { Succeeded = false, Message = error?.Message };

                switch (error)
                {
                    case Core.Application.Exceptions.ApiException e:
                        // Користувацька помилка додатку
                        statusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case Core.Application.Exceptions.ValidationException e:
                        // Помилка валідації
                        statusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.Errors = e.Errors;
                        break;
                    case KeyNotFoundException e:
                        // Помилка "не знайдено"
                        statusCode = (int)HttpStatusCode.NotFound;
                        break;
                    default:
                        // Необроблена помилка
                        statusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                response.StatusCode = statusCode;

                // Відправити сповіщення до Telegram
                await _telegramService.SendErrorNotificationAsync(error?.Message, statusCode);

                var result = JsonSerializer.Serialize(responseModel);
                await response.WriteAsync(result);
            }
        }
    }
}