using System.Linq;
using Domain;
using Entities;
using Models;

namespace Mappers
{
    public static class DriverMapper
    {
        public static DriverEntity ToEntity(this Driver driver)
        {
            if (driver == null) return null;
            
            return new DriverEntity
            {
                Id = driver.Id,
                CreatedDate = driver.CreatedDate, 
                CompanyId = driver.CompanyId,
                Name = driver.Name,
                Vehicles = driver.Vehicles?.Select(vehicle => vehicle.ToEntity()).ToList(),
                SoftDeletedDate = driver.SoftDeletedDate,
                IsDeleted = driver.IsDeleted
            };
        }

        public static Driver ToDomain(this DriverModel driverModel)
        {
            if (driverModel == null) return null;

            return new Driver
            {
                Id = driverModel.Id,
                CreatedDate = driverModel.CreatedDate,
                CompanyId = driverModel.CompanyId,
                Name = driverModel.Name,
                Vehicles = driverModel.Vehicles?.Select(model => model.ToDomain()).ToList(),
                SoftDeletedDate = driverModel.SoftDeletedDate,
                IsDeleted = driverModel.IsDeleted
            };
        }
        
        public static Driver ToDomain(this DriverEntity driverEntity)
        {
            if (driverEntity == null) return null;

            return new Driver
            {
                Id = driverEntity.Id,
                CreatedDate = driverEntity.CreatedDate,
                CompanyId = driverEntity.CompanyId,
                Name = driverEntity.Name,
                Vehicles = driverEntity.Vehicles?.Select(entity => entity.ToDomain()).ToList(),
                SoftDeletedDate = driverEntity.SoftDeletedDate,
                IsDeleted = driverEntity.IsDeleted
            };
        }

        public static DriverModel ToModel(this Driver driver)
        {
            if (driver == null) return null;
            
            return new DriverModel
            {
                Id = driver.Id,
                CreatedDate = driver.CreatedDate,
                CompanyId = driver.CompanyId,
                Name = driver.Name,
                Vehicles = driver.Vehicles?.Select(vehicle => vehicle.ToModel()).ToList(),
                SoftDeletedDate = driver.SoftDeletedDate,
                IsDeleted = driver.IsDeleted
            };
        }
    }
}