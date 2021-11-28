using TransportManager.Domain;
using TransportManager.Entities;

namespace TransportManager.DataXML.Repositories.Abstract
{
    public interface ICompaniesRepository : IBaseRepository<CompanyEntity>
    {
        CompanyEntity AddOrUpdate(Company company);
        CompanyEntity GetCompanyByCompanyId(int companyId);
    }
}