using System.Collections.Generic;
using System.Threading.Tasks;
using TransportManager.Domain;
using TransportManager.Entities;

namespace TransportManager.DataEF.Repositories.Abstract
{
    public interface ICompaniesRepository
    {
        Task<CompanyEntity> AddOrUpdateCompanyAsync(Company company);
        Task<CompanyEntity> DeleteCompanyAsync(int companyId);
        Task<CompanyEntity> GetCompanyAsync(int companyId);
        Task<List<CompanyEntity>> GetAllCompaniesAsync();
        Task<CompanyEntity> RemoveCompanyAsync(int companyId);
    }
}