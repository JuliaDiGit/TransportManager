using System.Collections.Generic;
using System.Threading.Tasks;
using TransportManager.Domain;
using TransportManager.Entities;

namespace TransportManager.DataEF.Repositories.Abstract
{
    public interface IDriversRepository
    {
        Task<DriverEntity> AddOrUpdateDriverAsync(Driver driver);
        Task<DriverEntity> DeleteDriverAsync(int id);
        Task<DriverEntity> GetDriverAsync(int id);
        Task<List<DriverEntity>> GetAllDriversAsync();
        Task<DriverEntity> RemoveDriverAsync(int id);
    }
}