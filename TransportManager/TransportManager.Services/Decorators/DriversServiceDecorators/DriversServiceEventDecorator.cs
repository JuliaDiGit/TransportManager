using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportManager.Domain;
using TransportManager.Events;
using TransportManager.Models;
using TransportManager.Services.Abstract;
using TransportManager.Services.Properties;

namespace TransportManager.Services.Decorators.DriversServiceDecorators
{
    public class DriversServiceEventDecorator : IDriversService
    {
        private readonly IDriversService _driversService;
        
        public DriversServiceEventDecorator(IDriversService driversService)
        {
            _driversService = driversService;
        }
        
        public async Task<Driver> AddOrUpdateDriverAsync(DriverModel driverModel)
        {
            if (driverModel == null) return null;

            try
            {
                Driver driver = await _driversService.AddOrUpdateDriverAsync(driverModel);

                string status = driver == null 
                                ? Resources.Status_Fail 
                                : Resources.Status_Success;

                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_AddOrUpdateDriver,
                                                             status, 
                                                             driverModel.Id));

                return driver;
            }
            catch (Exception e)
            {
                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_AddOrUpdateDriver, 
                                                             e.GetType().ToString(), 
                                                             driverModel.Id));
                throw;
            }
        }

        public async Task<Driver> DeleteDriverByIdAsync(int id)
        {
            try
            {
                Driver driver = await _driversService.DeleteDriverByIdAsync(id);

                string status = driver == null 
                                ? Resources.Status_Fail 
                                : Resources.Status_Success;

                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_DeleteDriver,
                                                             status, 
                                                             id));

                return driver;
            }
            catch (Exception e)
            {
                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_DeleteDriver, 
                                                             e.GetType().ToString(), 
                                                             id));
                throw;
            }
        }

        public async Task<Driver> GetDriverByIdAsync(int id)
        {
            try
            {
                Driver driver = await _driversService.GetDriverByIdAsync(id);

                string status = driver == null
                                ? Resources.Status_NotFound 
                                : Resources.Status_Success;

                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_GetDriver,
                                                             status, 
                                                             id));

                return driver;
            }
            catch (Exception e)
            {
                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_GetDriver,
                                                             e.GetType().ToString(), 
                                                             id));
                throw;
            }
        }

        public async Task<List<Driver>> GetAllDriversAsync()
        {
            try
            {
                return await _driversService.GetAllDriversAsync();
            }
            catch (Exception e)
            {
                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_GetAllDrivers, e.GetType().ToString()));

                throw;
            }
        }

        public async Task<Driver> RemoveDriverByIdAsync(int id)
        {
            try
            {
                Driver driver = await _driversService.RemoveDriverByIdAsync(id);

                string status = driver == null
                                ? Resources.Status_Fail 
                                : Resources.Status_Success;

                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_RemoveDriver,
                                                             status, 
                                                             id));

                return driver;
            }
            catch (Exception e)
            {
                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_RemoveDriver,
                                                             e.GetType().ToString(), 
                                                             id));
                throw;
            }
        }

        public event EventHandler<SendEventArgs> SendingEvent;
    }
}