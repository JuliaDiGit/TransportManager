using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Events;
using Models;

namespace Services.Abstract
{
    public interface IUsersService
    {
        Task<User> AddOrUpdateUserAsync(UserModel userModel);
        Task<User> DeleteUserByIdAsync(int id);
        Task<User> GetUserByLoginAsync(string login);
        Task<List<User>> GetAllUsersAsync();
        Task<User> RemoveUserByIdAsync(int id);

        event EventHandler<SendEventArgs> SendingEvent;
    }
}