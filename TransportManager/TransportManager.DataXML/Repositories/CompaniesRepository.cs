using System.Linq;
using TransportManager.Domain;
using TransportManager.Entities;
using TransportManager.Mappers;
using TransportManager.DataXML.Repositories.Abstract;

namespace TransportManager.DataXML.Repositories
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