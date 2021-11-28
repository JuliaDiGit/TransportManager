using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportManager.DataEF.Repositories.Abstract;
using TransportManager.Domain;
using TransportManager.Events;
using TransportManager.Mappers;
using TransportManager.Models;
using TransportManager.Services.Abstract;

namespace TransportManager.Services
{
    public class DriversService : IDriversService
    {
        private readonly IDriversRepository _driversRepository;

        public DriversService(IDriversRepository driversRepository)
        {
            _driversRepository = driversRepository;
        }

        public async Task<Driver> AddOrUpdateDriverAsync(DriverModel driverModel)
        {
            if (driverModel == null) throw new NullReferenceException();
            
            Driver driver = driverModel.ToDomain();

            if (driver == null) return null;
            
            return (await _driversRepository.AddOrUpdateDriverAsync(driver))
                                            .ToDomain();
        }

        public async Task<Driver> DeleteDriverByIdAsync(int id)
        {
            if (id == default) return null;
            
            return (await _driversRepository.DeleteDriverAsync(id))
                                            .ToDomain();
        }

        public async Task<Driver> GetDriverByIdAsync(int id)
        {
            if (id == default) return null;
            
            return (await _driversRepository.GetDriverAsync(id))
                                            .ToDomain();
        }

        public async Task<List<Driver>> GetAllDriversAsync()
        {
            return (await _driversRepository.GetAllDriversAsync())?
                                            .Select(driverEntity => driverEntity.ToDomain())
                                            .ToList();
        }

        public async Task<Driver> RemoveDriverByIdAsync(int id)
        {
            if (id == default) return null;

            return (await _driversRepository.RemoveDriverAsync(id))
                                            .ToDomain();
        }

        public event EventHandler<SendEventArgs> SendingEvent;
    }
}