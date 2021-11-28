using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportManager.Domain;
using TransportManager.Events;
using TransportManager.Models;
using TransportManager.Services.Abstract;
using TransportManager.Services.Properties;

namespace TransportManager.Services.Decorators.CompaniesServiceDecorators
{
    public class CompaniesServiceEventDecorator : ICompaniesService
    {
        private readonly ICompaniesService _companiesService;

        public CompaniesServiceEventDecorator(ICompaniesService companiesService)
        {
            _companiesService = companiesService;
        }

        public async Task<Company> AddOrUpdateCompanyAsync(CompanyModel companyModel)
        {
            try
            {
                Company company = await _companiesService.AddOrUpdateCompanyAsync(companyModel);

                string status = company == null 
                                ? Resources.Status_Fail 
                                : Resources.Status_Success;

                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_AddOrUpdateCompany,
                                                             status, 
                                                             companyModel.CompanyId));

                return company;
            }
            catch (Exception e)
            {
                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_AddOrUpdateCompany,
                                                             e.GetType().ToString(), 
                                                             companyModel.CompanyId));
                throw;
            }
        }

        public async Task<Company> DeleteCompanyByCompanyIdAsync(int companyId)
        {
            try
            {
                Company company = await _companiesService.DeleteCompanyByCompanyIdAsync(companyId);

                string status = company == null 
                                ? Resources.Status_Fail 
                                : Resources.Status_Success;

                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_DeleteCompanyByCompanyId,
                                                             status, 
                                                             companyId));

                return company;
            }
            catch (Exception e)
            {
                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_DeleteCompanyByCompanyId,
                                                             e.GetType().ToString(), 
                                                             companyId));
                throw;
            }
        }

        public async Task<Company> GetCompanyByCompanyIdAsync(int companyId)
        {
            try
            {
                Company company = await _companiesService.GetCompanyByCompanyIdAsync(companyId);

                string status = company == null
                                ? Resources.Status_NotFound
                                : Resources.Status_Success;

                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_GetCompanyByCompanyId,
                                                             status, 
                                                             companyId));

                return company;
            }
            catch (Exception e)
            {
                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_GetCompanyByCompanyId,
                                                             e.GetType().ToString(),
                                                             companyId));
                throw;
            }
        }

        public async Task<List<Company>> GetAllCompaniesAsync()
        {
            try
            {
                return await _companiesService.GetAllCompaniesAsync();
            }
            catch (Exception e)
            {
                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_GetAllCompanies, e.GetType().ToString()));
                throw;
            }
        }

        public async Task<Company> RemoveCompanyByCompanyIdAsync(int companyId)
        {
            try
            {
                Company company = await _companiesService.RemoveCompanyByCompanyIdAsync(companyId);

                string status = company == null
                                ? Resources.Status_Fail
                                : Resources.Status_Success;

                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_RemoveCompanyByCompanyId,
                                                             status, 
                                                             companyId));

                return company;
            }
            catch (Exception e)
            {
                SendingEvent?.Invoke(this, new SendEventArgs(Resources.Operation_RemoveCompanyByCompanyId,
                                                             e.GetType().ToString(), 
                                                             companyId));
                throw;
            }
        }

        public event EventHandler<SendEventArgs> SendingEvent;
    }
}