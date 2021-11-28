using System.Linq;
using TransportManager.Domain;
using TransportManager.Entities;
using TransportManager.Models;

namespace TransportManager.Mappers
{
    public static class CompanyMapper
    {
        public static CompanyEntity ToEntity(this Company company)
        {
            if (company == null) return null;
            
            return new CompanyEntity
            {
                Id = company.Id,
                CreatedDate = company.CreatedDate,
                SoftDeletedDate = company.SoftDeletedDate,
                IsDeleted = company.IsDeleted,
                CompanyId = company.CompanyId,
                CompanyName = company.CompanyName,
                Drivers = company.Drivers?.Select(driver => driver.ToEntity()).ToList(),
                Vehicles = company.Vehicles?.Select(vehicle => vehicle.ToEntity()).ToList()
            };
        }
        
        public static Company ToDomain(this CompanyModel companyModel)
        {
            if (companyModel == null) return null;
            
            return new Company
            {
                Id = companyModel.Id,
                CreatedDate = companyModel.CreatedDate,
                SoftDeletedDate = companyModel.SoftDeletedDate,
                IsDeleted = companyModel.IsDeleted,
                CompanyId = companyModel.CompanyId,
                CompanyName = companyModel.CompanyName,
                Drivers = companyModel.Drivers?.Select(driver => driver.ToDomain()).ToList(),
                Vehicles = companyModel.Vehicles?.Select(vehicle => vehicle.ToDomain()).ToList()
            };
        }
        
        public static Company ToDomain(this CompanyEntity companyEntity)
        {
            if (companyEntity == null) return null;
            
            return new Company
            {
                Id = companyEntity.Id,
                CreatedDate = companyEntity.CreatedDate,
                SoftDeletedDate = companyEntity.SoftDeletedDate,
                IsDeleted = companyEntity.IsDeleted,
                CompanyId = companyEntity.CompanyId,
                CompanyName = companyEntity.CompanyName,
                Drivers = companyEntity.Drivers?.Select(driver => driver.ToDomain()).ToList(),
                Vehicles = companyEntity.Vehicles?.Select(vehicle => vehicle.ToDomain()).ToList()
            };
        }
        
        public static CompanyModel ToModel(this Company company)
        {
            if (company == null) return null;
            
            return new CompanyModel()
            {
                Id = company.Id,
                CreatedDate = company.CreatedDate,
                SoftDeletedDate = company.SoftDeletedDate,
                IsDeleted = company.IsDeleted,
                CompanyId = company.CompanyId,
                CompanyName = company.CompanyName,
                Drivers = company.Drivers?.Select(driver => driver.ToModel()).ToList(),
                Vehicles = company.Vehicles?.Select(vehicle => vehicle.ToModel()).ToList()
            };
        }
    }
}