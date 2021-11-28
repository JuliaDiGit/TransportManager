using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportManager.DataEF.Repositories.Abstract;
using TransportManager.Domain;
using TransportManager.Events;
using TransportManager.Mappers;
using TransportManager.Models;
using TransportManager.Services.Abstract;

namespace TransportManager.Services
{
    public class CompaniesService : ICompaniesService
    {
        private readonly ICompaniesRepository _companiesRepository;
        
        public CompaniesService(ICompaniesRepository companiesRepository)
        {
            _companiesRepository = companiesRepository;
        }

        public async Task<Company> AddOrUpdateCompanyAsync(CompanyModel companyModel)
        {
            if (companyModel == null) throw new NullReferenceException();
            
            Company company = companyModel.ToDomain();

            if (company == null) return null;

            return (await _companiesRepository.AddOrUpdateCompanyAsync(company))
                                              .ToDomain();

        }

        public async Task<Company> DeleteCompanyByCompanyIdAsync(int companyId)
        {
            if (companyId == default) return null;
            
            return (await _companiesRepository.DeleteCompanyAsync(companyId))
                                              .ToDomain();
        }

        public async Task<Company> GetCompanyByCompanyIdAsync(int companyId)
        {
            if (companyId == default) return null;
            
            return (await _companiesRepository.GetCompanyAsync(companyId))
                                              .ToDomain();
        }
        
        public async Task<List<Company>> GetAllCompaniesAsync()
        {
            return (await _companiesRepository.GetAllCompaniesAsync())?
                                              .Select(companyEntity => companyEntity.ToDomain())
                                              .ToList();
        }

        public async Task<Company> RemoveCompanyByCompanyIdAsync(int companyId)
        {
            if (companyId == default) return null;

            return (await _companiesRepository.RemoveCompanyAsync(companyId))
                                              .ToDomain();
        }

        public event EventHandler<SendEventArgs> SendingEvent;
    }
}