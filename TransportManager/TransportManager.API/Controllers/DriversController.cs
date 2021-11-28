using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportManager.Events;
using TransportManager.Common.Enums;
using TransportManager.Common.Exceptions;
using TransportManager.Mappers;
using TransportManager.Models;
using TransportManager.Services.Abstract;
using TransportManager.Models.Validation;

namespace TransportManager.API.Controllers
{
    public class DriversController
    {
        private readonly UserModel _user;
        private readonly IDriversService _driversService;

        public DriversController(UserModel user, IDriversService driversService)
        {
            _user = user;
            _driversService = driversService;
            _driversService.SendingEvent += new MessageHandlerBox().OnSendingEventShowNotification;
        }
        
        public async Task<DriverModel> AddOrUpdateDriverAsync(DriverModel driverModel)
        {
            if (driverModel == null) throw new NullReferenceException();
            if (_user.Role != Role.Admin) throw new AccessException();

            ValidationFilter.Validate(driverModel);
            
            return (await _driversService.AddOrUpdateDriverAsync(driverModel)).ToModel();
        }

        public async Task<DriverModel> DeleteDriverByIdAsync(int id)
        {
            if (id == default) return null;
            
            if (_user.Role != Role.Admin) throw new AccessException();

            return (await _driversService.DeleteDriverByIdAsync(id)).ToModel();
        }

        public async Task<DriverModel> GetDriverByIdAsync(int id)
        {
            if (id == default) return null;
            
            return (await _driversService.GetDriverByIdAsync(id)).ToModel();
        }

        public async Task<List<DriverModel>> GetAllDriversAsync()
        {
            return (await _driversService.GetAllDriversAsync())
                                         .Select(driver => driver.ToModel())
                                         .ToList();
        }

        public async Task<DriverModel> RemoveDriverByIdAsync(int id)
        {
            if (id == default) return null;

            if (_user.Role != Role.Admin) throw new AccessException();

            return (await _driversService.RemoveDriverByIdAsync(id)).ToModel();
        }
    }
}