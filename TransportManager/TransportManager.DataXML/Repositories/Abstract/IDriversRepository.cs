using System.Collections.Generic;
using TransportManager.Domain;
using TransportManager.Entities;

namespace TransportManager.DataXML.Repositories.Abstract
{
    public interface IDriversRepository : IBaseRepository<DriverEntity>
    {
        DriverEntity AddOrUpdate(Driver driver);
        List<DriverEntity> GetManyDriversByCompanyId(int companyId);
    }
}