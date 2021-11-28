using System.Collections.Generic;
using System.Linq;
using TransportManager.Domain;
using TransportManager.Entities;
using TransportManager.Mappers;
using TransportManager.DataXML.Repositories.Abstract;

namespace TransportManager.DataXML.Repositories
{
    public class DriversRepository : BaseRepository<DriverEntity>, IDriversRepository
    {
        public DriversRepository(string path) : base(path)
        {
        }

        public DriverEntity AddOrUpdate(Driver driver)
        {
            return base.AddOrUpdate(driver.ToEntity());
        }

        public List<DriverEntity> GetManyDriversByCompanyId(int companyId)
        {
            return GetAll().Where(driver => driver.CompanyId == companyId)
                           .ToList();
        }
    }
}