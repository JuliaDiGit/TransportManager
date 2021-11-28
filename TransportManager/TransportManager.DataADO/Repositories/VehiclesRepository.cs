using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TransportManager.Common.Exceptions;
using TransportManager.Domain;
using TransportManager.Entities;
using TransportManager.Mappers;
using TransportManager.DataADO.EntitiesParameters;
using TransportManager.DataADO.Repositories.Abstract;

namespace TransportManager.DataADO.Repositories
{
    public class VehiclesRepository : IVehiclesRepository
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DbConnect"].ConnectionString;

        public async Task<VehicleEntity> AddOrUpdateVehicleAsync(Vehicle vehicle)
        {
            if (vehicle == null) throw new NullReferenceException();
            
            const string addProcedure = "sp_InsertVehicle";
            const string updProcedure = "sp_UpdateVehicle";

            try
            {
                var parameters = new VehicleEntityParameters(vehicle.Id,
                                                                vehicle.CompanyId, 
                                                                vehicle.DriverId, 
                                                                vehicle.Model, 
                                                                vehicle.GovernmentNumber);

                using (var connection = new SqlConnection(_connectionString))
                {
                    SqlCommand command;

                    if (vehicle.Id == 0)
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

                    command.Parameters.Add(parameters.CompanyId);
                    command.Parameters.Add(parameters.DriverId);
                    command.Parameters.Add(parameters.Model);
                    command.Parameters.Add(parameters.GovernmentNumber);

                    await connection.OpenAsync();

                    if (vehicle.Id == 0)
                    {
                        vehicle.Id = Convert.ToInt32(await command.ExecuteScalarAsync());
                    }
                    else await command.ExecuteNonQueryAsync();

                    return vehicle.ToEntity();

                }
            }
            catch (Exception e)
            {
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
        }

        public async Task<VehicleEntity> DeleteVehicleAsync(int id)
        {
            if (id == default) return null;
            
            VehicleEntity vehicle = await GetVehicleAsync(id);

            if (vehicle == null) return null;
            
            const string procedure = "sp_DeleteVehicle";

            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var command = new SqlCommand(procedure, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    
                    command.Parameters.AddWithValue("@Id", id);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    return vehicle;
                }
                catch (Exception e)
                {
                    if (e is SqlException) throw new SourceNotAvailableException();
                    return null;
                }
            }
        }

        public async Task<VehicleEntity> GetVehicleAsync(int id)
        {
            if (id == default) return null;
            
            const string procedure = "sp_GetVehicle";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand(procedure, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@Id", id);

                    await connection.OpenAsync();
                    
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                return new VehicleEntity()
                                {
                                    Id = reader.GetInt32(0),
                                    CreatedDate = reader.GetDateTime(1),
                                    CompanyId = reader.GetInt32(2),
                                    DriverId = reader.IsDBNull(3) ? (int?) null : reader.GetInt32(3),
                                    Model = reader.GetString(4),
                                    GovernmentNumber = reader.GetString(5)
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

        public async Task<List<VehicleEntity>> GetAllVehiclesAsync()
        {
            var vehicles = new ConcurrentDictionary<int, VehicleEntity>();
            const string procedure = "sp_GetAllVehicles";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var command = new SqlCommand(procedure, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                vehicles.TryAdd(vehicles.Count, new VehicleEntity()
                                {
                                    Id = reader.GetInt32(0),
                                    CreatedDate = reader.GetDateTime(1),
                                    CompanyId = reader.GetInt32(2),
                                    DriverId = reader.IsDBNull(3) ? (int?) null : reader.GetInt32(3),
                                    Model = reader.GetString(4),
                                    GovernmentNumber = reader.GetString(5)
                                });
                            }
                        }
                    }

                    var vehiclesList = new List<VehicleEntity>();
                    foreach (var vehicle in vehicles)
                    {
                        vehiclesList.Insert(vehicle.Key, vehicle.Value);
                    }

                    return vehiclesList;
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