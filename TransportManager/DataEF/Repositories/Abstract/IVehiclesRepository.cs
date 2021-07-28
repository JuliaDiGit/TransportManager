﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Entities;

namespace DataEF.Repositories.Abstract
{
    public interface IVehiclesRepository
    {
        Task<VehicleEntity> AddOrUpdateVehicleAsync(Vehicle vehicle);
        Task<VehicleEntity> DeleteVehicleAsync(int id);
        Task<VehicleEntity> GetVehicleAsync(int id);
        Task<List<VehicleEntity>> GetAllVehiclesAsync();
        Task<VehicleEntity> RemoveVehicleAsync(int id);
    }
}