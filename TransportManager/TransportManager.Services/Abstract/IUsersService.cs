using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportManager.Domain;
using TransportManager.Events;
using TransportManager.Models;

namespace TransportManager.Services.Abstract
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