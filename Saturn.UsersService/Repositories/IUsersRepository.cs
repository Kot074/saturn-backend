using System.Collections.Generic;
using System.Threading.Tasks;
using Saturn.UsersService.Database.Models;

namespace Saturn.UsersService.Repositories
{
    public interface IUsersRepository
    {
        public Task Create(UserDbModel entity);
        public Task<UserDbModel> Read(long id);
        public Task<IEnumerable<UserDbModel>> ReadAll();
        public Task Update(UserDbModel entity);
        public Task Delete(long id);
    }
}
