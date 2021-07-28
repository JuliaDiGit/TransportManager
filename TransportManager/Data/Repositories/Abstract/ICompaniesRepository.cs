using Domain;
using Entities;

namespace Data.Repositories.Abstract
{
    public interface ICompaniesRepository : IBaseRepository<CompanyEntity>
    {
        CompanyEntity AddOrUpdate(Company company);
        CompanyEntity GetCompanyByCompanyId(int companyId);
    }
}