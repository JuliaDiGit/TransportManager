using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Events;
using Models;
using Services.Abstract;
using Services.Properties;

namespace Services.Decorators.VehiclesServiceDecorators
{
    public class VehiclesServiceEventDecorator : IVehiclesService
    {
        private readonly IVehiclesService _vehiclesService;
        
        public VehiclesServiceEventDecorator(IVehiclesService vehiclesService)
        {
            _vehiclesService = vehiclesService;
        }

        public async Task<Vehicle> AddOrUpdateVehicleAsync(VehicleModel vehicleModel)
        {
            try
            {
                Vehicle vehicle = await _vehiclesService.AddOrUpdateVehicleAsync(vehicleModel);

                string status = vehicle == null 
                                ? Resources.Status_Fail 
                                : Resources.Status_Success;

                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_AddOrUpdateVehicle,
                                                             status, 
                                                             vehicleModel.Id));

                return vehicle;
            }
            catch (Exception e)
            {
                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_AddOrUpdateVehicle,
                                                             e.GetType().ToString(),
                                                             vehicleModel.Id));
                throw;
            }
        }

        public async Task<Vehicle> DeleteVehicleByIdAsync(int id)
        {
            try
            {
                Vehicle vehicle = await _vehiclesService.DeleteVehicleByIdAsync(id);

                string status = vehicle == null 
                                ? Resources.Status_Fail 
                                : Resources.Status_Success;

                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_DeleteVehicle,
                                                             status, 
                                                             id));

                return vehicle;
            }
            catch (Exception e)
            {
                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_DeleteVehicle,
                                                             e.GetType().ToString(), 
                                                             id));
                throw;
            }
        }

        public async Task<Vehicle> GetVehicleByIdAsync(int id)
        {
            try
            {
                Vehicle vehicle = await _vehiclesService.GetVehicleByIdAsync(id);

                string status = vehicle == null
                                ? Resources.Status_NotFound
                                : Resources.Status_Success;

                if (vehicle == null)
                {
                    SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_GetVehicle,
                        status,
                        id));
                }

                return vehicle;
            }
            catch (Exception e)
            {
                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_GetVehicle,
                                                             e.GetType().ToString(), 
                                                             id));

                throw;
            }
        }

        public async Task<List<Vehicle>> GetAllVehiclesAsync()
        {
            try
            {
                return await _vehiclesService.GetAllVehiclesAsync();
            }
            catch (Exception e)
            {
                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_GetVehicle, e.GetType().ToString()));

                throw;
            }
        }

        public async Task<Vehicle> RemoveVehicleByIdAsync(int id)
        {
            try
            {
                Vehicle vehicle = await _vehiclesService.RemoveVehicleByIdAsync(id);

                string status = vehicle == null
                                ? Resources.Status_Fail
                                : Resources.Status_Success;

                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_RemoveVehicle,
                                                             status, 
                                                             id));

                return vehicle;
            }
            catch (Exception e)
            {
                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_RemoveVehicle,
                                                             e.GetType().ToString(), 
                                                             id));
                throw;
            }
        }

        public event EventHandler<SendEventArgs> SendingEvent;
    }
}