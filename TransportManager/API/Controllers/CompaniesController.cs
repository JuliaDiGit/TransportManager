using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enums;
using Events;
using Exceptions;
using Mappers;
using Models;
using Services.Abstract;
using Models.Validation;

namespace API.Controllers
{
    public class CompaniesController
    {
        private readonly UserModel _user;
        private readonly ICompaniesService _companiesService;

        public CompaniesController(UserModel user, ICompaniesService companiesService)
        {
            _user = user;
            _companiesService = companiesService;
            _companiesService.SendingEvent += new MessageHandlerBox().OnSendingEventShowNotification;
        }

        public async Task<CompanyModel> AddOrUpdateCompanyAsync(CompanyModel companyModel)
        {
            if (companyModel == null) throw new NullReferenceException();
            
            if (_user.Role != Role.Admin) throw new AccessException();

            ValidationFilter.Validate(companyModel);
            
            return (await _companiesService.AddOrUpdateCompanyAsync(companyModel))
                                           .ToModel();
        }

        public async Task<CompanyModel> DeleteCompanyByCompanyIdAsync(int companyId)
        {
            if (companyId == default) return null;
            
            if (_user.Role != Role.Admin) throw new AccessException();

            return (await _companiesService.DeleteCompanyByCompanyIdAsync(companyId))
                                           .ToModel();
        }

        public async Task<CompanyModel> GetCompanyByCompanyIdAsync(int companyId)
        {
            if (companyId == default) return null;
            
            return (await _companiesService.GetCompanyByCompanyIdAsync(companyId))
                                           .ToModel();
        }

        public async Task<List<CompanyModel>> GetAllCompaniesAsync()
        {
            return (await _companiesService.GetAllCompaniesAsync())
                                           .Select(company => company.ToModel())
                                           .ToList();
        }

        public async Task<CompanyModel> RemoveCompanyByCompanyIdAsync(int companyId)
        {
            if (companyId == default) return null;

            if (_user.Role != Role.Admin) throw new AccessException();

            return (await _companiesService.RemoveCompanyByCompanyIdAsync(companyId))
                                           .ToModel();
        }

    }
}