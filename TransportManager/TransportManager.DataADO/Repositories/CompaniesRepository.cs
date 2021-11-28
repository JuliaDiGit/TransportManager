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
    public class CompaniesRepository : ICompaniesRepository
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DbConnect"].ConnectionString;

        public async Task<CompanyEntity> AddOrUpdateCompanyAsync(Company company)
        {
            if (company == null) throw new NullReferenceException();
            
            const string addProcedure = "sp_InsertCompany";
            const string updProcedure = "sp_UpdateCompany";

            try
            {
                var parameters = new CompanyEntityParameters(company.Id,
                                                                company.CompanyId, 
                                                                company.CompanyName);
                
                using (var connection = new SqlConnection(_connectionString))
                {

                    SqlCommand command;

                    if (company.Id == default)
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
                    }

                    command.Parameters.Add(parameters.CompanyId);
                    command.Parameters.Add(parameters.CompanyName);

                    await connection.OpenAsync();

                    if (company.Id == default)
                    {
                        company.Id = Convert.ToInt32(await command.ExecuteScalarAsync());
                    }
                    else await command.ExecuteNonQueryAsync();

                    return company.ToEntity();
                }
            }
            catch (Exception e)
            {
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
        }

        public async Task<CompanyEntity> DeleteCompanyAsync(int id)
        {
            if (id == default) return null;
            
            CompanyEntity company = await GetCompanyAsync(id);

            if (company == null) return null;

            const string procedure = "sp_DeleteCompanyByCompanyId";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand(procedure, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue(@"CompanyId", company.CompanyId);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    return company;
                }
            }
            catch (Exception e)
            {
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
        }
        
        public async Task<CompanyEntity> GetCompanyAsync(int id)
        { 
            if (id == default) return null;

            const string procedure = "sp_GetCompanyWithDriversAndVehiclesByNextResult";
            
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
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        CompanyEntity company = null;
                        DriverEntity driver = null;
                        ConcurrentDictionary<int, VehicleEntity> vehicles = null;
                        var drivers = new ConcurrentDictionary<int, DriverEntity>();

                        while (await reader.ReadAsync())
                        {
                            if (company == null)
                            {
                                company = new CompanyEntity()
                                {
                                    Id = reader.GetInt32(0),
                                    CreatedDate = reader.GetDateTime(1),
                                    CompanyId = reader.GetInt32(2),
                                    CompanyName = reader.GetString(3),
                                    Drivers = new List<DriverEntity>(),
                                    Vehicles = new List<VehicleEntity>()
                                };
                            }

                            if (!reader.IsDBNull(4))
                            {
                                var nextDriver = new DriverEntity()
                                {
                                    Id = reader.GetInt32(4),
                                    CreatedDate = reader.GetDateTime(5),
                                    CompanyId = company.CompanyId,
                                    Name = reader.GetString(7),
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

                                if (!reader.IsDBNull(8))
                                {
                                    vehicles.TryAdd(vehicles.Count, new VehicleEntity
                                    {
                                        Id = reader.GetInt32(8),
                                        CreatedDate = reader.GetDateTime(9),
                                        CompanyId = driver.CompanyId,
                                        DriverId = driver.Id,
                                        Model = reader.GetString(12),
                                        GovernmentNumber = reader.GetString(13)
                                    });
                                }
                            }
                        }

                        if (company != null)
                        {
                            if (vehicles != null)
                            {
                                foreach (var vehiclePair in vehicles)
                                {
                                    driver.Vehicles.ToList().Insert(vehiclePair.Key, vehiclePair.Value);
                                }
                            }

                            drivers.TryAdd(drivers.Count, driver);

                            foreach (var driverPair in drivers)
                            {
                                company.Drivers.ToList().Insert(driverPair.Key, driverPair.Value);
                            }
                        }
                        else return null;

                        await reader.NextResultAsync();

                        vehicles = new ConcurrentDictionary<int, VehicleEntity>();

                        while (await reader.ReadAsync())
                        {
                            if (!reader.IsDBNull(4))
                            {
                                vehicles.TryAdd(vehicles.Count, new VehicleEntity
                                {
                                    Id = reader.GetInt32(4),
                                    CreatedDate = reader.GetDateTime(5),
                                    CompanyId = company.CompanyId,
                                    DriverId = reader.IsDBNull(7) ? (int?) null : reader.GetInt32(7),
                                    Model = reader.GetString(8),
                                    GovernmentNumber = reader.GetString(9)
                                });
                            }
                        }

                        reader.Close();

                        foreach (var vehiclePair in vehicles)
                        {
                            company.Vehicles.ToList().Insert(vehiclePair.Key, vehiclePair.Value);
                        }

                        return company;
                    }
                }
            }
            catch (Exception e)
            {
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
            
            return null;
        }

        public async Task<List<CompanyEntity>> GetAllCompaniesAsync()
        {
            const string procedure = "sp_GetAllCompaniesWithDriversAndVehiclesByNextResult";
            var companies = new ConcurrentDictionary<int, CompanyEntity>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand(procedure, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    await connection.OpenAsync();
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        CompanyEntity company = null;
                        DriverEntity driver = null;
                        ConcurrentDictionary<int, VehicleEntity> vehicles = null;
                        ConcurrentDictionary<int, DriverEntity> drivers = null;

                        //из первой таблицы запроса запоняем список компаний и её водителей
                        while (await reader.ReadAsync())
                        {
                            var nextCompany = new CompanyEntity()
                            {
                                Id = reader.GetInt32(0),
                                CreatedDate = reader.GetDateTime(1),
                                CompanyId = reader.GetInt32(2),
                                CompanyName = reader.GetString(3),
                                Drivers = new List<DriverEntity>(),
                                Vehicles = new List<VehicleEntity>()
                            };

                            //заходим если это первый вызов или если данные для чтения по текущей компании закончились
                            if (company == null || company.Id != nextCompany.Id)
                            {
                                if (company != null)
                                {
                                    //перед добавлением компании в список - заполняем её вложенные данные
                                    if (vehicles != null)
                                    {
                                        //заполняем ТС у водителей
                                        foreach (var vehiclePair in vehicles)
                                        {
                                            driver.Vehicles.ToList().Insert(vehiclePair.Key, vehiclePair.Value);
                                        }
                                    }

                                    //добавляем текущего водителя в список водителей
                                    drivers.TryAdd(drivers.Count, driver);
                                    
                                    //запоняем водителей у компании
                                    foreach (var driverPair in drivers)
                                    {
                                        company.Drivers.ToList().Insert(driverPair.Key, driverPair.Value);
                                    }

                                    companies.TryAdd(companies.Count, company);
                                }

                                //присваиваем компании новые прочтённые данные
                                company = nextCompany;
                                //создаем новый список водителей, т.к новая компания
                                drivers = new ConcurrentDictionary<int, DriverEntity>();
                            }

                            //если водители есть - считываем данные (если столбец id водителя не null)
                            if (!reader.IsDBNull(4))
                            {
                                var nextDriver = new DriverEntity()
                                {
                                    Id = reader.GetInt32(4),
                                    CreatedDate = reader.GetDateTime(5),
                                    CompanyId = company.CompanyId,
                                    Name = reader.GetString(7),
                                    Vehicles = new List<VehicleEntity>()
                                };

                                if (driver == null || driver.Id != nextDriver.Id)
                                {
                                    if (driver != null && driver.CompanyId == company.CompanyId)
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

                                //если ТС есть - считываем данные (если столбец id ТС не null)
                                if (!reader.IsDBNull(8))
                                {
                                    vehicles.TryAdd(vehicles.Count, new VehicleEntity
                                    {
                                        Id = reader.GetInt32(8),
                                        CreatedDate = reader.GetDateTime(9),
                                        CompanyId = driver.CompanyId,
                                        DriverId = driver.Id,
                                        Model = reader.GetString(12),
                                        GovernmentNumber = reader.GetString(13)
                                    });
                                }
                            }
                        }
                        
                        if (company == null) return null;
                        
                        //из-за конца цикла последняя считанная инфа никуда не сохранилась,
                        //поэтому добавляем её в соответсующие списки
                        
                        if (vehicles != null)
                        {
                            foreach (var vehiclePair in vehicles)
                            {
                                driver.Vehicles.ToList().Insert(vehiclePair.Key, vehiclePair.Value);
                            }
                        }

                        drivers.TryAdd(drivers.Count, driver);

                        foreach (var driverPair in drivers)
                        {
                            company.Drivers.ToList().Insert(driverPair.Key, driverPair.Value);
                        }


                        companies.TryAdd(companies.Count, company);

                        //переходим ко второй таблице для заполнения списка ТС компании
                        await reader.NextResultAsync();

                        company = null;
                        vehicles = new ConcurrentDictionary<int, VehicleEntity>();

                        while (await reader.ReadAsync())
                        {
                            var nextCompanyId = reader.GetInt32(0);

                            if (company == null || company.Id != nextCompanyId)
                            {
                                if (company != null)
                                {
                                    foreach (var vehiclePair in vehicles)
                                    {
                                        company.Vehicles.ToList().Insert(vehiclePair.Key, vehiclePair.Value);
                                    }
                                }

                                //выбираем компанию из созданного ранее списка по ИД
                                company = companies.Single(companyPair => companyPair.Value.Id == nextCompanyId)
                                                   .Value;
                            }

                            //если есть ТС - считываем (если столбец id ТС не null)
                            if (!reader.IsDBNull(4))
                            {
                                vehicles.TryAdd(vehicles.Count, new VehicleEntity
                                {
                                    Id = reader.GetInt32(4),
                                    CreatedDate = reader.GetDateTime(5),
                                    CompanyId = company.CompanyId,
                                    DriverId = reader.IsDBNull(7) ? (int?) null : reader.GetInt32(7),
                                    Model = reader.GetString(8),
                                    GovernmentNumber = reader.GetString(9)
                                });
                            }
                        }
                        
                        if (company == null) return null;

                        foreach (var vehiclePair in vehicles)
                        {
                            company.Vehicles.ToList().Insert(vehiclePair.Key, vehiclePair.Value);
                        }
                    }

                    reader.Close();

                    var companiesList = new List<CompanyEntity>();

                    foreach (var companyPair in companies)
                    {
                        companiesList.Insert(companyPair.Key, companyPair.Value);
                    }

                    return companiesList;
                }
            }
            catch (Exception e)
            {
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
        }

        public async Task<CompanyEntity> GetCompanyByCompanyIdAsync(int companyId)
        {
            if (companyId == default) return null;

            const string procedure = "sp_GetCompanyWithDriversAndVehiclesByCompanyIdByNextResult";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand(procedure, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue(@"CompanyId", companyId);

                    await connection.OpenAsync();
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        CompanyEntity company = null;
                        DriverEntity driver = null;
                        ConcurrentDictionary<int, VehicleEntity> vehicles = null;
                        var drivers = new ConcurrentDictionary<int, DriverEntity>();

                        while (await reader.ReadAsync())
                        {
                            if (company == null)
                            {
                                company = new CompanyEntity()
                                {
                                    Id = reader.GetInt32(0),
                                    CreatedDate = reader.GetDateTime(1),
                                    CompanyId = reader.GetInt32(2),
                                    CompanyName = reader.GetString(3),
                                    Drivers = new List<DriverEntity>(),
                                    Vehicles = new List<VehicleEntity>()
                                };
                            }

                            if (!reader.IsDBNull(4))
                            {
                                var nextDriver = new DriverEntity()
                                {
                                    Id = reader.GetInt32(4),
                                    CreatedDate = reader.GetDateTime(5),
                                    CompanyId = company.CompanyId,
                                    Name = reader.GetString(7),
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

                                if (!reader.IsDBNull(8))
                                {
                                    vehicles.TryAdd(vehicles.Count, new VehicleEntity
                                    {
                                        Id = reader.GetInt32(8),
                                        CreatedDate = reader.GetDateTime(9),
                                        CompanyId = driver.CompanyId,
                                        DriverId = driver.Id,
                                        Model = reader.GetString(12),
                                        GovernmentNumber = reader.GetString(13)
                                    });
                                }
                            }
                        }

                        if (company != null)
                        {
                            if (vehicles != null)
                            {
                                foreach (var vehiclePair in vehicles)
                                {
                                    driver.Vehicles.ToList().Insert(vehiclePair.Key, vehiclePair.Value);
                                }
                            }

                            drivers.TryAdd(drivers.Count, driver);

                            foreach (var driverPair in drivers)
                            {
                                company.Drivers.ToList().Insert(driverPair.Key, driverPair.Value);
                            }
                        }
                        else return null;

                        await reader.NextResultAsync();

                        vehicles = new ConcurrentDictionary<int, VehicleEntity>();

                        while (await reader.ReadAsync())
                        {
                            if (!reader.IsDBNull(4))
                            {
                                vehicles.TryAdd(vehicles.Count, new VehicleEntity
                                {
                                    Id = reader.GetInt32(0),
                                    CreatedDate = reader.GetDateTime(1),
                                    CompanyId = company.CompanyId,
                                    DriverId = reader.IsDBNull(3) ? (int?) null : reader.GetInt32(3),
                                    Model = reader.GetString(4),
                                    GovernmentNumber = reader.GetString(5)
                                });
                            }
                        }

                        reader.Close();

                        foreach (var vehiclePair in vehicles)
                        {
                            company.Vehicles.ToList().Insert(vehiclePair.Key, vehiclePair.Value);
                        }

                        return company;
                    }
                }
            }
            catch (Exception e)
            {
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }

            return null;
        }
    }
}