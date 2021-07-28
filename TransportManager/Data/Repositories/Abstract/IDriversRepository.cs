using System.Collections.Generic;
using Domain;
using Entities;

namespace Data.Repositories.Abstract
{
    public interface IDriversRepository : IBaseRepository<DriverEntity>
    {
        DriverEntity AddOrUpdate(Driver driver);
        List<DriverEntity> GetManyDriversByCompanyId(int companyId);
    }
}