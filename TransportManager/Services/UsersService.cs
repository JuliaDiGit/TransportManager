using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataEF.Repositories.Abstract;
using Domain;
using Enums;
using Events;
using Mappers;
using Models;
using Services.Abstract;

namespace Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;

        public UsersService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public async Task<User> AddOrUpdateUserAsync(UserModel userModel)
        {
            if (userModel == null) throw new NullReferenceException();;
            
            User user = userModel.ToDomain();

            if (user.Id == 0)
            {
                Role[] accesses = (Role[]) Enum.GetValues(typeof(Role));
                user.Role = accesses[new Random().Next(0, accesses.Length)];
            }

            return (await _usersRepository.AddOrUpdateUserAsync(user))
                                          .ToDomain();
        }

        public async Task<User> DeleteUserByIdAsync(int id)
        {
            if (id == default) return null;
            
            return (await _usersRepository.DeleteUserAsync(id))
                                          .ToDomain();
        }

        public async Task<User> GetUserByLoginAsync(string login)
        {
            if (login == null) return null;
            
            return (await _usersRepository.GetUserByLoginAsync(login))
                                          .ToDomain();
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return (await _usersRepository.GetAllUsersAsync())
                                          .Select(userEntity => userEntity.ToDomain())
                                          .ToList();
        }

        public async Task<User> RemoveUserByIdAsync(int id)
        {
            if (id == default) return null;

            return (await _usersRepository.RemoveUserAsync(id))
                                          .ToDomain();
        }

        public event EventHandler<SendEventArgs> SendingEvent;
    }
}