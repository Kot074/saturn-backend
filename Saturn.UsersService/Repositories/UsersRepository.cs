using Microsoft.EntityFrameworkCore;
using Saturn.UsersService.Database;
using Saturn.UsersService.Database.Models;

namespace Saturn.UsersService.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly UsersDbContext _dbContext;

        public UsersRepository(IConfiguration configuration)
        {
            _dbContext = new UsersDbContext(configuration);
        }

        public Task Create(UserDbModel entity)
        {
            return Task.Run(() =>
            {
                _dbContext.Add(entity);
                _dbContext.SaveChanges();
            });
        }

        public Task<UserDbModel> Read(long id)
        {
            return Task.Run(() =>
            {
                try
                {
                    return _dbContext.Users
                        .AsNoTracking()
                        .Single(x => x.Id == id);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Не удалось найти пользователя с Id = {id}", ex);
                }
            });
        }

        public Task<IEnumerable<UserDbModel>> ReadAll()
        {
            return Task.Run(() =>
            {
                return _dbContext.Users
                    .AsNoTracking()
                    .AsEnumerable();
            });
        }

        public Task Update(UserDbModel entity)
        {
            return Task.Run(() =>
            {
                _dbContext.Users.Update(entity);
                _dbContext.SaveChanges();
            });
        }

        public Task Delete(long id)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var user = await Read(id);
                    _dbContext.Entry(user).State = EntityState.Deleted;
                    _dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    new Exception($"Не удалось удалить пользователя с Id = {id}.", ex);
                }
            });
        }
    }
}
