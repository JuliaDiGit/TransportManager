using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TransportManager.Domain;
using TransportManager.Entities;
using TransportManager.Common.Enums;
using TransportManager.Common.Exceptions;
using TransportManager.Mappers;
using TransportManager.DataADO.EntitiesParameters;
using TransportManager.DataADO.Repositories.Abstract;

namespace TransportManager.DataADO.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DbConnect"].ConnectionString;

        public async Task<UserEntity> AddOrUpdateUserAsync(User user)
        {
            if (user == null) throw new NullReferenceException();
            
            const string addProcedure = "sp_InsertUser";
            const string updProcedure = "sp_UpdateUser";

            try
            {
                var parameters = new UserEntityParameters(user.Id,
                                                             user.Login, 
                                                             user.Password,
                                                             (int) user.Role);

                using (var connection = new SqlConnection(_connectionString))
                {
                    SqlCommand command;

                    if (user.Id == 0)
                    {
                        command = new SqlCommand(addProcedure, connection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                    }
                    else
                    {
                        command = new SqlCommand(updProcedure, connection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        
                        command.Parameters.Add(parameters.Id);
                    }

                    command.Parameters.Add(parameters.Login);
                    command.Parameters.Add(parameters.Password);
                    command.Parameters.Add(parameters.Role);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    return user.ToEntity();
                }
            }
            catch (Exception e)
            {
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
        }

        public async Task<UserEntity> DeleteUserAsync(int id)
        {
            if (id == default) return null;
            
            UserEntity user = await GetUserAsync(id);

            if (user == null) return null;

            const string procedure = "sp_DeleteUser";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand(procedure, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue(@"Id", id);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

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
            
            const string procedure = "sp_GetUser";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand(procedure, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue(@"Id", id);

                    await connection.OpenAsync();
                    
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                return new UserEntity
                                {
                                    Id = reader.GetInt32(0),
                                    CreatedDate = reader.GetDateTime(1),
                                    Login = reader.GetString(2),
                                    Password = reader.GetString(3),
                                    Role = (Role) Enum.GetValues(typeof(Role))
                                                            .GetValue(reader.GetInt32(4))
                                };
                            }
                        }
                    }
                    
                    return null;
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
            var users = new ConcurrentDictionary<int, UserEntity>();
            const string procedure = "sp_GetAllUser";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand(procedure, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    await connection.OpenAsync();
                    
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                users.TryAdd(users.Count, new UserEntity
                                {
                                    Id = reader.GetInt32(0),
                                    CreatedDate = reader.GetDateTime(1),
                                    Login = reader.GetString(2),
                                    Password = reader.GetString(3),
                                    Role = (Role) Enum.GetValues(typeof(Role))
                                                            .GetValue(reader.GetInt32(4))
                                });
                            }
                        }
                    }

                    var usersList = new List<UserEntity>();

                    foreach (var userPair in users)
                    {
                        usersList.Insert(userPair.Key, userPair.Value);
                    }
                    
                    return usersList;
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
            
            const string procedure = "sp_GetUserByLogin";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand(procedure, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue(@"Login", login);

                    await connection.OpenAsync();
                    
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                return new UserEntity
                                {
                                    Id = reader.GetInt32(0),
                                    CreatedDate = reader.GetDateTime(1),
                                    Login = reader.GetString(2),
                                    Password = reader.GetString(3),
                                    Role = (Role) Enum.GetValues(typeof(Role))
                                                            .GetValue(reader.GetInt32(4))
                                };
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
        }
    }
}