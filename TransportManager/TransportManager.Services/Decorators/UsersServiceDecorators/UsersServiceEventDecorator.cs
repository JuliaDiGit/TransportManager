using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportManager.Domain;
using TransportManager.Events;
using TransportManager.Models;
using TransportManager.Services.Abstract;
using TransportManager.Services.Properties;

namespace TransportManager.Services.Decorators.UsersServiceDecorators
{
    public class UsersServiceEventDecorator : IUsersService
    {
        private readonly IUsersService _usersService;

        public UsersServiceEventDecorator(IUsersService usersService)
        {
            _usersService = usersService;
        }

        public async Task<User> AddOrUpdateUserAsync(UserModel userModel)
        {
            try
            {
                User user = await _usersService.AddOrUpdateUserAsync(userModel);

                string status = user == null
                                ? Resources.Status_Fail
                                : Resources.Status_Success;

                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_AddOrUpdateUser, 
                                                             status,
                                                             userModel.Id));

                return user;
            }
            catch (Exception e)
            {
                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_AddOrUpdateUser,
                                                             e.GetType().ToString(), 
                                                             userModel.Id));
                
                throw;
            }
        }

        public async Task<User> DeleteUserByIdAsync(int id)
        {
            return await _usersService.DeleteUserByIdAsync(id);
        }

        public async Task<User> GetUserByLoginAsync(string login)
        {
            return await _usersService.GetUserByLoginAsync(login);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _usersService.GetAllUsersAsync();
        }

        public async Task<User> RemoveUserByIdAsync(int id)
        {
            return await _usersService.RemoveUserByIdAsync(id);
        }

        public event EventHandler<SendEventArgs> SendingEvent;
    }
}
