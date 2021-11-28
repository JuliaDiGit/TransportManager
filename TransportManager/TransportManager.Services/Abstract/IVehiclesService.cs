using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportManager.Domain;
using TransportManager.Events;
using TransportManager.Models;

namespace TransportManager.Services.Abstract
{
    public interface IVehiclesService
    {
        Task<Vehicle> AddOrUpdateVehicleAsync(VehicleModel vehicleModel);
        Task<Vehicle> DeleteVehicleByIdAsync(int id);
        Task<Vehicle> GetVehicleByIdAsync(int id);
        Task<List<Vehicle>> GetAllVehiclesAsync();
        Task<Vehicle> RemoveVehicleByIdAsync(int id);


        event EventHandler<SendEventArgs> SendingEvent;
    }
}