using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Entities;

namespace DataEF.Repositories.Abstract
{
    public interface IUsersRepository
    {
        Task<UserEntity> AddOrUpdateUserAsync(User user);
        Task<UserEntity> DeleteUserAsync(int id);
        Task<UserEntity> GetUserAsync(int id);
        Task<List<UserEntity>> GetAllUsersAsync();
        Task<UserEntity> GetUserByLoginAsync(string login);
        Task<UserEntity> RemoveUserAsync(int id);
    }
}