using System.Collections.Generic;
using System.Linq;
using TransportManager.Domain;
using TransportManager.Entities;
using TransportManager.Mappers;
using TransportManager.DataXML.Repositories.Abstract;

namespace TransportManager.DataXML.Repositories
{
    public class VehiclesRepository : BaseRepository<VehicleEntity>, IVehiclesRepository
    {
        public VehiclesRepository(string path) : base(path)
        {
        }
        
        public VehicleEntity AddOrUpdate(Vehicle vehicle)
        {
            return base.AddOrUpdate(vehicle.ToEntity());
        }
        
        public List<VehicleEntity> GetManyVehiclesByDriverId(int driverId)
        {
            return GetAll().Where(vehicle => vehicle.DriverId == driverId)
                           .ToList();
        }

        public List<VehicleEntity> GetManyVehiclesByCompanyId(int companyId)
        {
            return GetAll().Where(vehicle => vehicle.CompanyId == companyId)
                           .ToList();
        }
    }
}