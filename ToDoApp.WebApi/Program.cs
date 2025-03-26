using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;
using ToDoApp.Core.Application.Intefaces.Services;

namespace ToDoApp.WebApi
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
 
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .CreateLogger();

            try
            {
                Log.Information("Запуск веб-API програми Todo з інтеграцією Telegram");

       
                var host = CreateHostBuilder(args).Build();
                await host.RunAsync();

                return 0;
            }
            catch (Exception ex)
            {
                
                Log.Fatal(ex, "Програму було завершено через критичну помилку");

               
                try
                {
              
                    Log.Error("Критична помилка при запуску програми: {ErrorMessage}", ex.Message);
                }
                catch
                {
                    
                }

                return 1;
            }
            finally
            {
                
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog() 
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
            
                    logging.ClearProviders();

                    logging.AddSerilog();

                    logging.SetMinimumLevel(LogLevel.Information);
                });
    }
}