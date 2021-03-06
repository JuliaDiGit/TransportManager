using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportManager.Domain;
using TransportManager.Events;
using TransportManager.Models;


namespace TransportManager.Services.Abstract
{
    public interface IDriversService
    {
        Task<Driver> AddOrUpdateDriverAsync(DriverModel driverModel);
        Task<Driver> DeleteDriverByIdAsync(int id);
        Task<Driver> GetDriverByIdAsync(int id);
        Task<List<Driver>> GetAllDriversAsync();
        Task<Driver> RemoveDriverByIdAsync(int id);


        event EventHandler<SendEventArgs> SendingEvent;
    }
}