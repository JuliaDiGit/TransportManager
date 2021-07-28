using Domain;
using Entities;

namespace Data.Repositories.Abstract
{
    public interface IUsersRepository : IBaseRepository<UserEntity>
    {
        UserEntity GetUserByLogin(string login);
        UserEntity AddOrUpdate(User user);
    }
}