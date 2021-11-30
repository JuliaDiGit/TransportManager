using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportManager.Domain;
using TransportManager.Events;
using TransportManager.Loggers.Abstract;
using TransportManager.Models;
using TransportManager.Services.Abstract;
using TransportManager.Services.Properties;

namespace TransportManager.Services.Decorators.DriversServiceDecorators
{
    public class DriversServiceLoggerDecorator : IDriversService
    {
        private UserModel _user;
        private readonly ILogger _logger;
        private readonly IDriversService _inner;
        private const string Id = "Id";

        public DriversServiceLoggerDecorator(UserModel user, 
                                             ILogger logger, 
                                             IDriversService inner)
        {
            _user = user;
            _logger = logger;
            _inner = inner;
        }

        public async Task<Driver> AddOrUpdateDriverAsync(DriverModel driverModel)
        {
            try
            {
                Driver driver = await _inner.AddOrUpdateDriverAsync(driverModel);

                string status = driver == null 
                                ? Resources.Status_Fail 
                                : Resources.Status_Success;

                _logger.Trace($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{Resources.Operation_AddOrUpdateDriver} - " +
                              $"{status} - " +
                              $"{Id} {driverModel.Id}");

                return driver;
            }
            catch (Exception e)
            {
                _logger.Error($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{e.GetType()}");
                throw;
            }
        }

        public async Task<Driver> DeleteDriverByIdAsync(int id)
        {
            try
            {
                Driver driver = await _inner.DeleteDriverByIdAsync(id);

                string status = driver == null 
                                ? Resources.Status_Fail 
                                : Resources.Status_Success;

                _logger.Trace($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{Resources.Operation_DeleteDriver} - " +
                              $"{status} - " +
                              $"{Id} {id}");

                return driver;
            }
            catch (Exception e)
            {
                _logger.Error($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{e.GetType()}");
                throw;
            }
        }

        public async Task<Driver> GetDriverByIdAsync(int id)
        {
            try
            {
                Driver driver = await _inner.GetDriverByIdAsync(id);

                string status = driver == null
                                ? Resources.Status_NotFound
                                : Resources.Status_Success;

                _logger.Trace($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{Resources.Operation_GetDriver} - " +
                              $"{status} - " +
                              $"{Id} {id}");

                return driver;
            }
            catch (Exception e)
            {
                _logger.Error($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{e.GetType()}");
                throw;
            }
        }

        public async Task<List<Driver>> GetAllDriversAsync()
        {
            try
            {
                List<Driver> drivers = await _inner.GetAllDriversAsync();

                _logger.Trace($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{Resources.Operation_GetAllDrivers} - " +
                              $"{Resources.Status_Success}");

                return drivers;
            }
            catch (Exception e)
            {
                _logger.Error($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{e.GetType()}");
                throw;
            }
        }

        public async Task<Driver> RemoveDriverByIdAsync(int id)
        {
            try
            {
                Driver driver = await _inner.RemoveDriverByIdAsync(id);

                string status = driver == null
                                ? Resources.Status_Fail
                                : Resources.Status_Success;

                _logger.Trace($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{Resources.Operation_RemoveDriver} - " +
                              $"{status} - " +
                              $"{Id} {id}");

                return driver;
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