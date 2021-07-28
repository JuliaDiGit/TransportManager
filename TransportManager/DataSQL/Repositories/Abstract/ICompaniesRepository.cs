using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Entities;

namespace DataSQL.Repositories.Abstract
{
    public interface ICompaniesRepository
    {
        Task<CompanyEntity> AddOrUpdateCompanyAsync(Company company);
        Task<CompanyEntity> DeleteCompanyAsync(int id);
        Task<CompanyEntity> GetCompanyAsync(int id);
        Task<List<CompanyEntity>> GetAllCompaniesAsync();
        Task<CompanyEntity> GetCompanyByCompanyIdAsync(int companyId);


    }
}
