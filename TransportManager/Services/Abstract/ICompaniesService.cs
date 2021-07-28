using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Events;
using Models;

namespace Services.Abstract
{
    public interface ICompaniesService
    {
        Task<Company> AddOrUpdateCompanyAsync(CompanyModel companyModel);
        Task<Company> DeleteCompanyByCompanyIdAsync(int companyId);
        Task<Company> GetCompanyByCompanyIdAsync(int companyId);
        Task<List<Company>> GetAllCompaniesAsync();
        Task<Company> RemoveCompanyByCompanyIdAsync(int companyId);
        event EventHandler<SendEventArgs> SendingEvent;
    }
}