using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DataEF.DbContext;
using DataEF.Repositories.Abstract;
using Domain;
using Entities;
using Exceptions;
using Mappers;

namespace DataEF.Repositories
{
    public class VehiclesRepository : IVehiclesRepository
    {
        public async Task<VehicleEntity> AddOrUpdateVehicleAsync(Vehicle vehicle)
        {
            if (vehicle == null) throw new NullReferenceException();

            try
            {
                var vehicleEntity = vehicle.ToEntity();

                using (var context = new EfDbContext())
                {
                    var entity = await context.Set<VehicleEntity>().FindAsync(vehicle.Id);

                    context.Vehicles.AddOrUpdate(vehicleEntity);

                    await context.SaveChangesAsync();

                    return vehicleEntity;
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

            try
            {
                using (var context = new EfDbContext())
                {
                    var entity = await context.Vehicles.FindAsync(id);

                    if (entity == null) throw new NullReferenceException(nameof(VehicleEntity));

                    context.Vehicles.Remove(entity);

                    await context.SaveChangesAsync();

                    return entity;
                }
            }
            catch (Exception e)
            {
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
        }

        public async Task<VehicleEntity> GetVehicleAsync(int id)
        {
            if (id == default) return null;

            try
            {
                using (var context = new EfDbContext())
                {
                    return await context.Vehicles.FindAsync(id);
                }
            }
            catch (Exception e)
            {
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
        }

        public async Task<List<VehicleEntity>> GetAllVehiclesAsync()
        {
            try
            {
                using (var context = new EfDbContext())
                {
                    return await context.Vehicles.ToListAsync();
                }
            }
            catch (Exception e)
            {
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
        }
        
        public async Task<VehicleEntity> RemoveVehicleAsync(int id)
        {
            if (id == default) return null;

            try
            {
                using (var context = new EfDbContext())
                {
                    var vehicle = await context.Vehicles.FindAsync(id);

                    if (vehicle == null || vehicle.IsDeleted) throw new NullReferenceException(nameof(VehicleEntity));

                    vehicle.IsDeleted = true;
                    vehicle.SoftDeletedDate = DateTime.Now;

                    context.Vehicles.AddOrUpdate(vehicle);

                    await context.SaveChangesAsync();

                    return vehicle;
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