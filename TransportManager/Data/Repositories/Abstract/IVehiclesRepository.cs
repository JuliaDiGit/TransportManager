using System.Collections.Generic;
using Domain;
using Entities;

namespace Data.Repositories.Abstract
{
    public interface IVehiclesRepository : IBaseRepository<VehicleEntity>
    { 
        VehicleEntity AddOrUpdate(Vehicle vehicle);
        List<VehicleEntity> GetManyVehiclesByDriverId(int driverId);
        List<VehicleEntity> GetManyVehiclesByCompanyId(int companyId);
    }
}