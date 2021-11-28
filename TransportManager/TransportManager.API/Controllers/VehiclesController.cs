using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportManager.Events;
using TransportManager.Common.Enums;
using TransportManager.Common.Exceptions;
using TransportManager.Mappers;
using TransportManager.Models;
using TransportManager.Models.Validation;
using TransportManager.Services.Abstract;

namespace TransportManager.API.Controllers
{
    public class VehiclesController
    {
        private readonly UserModel _user;
        private readonly IVehiclesService _vehiclesService;

        public VehiclesController(UserModel user, IVehiclesService vehiclesService)
        {
            _user = user;
            _vehiclesService = vehiclesService;
            _vehiclesService.SendingEvent += new MessageHandlerBox().OnSendingEventShowNotification;
        }
        
        public async Task<VehicleModel> AddOrUpdateVehicleAsync(VehicleModel vehicleModel)
        {
            if (vehicleModel == null) throw new NullReferenceException();
            if (_user.Role != Role.Admin) throw new AccessException();

            ValidationFilter.Validate(vehicleModel);
            
            return (await _vehiclesService.AddOrUpdateVehicleAsync(vehicleModel)).ToModel();
        }

        public async Task<VehicleModel> GetVehicleByIdAsync(int id)
        {
            if (id == default) return null;
            
            return (await _vehiclesService.GetVehicleByIdAsync(id)).ToModel();
        }

        public async Task<VehicleModel> DeleteVehicleByIdAsync(int id)
        {
            if (id == default) return null;
            if (_user.Role != Role.Admin) throw new AccessException();

            return (await _vehiclesService.DeleteVehicleByIdAsync(id)).ToModel();
        }
        
        public async Task<List<VehicleModel>> GetAllVehiclesAsync()
        {
            return (await _vehiclesService.GetAllVehiclesAsync())
                                          .Select(vehicle => vehicle.ToModel())
                                          .ToList();
        }

        public async Task<VehicleModel> RemoveVehicleByIdAsync(int id)
        {
            if (id == default) return null;
            if (_user.Role != Role.Admin) throw new AccessException();

            return (await _vehiclesService.RemoveVehicleByIdAsync(id)).ToModel();
        }
    }
}