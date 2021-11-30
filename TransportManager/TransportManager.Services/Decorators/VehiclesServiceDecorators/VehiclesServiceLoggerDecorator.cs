using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportManager.Domain;
using TransportManager.Events;
using TransportManager.Loggers.Abstract;
using TransportManager.Models;
using TransportManager.Services.Abstract;
using TransportManager.Services.Properties;

namespace TransportManager.Services.Decorators.VehiclesServiceDecorators
{
    public class VehiclesServiceLoggerDecorator : IVehiclesService
    {
        private readonly UserModel _user;
        private readonly ILogger _logger;
        private readonly IVehiclesService _inner;
        private const string Id = "Id";

        public VehiclesServiceLoggerDecorator(UserModel user, 
                                              ILogger logger, 
                                              IVehiclesService inner)
        {
            _user = user;
            _logger = logger;
            _inner = inner;
        }

        public async Task<Vehicle> AddOrUpdateVehicleAsync(VehicleModel vehicleModel)
        {
            try
            {
                Vehicle vehicle2 = await _inner.AddOrUpdateVehicleAsync(vehicleModel);

                string status = vehicle2 == null 
                                ? Resources.Status_Fail 
                                : Resources.Status_Success;

                _logger.Trace($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{Resources.Operation_AddOrUpdateVehicle} - " +
                              $"{status} - " +
                              $"{Id} {vehicleModel.Id}");

                return vehicle2;
            }
            catch (Exception e)
            {
                _logger.Error($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{e.GetType()}");
                throw;
            }
        }

        public async Task<Vehicle> DeleteVehicleByIdAsync(int id)
        {
            try
            {
                Vehicle vehicle = await _inner.DeleteVehicleByIdAsync(id);

                string status = vehicle == null 
                                ? Resources.Status_Fail 
                                : Resources.Status_Success;

                _logger.Trace($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{Resources.Operation_DeleteVehicle} - " +
                              $"{status} - {Id} {id}");

                return vehicle;
            }
            catch (Exception e)
            {
                _logger.Error($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{e.GetType()}");
                throw;
            }
        }

        public async Task<Vehicle> GetVehicleByIdAsync(int id)
        {
            try
            {
                Vehicle vehicle = await _inner.GetVehicleByIdAsync(id);

                string status = vehicle == null
                                ? Resources.Status_NotFound 
                                : Resources.Status_Success;

                _logger.Trace($"{_user.Login} - " +
                                  $"{DateTime.Now} - " +
                                  $"{Resources.Operation_GetVehicle} - " +
                                  $"{status} - " +
                                  $"{Id} {id}");
                
                return vehicle;
            }
            catch (Exception e)
            {
                _logger.Error($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{e.GetType()}");
                throw;
            }
        }

        public async Task<List<Vehicle>> GetAllVehiclesAsync()
        {
            try
            {
                List<Vehicle> vehicles = await _inner.GetAllVehiclesAsync();

                _logger.Trace($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{Resources.Operation_GetAllVehicles} - " +
                              $"{Resources.Status_Success}");

                return vehicles;
            }
            catch (Exception e)
            {
                _logger.Error($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{e.GetType()}");
                throw;
            }
        }

        public async Task<Vehicle> RemoveVehicleByIdAsync(int id)
        {
            try
            {
                Vehicle vehicle = await _inner.RemoveVehicleByIdAsync(id);

                string status = vehicle == null
                                ? Resources.Status_Fail
                                : Resources.Status_Success;

                _logger.Trace($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{Resources.Operation_RemoveVehicle} - " +
                              $"{status} - {Id} {id}");

                return vehicle;
            }
            catch (Exception e)
            {
                _logger.Error($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{e.GetType()}");
                throw;
            }
        }

        public event EventHandler<SendEventArgs> SendingEvent;
    }
}