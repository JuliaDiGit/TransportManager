using System.Linq;
using TransportManager.Domain;
using TransportManager.Entities;
using TransportManager.Mappers;
using TransportManager.DataXML.Repositories.Abstract;

namespace TransportManager.DataXML.Repositories
{
    public class UsersRepository : BaseRepository<UserEntity>, IUsersRepository
    {
        public UsersRepository(string path) : base(path)
        {
        }
        
        public UserEntity GetUserByLogin(string login)
        {
            return GetAll().FirstOrDefault(user => user.Login.ToLower() == login.ToLower());
        }

        public UserEntity AddOrUpdate(User user)
        {
            return base.AddOrUpdate(user.ToEntity());
        }
    }
}