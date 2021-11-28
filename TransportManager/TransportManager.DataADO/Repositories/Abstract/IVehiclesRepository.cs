using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportManager.Domain;
using TransportManager.Entities;

namespace TransportManager.DataADO.Repositories.Abstract
{
    public interface IVehiclesRepository
    {
        Task<VehicleEntity> AddOrUpdateVehicleAsync(Vehicle vehicle);
        Task<VehicleEntity> DeleteVehicleAsync(int id);
        Task<VehicleEntity> GetVehicleAsync(int id);
        Task<List<VehicleEntity>> GetAllVehiclesAsync();
    }
}
