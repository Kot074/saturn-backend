using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NLog.Web;
using Saturn.CommonLibrary.Models;
using Saturn.UsersService.Common;
using Saturn.UsersService.Database;
using Saturn.UsersService.Database.Models;
using Saturn.UsersService.Repositories;

namespace Saturn.UsersService.Services
{
    public class InitiallizeHostedService : IHostedService
    {
        private readonly IUsersHelpersService _usersHelpers;
        private readonly IUsersRepository _usersRepository;
        private readonly NLog.ILogger _logger;

        private readonly DbContext _dbContext;
        private readonly string _ownerEmail;
        private readonly string _ownerPassword;

        public InitiallizeHostedService(IConfiguration configuration)
        {
            _logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
#if DOCKER
            _logger.Info($"Чтение данных первого пользователя из переменных окружения...");
            _ownerEmail = Environment.GetEnvironmentVariable(CommonConst.OwnerEmailVariable);
            _ownerPassword = Environment.GetEnvironmentVariable(CommonConst.OwnerPasswordVariable);
#else
            _logger.Info($"Чтение данных первого пользователя из конфигурационного файла...");
            _ownerEmail = configuration.GetValue<string>(CommonConst.ConfigurationOwnerEmailField);
            _ownerPassword = configuration.GetValue<string>(CommonConst.ConfigurationOwnerPasswordField);
#endif

            _dbContext = new UsersDbContext(configuration);
            _usersRepository = new UsersRepository(configuration);
            _usersHelpers = new UsersHelpersService(_usersRepository);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _dbContext.Database.Migrate();

            var users = _usersRepository.ReadAll().Result;

            if (!string.IsNullOrWhiteSpace(_ownerEmail) && !string.IsNullOrWhiteSpace(_ownerPassword) && !users.Any())
            {
                _logger.Info("Создание первого пользователя...");
                var userDb = new UserDbModel
                {
                    Name = "EMPTY",
                    Lastname = "EMPTY",
                    Email = _ownerEmail,
                    Role = UserRoles.Administrator,
                    Key = _usersHelpers.EncodingString(_ownerPassword)
                };

                _usersRepository.Create(userDb);
            }
            else if (!users.Any())
            {
                _logger.Error("В базе данных нет ни одного пользователя.");
                throw new InvalidOperationException();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
