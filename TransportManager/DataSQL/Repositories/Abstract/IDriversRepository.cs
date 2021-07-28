using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Entities;

namespace DataSQL.Repositories.Abstract
{
    public interface IDriversRepository
    {
        Task<DriverEntity> AddOrUpdateDriverAsync(Driver driver);
        Task<DriverEntity> DeleteDriverAsync(int id);
        Task<DriverEntity> GetDriverAsync(int id);
        Task<List<DriverEntity>> GetAllDriversAsync();
    }
}
