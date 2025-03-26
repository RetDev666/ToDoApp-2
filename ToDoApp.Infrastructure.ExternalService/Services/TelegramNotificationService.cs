using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<TelegramNotificationService> _logger;

        public TelegramNotificationService(IConfiguration configuration, ILogger<TelegramNotificationService> logger)
        {
            _logger = logger;

            // Отримуємо значення з конфігурації
            var botToken = configuration["TelegramBot:Token"];
            _chatId = configuration["TelegramBot:ChatId"];

            // Логуємо значення конфігурації (без розкриття повного токена в логах)
            _logger.LogInformation("Ініціалізація сервісу Telegram з ID чату: {ChatId}", _chatId);
            if (!string.IsNullOrEmpty(botToken))
            {
                _logger.LogInformation("Токен бота знайдено в конфігурації");
            }
            else
            {
                _logger.LogWarning("Токен бота відсутній у конфігурації");
            }

            // Перевіряємо конфігурацію
            if (string.IsNullOrEmpty(botToken) || string.IsNullOrEmpty(_chatId))
            {
                throw new ArgumentException("Відсутня конфігурація бота Telegram. Перевірте файл appsettings.json.");
            }

            // Очищуємо значення токена
            //botToken = botToken.Trim().Replace("\"", "");

            try
            {
                // Створюємо клієнт Telegram бота
                _botClient = new TelegramBotClient(botToken);

                // Перевіряємо, чи дійсний токен бота (необов'язково, але рекомендовано)
                // Це робить виклик до API Telegram для перевірки роботи токена
                var me = _botClient.GetMeAsync().GetAwaiter().GetResult();
                _logger.LogInformation("Успішно підключено до Telegram як бот: {BotName}", me.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не вдалося ініціалізувати клієнт бота Telegram");
                throw;
            }
        }

        public async Task SendErrorNotificationAsync(string errorMessage, int statusCode)
        {
            try
            {
                // Форматуємо повідомлення з міткою часу, кодом статусу та деталями помилки
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var message = $"⚠️ Сповіщення про помилку ({timestamp})\n\n" +
                              $"Код статусу: {statusCode}\n" +
                              $"Повідомлення: {errorMessage}";

                // Надсилаємо повідомлення до Telegram
                await _botClient.SendTextMessageAsync(
                    chatId: _chatId,
                    text: message
                );

                _logger.LogInformation("Надіслано сповіщення про помилку в Telegram");
            }
            catch (Exception ex)
            {
                // Логуємо помилку, але не кидаємо виняток, щоб не перервати обробку основної помилки
                _logger.LogError(ex, "Не вдалося надіслати сповіщення про помилку в Telegram");
            }
        }
    }
}