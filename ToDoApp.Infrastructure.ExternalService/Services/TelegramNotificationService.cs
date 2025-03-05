using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using ToDoApp.Core.Application.Intefaces.Services;

namespace ToDoApp.Infrastructure.ExternalServices.Services
{
    public class TelegramNotificationService : ITelegramNotificationService
    {
        private readonly TelegramBotClient _botClient;
        private readonly string _chatId;

        public TelegramNotificationService(IConfiguration configuration)
        {
            var botToken = configuration["TelegramBot:Token"];
            _chatId = configuration["TelegramBot:ChatId"];

            // Додайте логування для відстеження отриманого токену
            Console.WriteLine($"Token from config: '{botToken}'");

            // Перевірте та видаліть будь-які пробіли або невидимі символи
            botToken = botToken?.Trim();

            if (string.IsNullOrEmpty(botToken) || string.IsNullOrEmpty(_chatId))
                throw new ArgumentException("Відсутня конфігурація для бота Telegram");

            try
            {
                // Переконайтесь, що токен не містить лапок або інших непотрібних символів
                botToken = botToken.Replace("\"", "").Trim();

                // Виведіть токен для перевірки (тільки для налагодження)
                Console.WriteLine($"Using token: '{botToken}'");

                // Спробуйте створити клієнта, вказавши токен безпосередньо
                _botClient = new TelegramBotClient("7304978265:AAElTsUzlb8XRX4TCnJZpEa8IfXl6oa4vjc");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task SendErrorNotificationAsync(string errorMessage, int statusCode)
        {
            var message = $"⚠️ Сповіщення про помилку\n\n" +
                          $"Код статусу: {statusCode}\n" +
                          $"Повідомлення: {errorMessage}";

            await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: message
            );
        }
    }
}