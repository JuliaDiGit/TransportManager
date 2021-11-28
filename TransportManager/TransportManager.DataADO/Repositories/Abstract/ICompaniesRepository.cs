using System.Collections.Generic;
using System.Threading.Tasks;
using TransportManager.Domain;
using TransportManager.Entities;

namespace TransportManager.DataADO.Repositories.Abstract
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
