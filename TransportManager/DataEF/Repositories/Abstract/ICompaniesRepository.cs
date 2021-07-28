using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Entities;

namespace DataEF.Repositories.Abstract
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