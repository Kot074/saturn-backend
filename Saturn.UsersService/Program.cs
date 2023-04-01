using NLog.Web;
using Saturn.UsersService.Extensions;
using Saturn.UsersService.Repositories;
using Saturn.UsersService.Services;

namespace Saturn.UsersService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            try
            {
                logger.Info("Конфигурирование сервиса...");
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
#if (DEBUG)
                    .AddJsonFile("appsettings.Development.json")
#else
                .AddJsonFile("appsettings.json")
#endif
                    .Build();

                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.
                builder.Services.AddHostedService<InitiallizeHostedService>();

                builder.Services.AddScoped<IUsersRepository, UsersRepository>();
                builder.Services.AddScoped<IUsersHelpersService, UsersHelpersService>();

                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddCors();

                builder.Services.AddApplicationAuthentication();
                builder.Services.AddApplicationSwagger();
                builder.Services.AddSingleton(typeof(NLog.Logger), logger);

                // Configure logging
                builder.Logging.ClearProviders();
                builder.Host.UseNLog();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseCors(opt =>
                {
                    opt.AllowAnyOrigin();
                    opt.AllowAnyMethod();
                    opt.AllowAnyHeader();
                });
                app.UseHttpsRedirection();
                app.UseAuthentication();
                app.UseAuthorization();
                app.MapControllers();

                logger.Info("Запуск сервиса...");
                app.Run();
            }
            catch (Exception ex)
            {
                logger.Error("При работе сервиса произошла ошибка. Работа будет завершена.", ex);
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }
    }
}