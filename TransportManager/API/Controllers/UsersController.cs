using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Events;
using Mappers;
using Models;
using Services.Abstract;
using Models.Validation;

namespace API.Controllers
{
    public class UsersController
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
            _usersService.SendingEvent += new MessageHandlerBox().OnSendingEventShowNotification;
        }
        
        public async Task<List<UserModel>> GetAllUsersAsync()
        {
            return (await _usersService.GetAllUsersAsync())
                                       .Select(user => user.ToModel())
                                       .ToList();
        }

        public async Task<UserModel> AddOrUpdateUserAsync(UserModel userModel)
        {
            if (userModel == null) throw new NullReferenceException();
            
            ValidationFilter.Validate(userModel);

            return (await _usersService.AddOrUpdateUserAsync(userModel))
                                       .ToModel();
        }

        public async Task<UserModel> GetUserByLoginAsync(string login)
        {
            if (login == null) throw new NullReferenceException();
            
            return (await _usersService.GetUserByLoginAsync(login))
                                       .ToModel();
        }

        public async Task<UserModel> DeleteUserByIdAsync(int id)
        {
            if (id == default) return null;
            
            return (await _usersService.DeleteUserByIdAsync(id))
                                       .ToModel();
        }

        public async Task<UserModel> RemoveUserByIdAsync(int id)
        {
            if (id == default) return null;

            return (await _usersService.RemoveUserByIdAsync(id))
                                       .ToModel();
        }
    }
}