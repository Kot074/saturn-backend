using Microsoft.EntityFrameworkCore;
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

        private readonly DbContext _dbContext;
        private readonly string _ownerEmail;
        private readonly string _ownerPassword;

        public InitiallizeHostedService(IConfiguration configuration)
        {
#if DOCKER
            var _ownerEmail = configuration.GetValue<string>(CommonConst.OwnerEmailVariable)
                              ?? throw new NullReferenceException($"Переменная окружения {CommonConst.OwnerEmailVariable} не существует!");
            var _ownerPassword = configuration.GetValue<string>(CommonConst.OwnerPasswordVariable)
                                 ?? throw new NullReferenceException($"Переменная окружения {CommonConst.OwnerPasswordVariable} не существует!");
#else
            _ownerEmail = configuration.GetValue<string>(CommonConst.ConfigurationOwnerEmailField);
            _ownerPassword = configuration.GetValue<string>(CommonConst.ConfigurationOwnerPasswordField);
#endif

            _dbContext = new UsersDbContext(configuration);
            _usersRepository = new UsersRepository(configuration);
            _usersHelpers = new UsersHelpersService(_usersRepository);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                _dbContext.Database.Migrate();

                var users = _usersRepository.ReadAll().Result;
                if (!string.IsNullOrWhiteSpace(_ownerEmail) && !string.IsNullOrWhiteSpace(_ownerPassword) && !users.Any())
                {
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
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
