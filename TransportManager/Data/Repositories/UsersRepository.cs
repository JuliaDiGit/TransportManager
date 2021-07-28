using System.Linq;
using Data.Repositories.Abstract;
using Domain;
using Entities;
using Mappers;

namespace Data.Repositories
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