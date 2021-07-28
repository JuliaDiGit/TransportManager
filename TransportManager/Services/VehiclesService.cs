using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataEF.Repositories.Abstract;
using Domain;
using Events;
using Services.Abstract;
using Mappers;
using Models;

namespace Services
{
    public class VehiclesService : IVehiclesService
    {
        private readonly IVehiclesRepository _vehiclesRepository;

        public VehiclesService(IVehiclesRepository vehiclesRepository)
        {
            _vehiclesRepository = vehiclesRepository;
        }

        public async Task<Vehicle> AddOrUpdateVehicleAsync(VehicleModel vehicleModel)
        {
            if (vehicleModel == null) throw new NullReferenceException();
            
            Vehicle vehicle = vehicleModel.ToDomain();

            return (await _vehiclesRepository.AddOrUpdateVehicleAsync(vehicle))
                                             .ToDomain();
        }

        public async Task<Vehicle> DeleteVehicleByIdAsync(int id)
        {
            if (id == default) return null;
            
            return (await _vehiclesRepository.DeleteVehicleAsync(id))
                                             .ToDomain();
        }
            
        public async Task<Vehicle> GetVehicleByIdAsync(int id)
        {
            if (id == default) return null;
            
            return (await _vehiclesRepository.GetVehicleAsync(id))
                                             .ToDomain();
        }

        public async Task<List<Vehicle>> GetAllVehiclesAsync()
        { 
            return (await _vehiclesRepository.GetAllVehiclesAsync())
                                             .Select(entity => entity.ToDomain())
                                             .ToList();
        }

        public async Task<Vehicle> RemoveVehicleByIdAsync(int id)
        {
            if (id == default) return null;

            return (await _vehiclesRepository.RemoveVehicleAsync(id))
                                             .ToDomain();
        }

        public event EventHandler<SendEventArgs> SendingEvent;
    }
}