using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using ToDoApp.Application;
using ToDoApp.Infrastructure.Persistence;
using ToDoApp.WebApi.Extensions;
using ToDoApp.Infrastructure.ExternalServices;

namespace ToDoApp.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationLayer();
            services.AddPersistenceInfrastructure(Configuration);
            services.AddExternalServicesInfrastructure(Configuration); // Додайте цей рядок
            services.AddControllers();

            // Якщо вам потрібно прибрати Swagger, закоментуйте або видаліть наступний рядок
            // services.AddSwaggerExtension();

            services.AddApiVersioningExtension();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthorization();

            // Закоментуйте або видаліть наступний рядок, якщо Swagger не потрібен
            // app.UseSwaggerExtension();

            app.UseErrorHandlingMiddleware();
            app.UseSerilogRequestLogging();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
