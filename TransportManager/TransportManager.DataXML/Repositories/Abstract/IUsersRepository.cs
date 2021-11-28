using TransportManager.Domain;
using TransportManager.Entities;

namespace TransportManager.DataXML.Repositories.Abstract
{
    public interface IUsersRepository : IBaseRepository<UserEntity>
    {
        UserEntity GetUserByLogin(string login);
        UserEntity AddOrUpdate(User user);
    }
}