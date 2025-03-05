using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks;

namespace ToDoApp.Core.Application.Intefaces.Services
{
    public interface ITelegramNotificationService
    {
        Task SendErrorNotificationAsync(string errorMessage, int statusCode);
    }
}