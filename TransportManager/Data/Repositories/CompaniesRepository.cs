using System.Linq;
using Data.Repositories.Abstract;
using Domain;
using Entities;
using Mappers;

namespace Data.Repositories
{
    public class CompaniesRepository : BaseRepository<CompanyEntity>, ICompaniesRepository
    {
        public CompaniesRepository(string path) : base(path)
        {
        }

        public CompanyEntity AddOrUpdate(Company company)
        {
            return base.AddOrUpdate(company.ToEntity());
        }

        public CompanyEntity GetCompanyByCompanyId(int companyId)
        {
            return GetAll().FirstOrDefault(companyEntity => companyEntity.CompanyId == companyId);
        }
    }
}