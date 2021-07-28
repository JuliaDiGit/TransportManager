using Domain;
using Entities;
using Models;

namespace Mappers
{
    public static class VehicleMapper
    {
        public static VehicleEntity ToEntity(this Vehicle vehicle)
        {
            if (vehicle == null) return null;
            
            return new VehicleEntity
            {
                Id = vehicle.Id,
                CreatedDate = vehicle.CreatedDate,
                CompanyId = vehicle.CompanyId,
                DriverId = vehicle.DriverId,
                Model = vehicle.Model,
                GovernmentNumber = vehicle.GovernmentNumber,
                IsDeleted = vehicle.IsDeleted,
                SoftDeletedDate = vehicle.SoftDeletedDate
            };
        }

        public static Vehicle ToDomain(this VehicleModel vehicleModel)
        {
            if (vehicleModel == null) return null;
            
            return new Vehicle
            {
                Id = vehicleModel.Id,
                CreatedDate = vehicleModel.CreatedDate,
                CompanyId = vehicleModel.CompanyId,
                DriverId = vehicleModel.DriverId,
                Model = vehicleModel.Model,
                GovernmentNumber = vehicleModel.GovernmentNumber,
                IsDeleted = vehicleModel.IsDeleted,
                SoftDeletedDate = vehicleModel.SoftDeletedDate
            };
        }
        
        public static Vehicle ToDomain(this VehicleEntity vehicleEntity)
        {
            if (vehicleEntity == null) return null;
            
            return new Vehicle
            {
                Id = vehicleEntity.Id,
                CreatedDate = vehicleEntity.CreatedDate,
                CompanyId = vehicleEntity.CompanyId,
                DriverId = vehicleEntity.DriverId,
                Model = vehicleEntity.Model,
                GovernmentNumber = vehicleEntity.GovernmentNumber,
                IsDeleted = vehicleEntity.IsDeleted,
                SoftDeletedDate = vehicleEntity.SoftDeletedDate
            };
        }

        public static VehicleModel ToModel(this Vehicle vehicle)
        {
            if (vehicle == null) return null;
            
            return new VehicleModel()
            {
                Id = vehicle.Id,
                CreatedDate = vehicle.CreatedDate,
                CompanyId = vehicle.CompanyId,
                DriverId = vehicle.DriverId,
                Model = vehicle.Model,
                GovernmentNumber = vehicle.GovernmentNumber,
                IsDeleted = vehicle.IsDeleted,
                SoftDeletedDate = vehicle.SoftDeletedDate
            };
        }
    }
}