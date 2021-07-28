using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Events;
using Models;


namespace Services.Abstract
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