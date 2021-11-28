using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TransportManager.Common.Exceptions;
using TransportManager.Domain;
using TransportManager.Entities;
using TransportManager.Mappers;
using TransportManager.DataADO.EntitiesParameters;
using TransportManager.DataADO.Repositories.Abstract;

namespace TransportManager.DataADO.Repositories
{
    public class DriversRepository : IDriversRepository
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DbConnect"].ConnectionString;

        public async Task<DriverEntity> AddOrUpdateDriverAsync(Driver driver)
        {
            if (driver == null) throw new NullReferenceException();
            
            const string addProcedure = "sp_InsertDriver";
            const string updProcedure = "sp_UpdateDriver";

            try
            {
                var parameters = new DriverEntityParameters(driver.Id,
                                                               driver.CompanyId,
                                                               driver.Name);

                using (var connection = new SqlConnection(_connectionString))
                {
                    SqlCommand command;

                    if (driver.Id == 0)
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
                    command.Parameters.Add(parameters.Name);

                    await connection.OpenAsync();

                    if (driver.Id == 0)
                    {
                        driver.Id = Convert.ToInt32(await command.ExecuteScalarAsync());
                    }
                    else await command.ExecuteNonQueryAsync();

                    return driver.ToEntity();
                }
            }
            catch (Exception e)
            {
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
        }

        public async Task<DriverEntity> DeleteDriverAsync(int id)
        {
            if (id == default) return null;
            
            DriverEntity driver = await GetDriverAsync(id);

            if (driver == null) return null;
            
            const string procedure = "sp_DeleteDriver";

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
                    await command.ExecuteNonQueryAsync();
                    return driver;
                }
            }
            catch (Exception e)
            {
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
        }
        
        public async Task<DriverEntity> GetDriverAsync(int id)
        {
            if (id == default) return null;
            
            const string procedure = "sp_GetDriverWithVehicles";
            var vehicles = new ConcurrentDictionary<int, VehicleEntity>();

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
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        DriverEntity driver = null;

                        while (await reader.ReadAsync())
                        {
                            if (driver == null)
                            {
                                driver = new DriverEntity()
                                {
                                    Id = reader.GetInt32(0),
                                    CreatedDate = reader.GetDateTime(1),
                                    CompanyId = reader.GetInt32(2),
                                    Name = reader.GetString(3),
                                    Vehicles = new List<VehicleEntity>()
                                };
                            }

                            //если есть ТС - считываем (если столбец id ТС не null)
                            if (!reader.IsDBNull(4))
                            {
                                vehicles.TryAdd(vehicles.Count, new VehicleEntity
                                {
                                    Id = reader.GetInt32(4),
                                    CreatedDate = reader.GetDateTime(5),
                                    CompanyId = driver.CompanyId,
                                    DriverId = driver.Id,
                                    Model = reader.GetString(8),
                                    GovernmentNumber = reader.GetString(9)
                                });
                            }
                        }

                        reader.Close();

                        if (driver == null) return null;
                        
                        foreach (var vehiclePair in vehicles)
                        {
                            driver.Vehicles.ToList().Insert(vehiclePair.Key, vehiclePair.Value);
                        }

                        return driver;
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

        public async Task<List<DriverEntity>> GetAllDriversAsync()
        {
            const string procedure = "sp_GetAllDriversWithVehicles";
            var drivers = new ConcurrentDictionary<int, DriverEntity>();
        
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var command = new SqlCommand(procedure, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    
                    await connection.OpenAsync();
                    SqlDataReader reader = await command.ExecuteReaderAsync();
        
                    if (reader.HasRows)
                    {
                        DriverEntity driver = null;
                        ConcurrentDictionary<int, VehicleEntity> vehicles = null;


                        while (await reader.ReadAsync())
                        {
                            var nextDriver = new DriverEntity()
                            {
                                Id = reader.GetInt32(0),
                                CreatedDate = reader.GetDateTime(1),
                                CompanyId = reader.GetInt32(2),
                                Name = reader.GetString(3),
                                Vehicles = new List<VehicleEntity>()
                            };

                            if (driver == null || driver.Id != nextDriver.Id)
                            {
                                if (driver != null)
                                {
                                    foreach (var vehiclePair in vehicles)
                                    {
                                        driver.Vehicles.ToList().Insert(vehiclePair.Key, vehiclePair.Value);
                                    }

                                    drivers.TryAdd(drivers.Count, driver);
                                }

                                driver = nextDriver;
                                vehicles = new ConcurrentDictionary<int, VehicleEntity>();
                            }
                            
                            //если есть ТС - считываем (если столбец id ТС не null)
                            if (!reader.IsDBNull(4))
                            {
                                vehicles.TryAdd(vehicles.Count, new VehicleEntity
                                {
                                    Id = reader.GetInt32(4),
                                    CreatedDate = reader.GetDateTime(5),
                                    CompanyId = driver.CompanyId,
                                    DriverId = driver.Id,
                                    Model = reader.GetString(8),
                                    GovernmentNumber = reader.GetString(9)
                                });
                            }
                        }

                        if (vehicles != null)
                        {
                            foreach (var vehiclePair in vehicles)
                            {
                                driver.Vehicles.ToList().Insert(vehiclePair.Key, vehiclePair.Value);
                            }
                        }

                        drivers.TryAdd(drivers.Count, driver);
                    }
                    
                    reader.Close();
                    
                    var driversList = new List<DriverEntity>();

                    foreach (var driverPair in drivers)
                    {
                        driversList.Insert(driverPair.Key, driverPair.Value);
                    }
        
                    return driversList;
                }
                catch (Exception e)
                {
                    if (e is SqlException) throw new SourceNotAvailableException();
                    return null;
                }
            }
        }
    }
}