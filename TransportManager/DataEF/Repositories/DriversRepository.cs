using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DataEF.DbContext;
using DataEF.Repositories.Abstract;
using Domain;
using Entities;
using Exceptions;
using Mappers;

namespace DataEF.Repositories
{
    public class DriversRepository : IDriversRepository
    {
        public async Task<DriverEntity> AddOrUpdateDriverAsync(Driver driver)
        {
            if (driver == null) throw new NullReferenceException();

            try
            {
                var driverEntity = driver.ToEntity();

                using (var context = new EfDbContext())
                {
                    var entity = await context.Drivers.Where(d => d.Id == driver.Id)
                                                      .Include(d => d.Vehicles)
                                                      .FirstOrDefaultAsync();
                    
                    //проверяем/меняем CompanyId у ТС
                    if (entity != null && driverEntity.CompanyId != entity.CompanyId)
                    {
                        var vehicles = entity.Vehicles.ToList();

                        vehicles.ForEach(vehicle =>
                        {
                            vehicle.CompanyId = driver.CompanyId;
                            context.Vehicles.AddOrUpdate(vehicle);
                        });
                    }

                    context.Drivers.AddOrUpdate(driverEntity);

                    await context.SaveChangesAsync();

                    return driverEntity;
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

            try
            {
                using (var context = new EfDbContext())
                {
                    var driverEntity = await context.Drivers.FindAsync(id);

                    if (driverEntity == null) throw new NullReferenceException(nameof(DriverEntity));

                    var vehicles = driverEntity.Vehicles.ToList();
                    vehicles.ForEach(vehicle =>
                    {
                        vehicle.DriverId = null;
                        context.Vehicles.AddOrUpdate(vehicle);
                    });
                    
                    context.Drivers.Remove(driverEntity);

                    await context.SaveChangesAsync();

                    return driverEntity;
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

            try
            {
                using (var context = new EfDbContext())
                {
                    return await context.Drivers.Where(driver => driver.Id == id)
                                                .Include(driver => driver.Vehicles)
                                                .FirstOrDefaultAsync();
                }
            }
            catch (Exception e)
            {
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
        }

        public async Task<List<DriverEntity>> GetAllDriversAsync()
        {
            try
            {
                using (var context = new EfDbContext())
                {
                    return await context.Drivers.Include(driver => driver.Vehicles)
                                                .ToListAsync();
                }
            }
            catch (Exception e)
            {
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
        }
        
        public async Task<DriverEntity> RemoveDriverAsync(int id)
        {
            if (id == default) return null;

            try
            {
                using (var context = new EfDbContext())
                {
                    var driver = await context.Drivers.Where(driverEntity => driverEntity.Id == id && !driverEntity.IsDeleted)
                                                      .Include(driverEntity => driverEntity.Vehicles)
                                                      .FirstOrDefaultAsync();

                    if (driver == null) throw new NullReferenceException(nameof(DriverEntity));
                    
                    var vehicles = driver.Vehicles.ToList();
                    vehicles.ForEach(vehicle =>
                    {
                        vehicle.DriverId = null;
                        context.Vehicles.AddOrUpdate(vehicle);
                    });

                    driver.IsDeleted = true;
                    driver.SoftDeletedDate = DateTime.Now;
                    context.Drivers.AddOrUpdate(driver);

                    await context.SaveChangesAsync();

                    return driver;
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