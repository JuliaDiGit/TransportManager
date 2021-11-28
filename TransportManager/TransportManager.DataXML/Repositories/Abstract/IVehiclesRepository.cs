using System.Collections.Generic;
using TransportManager.Domain;
using TransportManager.Entities;

namespace TransportManager.DataXML.Repositories.Abstract
{
    public interface IVehiclesRepository : IBaseRepository<VehicleEntity>
    { 
        VehicleEntity AddOrUpdate(Vehicle vehicle);
        List<VehicleEntity> GetManyVehiclesByDriverId(int driverId);
        List<VehicleEntity> GetManyVehiclesByCompanyId(int companyId);
    }
}