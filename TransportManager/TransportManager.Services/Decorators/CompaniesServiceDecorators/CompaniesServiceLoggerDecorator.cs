using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportManager.Domain;
using TransportManager.Events;
using TransportManager.Logger.Abstract;
using TransportManager.Models;
using TransportManager.Services.Abstract;
using TransportManager.Services.Properties;

namespace TransportManager.Services.Decorators.CompaniesServiceDecorators
{
    public class CompaniesServiceLoggerDecorator : ICompaniesService
    {
        private readonly UserModel _user;
        private readonly ILogger _logger;
        private readonly ICompaniesService _inner;
        private const string CompanyId = "CompanyId";

        public CompaniesServiceLoggerDecorator(UserModel user, 
                                               ILogger logger, 
                                               ICompaniesService inner)
        {
            _user = user;
            _logger = logger;
            _inner = inner;
        }

        public async Task<Company> AddOrUpdateCompanyAsync(CompanyModel companyModel)
        {
            try
            {
                Company company = await _inner.AddOrUpdateCompanyAsync(companyModel);

                string status = company == null 
                                ? Resources.Status_Fail 
                                : Resources.Status_Success;

                _logger.Trace($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{Resources.Operation_AddOrUpdateCompany} - " +
                              $"{status} - " +
                              $"{CompanyId} {companyModel.CompanyId}");

                return company;
            }
            catch (Exception e)
            {
                _logger.Error($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{e.GetType()}");
                throw;
            }
        }

        public async Task<Company> DeleteCompanyByCompanyIdAsync(int companyId)
        {
            try
            {
                Company company = await _inner.DeleteCompanyByCompanyIdAsync(companyId);

                string status = company == null 
                                ? Resources.Status_Fail 
                                : Resources.Status_Success;

                _logger.Trace($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{Resources.Operation_DeleteCompanyByCompanyId} - " +
                              $"{status} - " +
                              $"{CompanyId} {companyId}");

                return company;
            }
            catch (Exception e)
            {
                _logger.Error($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{e.GetType()}");
                throw;
            }
        }

        public async Task<Company> GetCompanyByCompanyIdAsync(int companyId)
        {
            try
            {
                Company company = await _inner.GetCompanyByCompanyIdAsync(companyId);

                string status = company == null
                                ? Resources.Status_NotFound 
                                : Resources.Status_Success;

                _logger.Trace($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{Resources.Operation_GetCompanyByCompanyId} - " +
                              $"{status} - " +
                              $"{CompanyId} {companyId}");

                return company;
            }
            catch (Exception e)
            {
                _logger.Error($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{e.GetType()}");
                throw;
            }
        }

        public async Task<List<Company>> GetAllCompaniesAsync()
        {
            try
            {
                List<Company> companies = await _inner.GetAllCompaniesAsync();

                _logger.Trace($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{Resources.Operation_GetAllCompanies} - " +
                              $"{Resources.Status_Success}");

                return companies;
            }
            catch (Exception e)
            {
                _logger.Error($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{e.GetType()}");
                throw;
            }
        }

        public async Task<Company> RemoveCompanyByCompanyIdAsync(int companyId)
        {
            try
            {
                Company company = await _inner.RemoveCompanyByCompanyIdAsync(companyId);

                string status = company == null
                                ? Resources.Status_Fail
                                : Resources.Status_Success;

                _logger.Trace($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{Resources.Operation_RemoveCompanyByCompanyId} - " +
                              $"{status} - " +
                              $"{CompanyId} {companyId}");

                return company;
            }
            catch (Exception e)
            {
                _logger.Error($"{_user.Login} - " +
                              $"{DateTime.Now} - " +
                              $"{e.GetType()}");
                throw;
            }
        }

        public event EventHandler<SendEventArgs> SendingEvent;
    }
}