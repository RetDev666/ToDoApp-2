using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ToDoApp.Application;
using ToDoApp.Infrastructure.Persistence;
using ToDoApp.Infrastructure.ExternalServices;
using ToDoApp.WebApi.Extensions;
using ToDoApp.WebApi.Middleware;

namespace ToDoApp.WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationLayer();

            services.AddPersistenceInfrastructure(Configuration);

            services.AddExternalServicesInfrastructure(Configuration);

            services.AddControllers();

            services.AddApiVersioningExtension();

            services.AddSwaggerExtension();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseErrorHandlingMiddleware();

            app.UseRouting();

            app.UseAuthorization();

            // Âêëþ÷àºìî Swagger UI
            app.UseSwaggerExtension();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}