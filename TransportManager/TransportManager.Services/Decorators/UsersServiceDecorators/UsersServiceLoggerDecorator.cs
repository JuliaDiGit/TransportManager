using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportManager.Domain;
using TransportManager.Events;
using TransportManager.Loggers.Abstract;
using TransportManager.Models;
using TransportManager.Services.Abstract;
using TransportManager.Services.Properties;

namespace TransportManager.Services.Decorators.UsersServiceDecorators
{
    public class UsersServiceLoggerDecorator : IUsersService
    {
        private readonly ILogger _logger;
        private readonly IUsersService _inner;

        public UsersServiceLoggerDecorator(ILogger logger, IUsersService inner)
        {
            _logger = logger;
            _inner = inner;
        }

        public async Task<User> AddOrUpdateUserAsync(UserModel userModel)
        {
            try
            {
                User user = await _inner.AddOrUpdateUserAsync(userModel);

                string status = user == null
                                ? Resources.Status_Fail 
                                : Resources.Status_Success;

                _logger.Trace($"{DateTime.Now} - " +
                              $"{Resources.Operation_AddOrUpdateUser} - " +
                              $"{status}");

                return user;
            }
            catch (Exception e)
            {
                _logger.Error($"{DateTime.Now} - " +
                              $"{Resources.Operation_DeleteUser} - " +
                              $"{e.GetType()}");
                throw;
            }
        }

        public async Task<User> DeleteUserByIdAsync(int id)
        {
            try
            {
                User user = await _inner.DeleteUserByIdAsync(id);

                string status = user == null 
                                ? Resources.Status_Fail 
                                : Resources.Status_Success;

                _logger.Trace($"{DateTime.Now} - " +
                              $"{Resources.Operation_DeleteUser} - " +
                              $"{status}");

                return user;
            }
            catch (Exception e)
            {
                _logger.Error($"{DateTime.Now} - " +
                              $"{Resources.Operation_DeleteUser} - " +
                              $"{e.GetType()}");
                throw;
            }
        }

        public async Task<User> GetUserByLoginAsync(string login)
        {
            try
            {
                User user = await _inner.GetUserByLoginAsync(login);

                string status = user == null
                                ? Resources.Status_Fail 
                                : Resources.Status_Success;

                _logger.Trace($"{DateTime.Now} - " +
                              $"{Resources.Operation_GetUserByLogin} - " +
                              $"{status}");

                return user;
            }
            catch (Exception e)
            {
                _logger.Error($"{DateTime.Now} - " +
                              $"{Resources.Operation_GetUserByLogin} - " +
                              $"{e.GetType()}");
                throw;
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                List<User> users = await _inner.GetAllUsersAsync();

                string status = users == null
                                ? Resources.Status_Fail 
                                : Resources.Status_Success;

                _logger.Trace($"{DateTime.Now} - " +
                              $"{Resources.Operation_GetAllUsers} - " +
                              $"{status}");

                return users;
            }
            catch (Exception e)
            {
                _logger.Error($"{DateTime.Now} - " +
                              $"{Resources.Operation_GetAllUsers} - " +
                              $"{e.GetType()}");
                throw;
            }
        }

        public async Task<User> RemoveUserByIdAsync(int id)
        {
            try
            {
                User user = await _inner.RemoveUserByIdAsync(id);

                string status = user == null
                                ? Resources.Status_Fail 
                                : Resources.Status_Success;

                _logger.Trace($"{DateTime.Now} - " +
                              $"{Resources.Operation_RemoveUser} - " +
                              $"{status}");

                return user;
            }
            catch (Exception e)
            {
                _logger.Error($"{DateTime.Now} - " +
                              $"{Resources.Operation_RemoveUser} - " +
                              $"{e.GetType()}");
                throw;
            }
        }

        public event EventHandler<SendEventArgs> SendingEvent;
    }
}
