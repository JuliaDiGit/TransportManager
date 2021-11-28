using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TransportManager.Common.Exceptions;
using TransportManager.Domain;
using TransportManager.Entities;
using TransportManager.Mappers;
using TransportManager.DataEF.DbContext;
using TransportManager.DataEF.Repositories.Abstract;

namespace TransportManager.DataEF.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        public async Task<UserEntity> AddOrUpdateUserAsync(User user)
        {
            if (user == null) throw new NullReferenceException();

            try
            {
                var userEntity = user.ToEntity();

                using (var context = new EfDbContext())
                {
                    var entity = await context.Users.FindAsync(user.Id);

                    context.Users.AddOrUpdate(userEntity);

                    await context.SaveChangesAsync();

                    return userEntity;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
        }

        public async Task<UserEntity> DeleteUserAsync(int id)
        {
            if (id == default) return null;

            try
            {
                using (var context = new EfDbContext())
                {
                    var user = await context.Users.FindAsync(id);

                    if (user == null) throw new NullReferenceException(nameof(UserEntity));

                    context.Users.Remove(user);

                    await context.SaveChangesAsync();

                    return user;
                }
            }
            catch (Exception e)
            {
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
        }

        public async Task<UserEntity> GetUserAsync(int id)
        {
            if (id == default) return null;

            try
            {
                using (var context = new EfDbContext())
                {
                    return await context.Users.FindAsync(id);
                }
            }
            catch (Exception e)
            {
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
        }

        public async Task<List<UserEntity>> GetAllUsersAsync()
        {
            try
            {
                using (var context = new EfDbContext())
                {
                    return await context.Users.ToListAsync();
                }
            }
            catch (Exception e)
            {
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
        }

        public async Task<UserEntity> GetUserByLoginAsync(string login)
        {
            if (login == null) throw new NullReferenceException();

            try
            {
                using (var context = new EfDbContext())
                {
                    return await context.Users.Where(user => user.Login == login)
                                              .FirstOrDefaultAsync();
                }
            }
            catch (Exception e)
            {
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
        }
        
        public async Task<UserEntity> RemoveUserAsync(int id)
        {
            if (id == default) return null;

            try
            {
                using (var context = new EfDbContext())
                {
                    var user = await context.Users.FindAsync(id);

                    if (user == null) throw new NullReferenceException(nameof(UserEntity));

                    user.IsDeleted = true;
                    user.SoftDeletedDate = DateTime.Now;

                    context.Users.AddOrUpdate(user);

                    await context.SaveChangesAsync();

                    return user;
                }
            }
            catch (Exception e)
            {
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
        }
    }
}