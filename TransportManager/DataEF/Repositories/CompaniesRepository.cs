using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DataEF.DbContext;
using DataEF.Repositories.Abstract;
using Domain;
using Entities;
using Exceptions;
using Mappers;

namespace DataEF.Repositories
{
    public class CompaniesRepository : ICompaniesRepository
    {
        public async Task<CompanyEntity> AddOrUpdateCompanyAsync(Company company)
        {
            if (company == null) throw new NullReferenceException();

            try
            {
                var companyEntity = company.ToEntity();

                using (var context = new EfDbContext())
                {
                    var entity = await context.Companies.FindAsync(company.CompanyId);

                    if (entity != null && entity.IsDeleted) return entity;

                    context.Companies.AddOrUpdate(companyEntity);

                    await context.SaveChangesAsync();

                    return companyEntity;
                }
            }
            catch (Exception e)
            {
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
        }

        public async Task<CompanyEntity> DeleteCompanyAsync(int companyId)
        {
            if (companyId == default) return null;

            try
            {
                using (var context = new EfDbContext())
                {
                    var company = await context.Companies.Where(c => c.CompanyId == companyId)
                                                         .Include(c => c.Drivers.Select(driver => driver.Vehicles))
                                                         .Include(c => c.Vehicles)
                                                         .FirstOrDefaultAsync();

                    if (company == null) throw new NullReferenceException(nameof(CompanyEntity));

                    //удаляем каждое ТС каждого водителя Компании
                    company.Drivers.ToList()
                                   .ForEach(d => 
                                   {
                                       d.Vehicles.ToList()
                                                 .ForEach(v => context.Vehicles.Remove(v)); 
                                       
                                       context.Drivers.Remove(d);
                                   });

                    context.Companies.Remove(company);

                    await context.SaveChangesAsync();

                    return company;
                }
            }
            catch (Exception e)
            {
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
        }

        public async Task<CompanyEntity> GetCompanyAsync(int companyId)
        {
            if (companyId == default) return null;

            try
            {
                using (var context = new EfDbContext())
                {
                    return await context.Companies.Where(c => c.CompanyId == companyId)
                                                  .Include(c => c.Drivers.Select(driver => driver.Vehicles))
                                                  .Include(c => c.Vehicles)
                                                  .FirstOrDefaultAsync();
                }
            }
            catch (Exception e)
            {
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
        }

        public async Task<List<CompanyEntity>> GetAllCompaniesAsync()
        {
            try
            {
                using (var context = new EfDbContext())
                {
                    return await context.Companies.Include(c => c.Drivers.Select(driver => driver.Vehicles))
                                                  .Include(c => c.Vehicles)
                                                  .ToListAsync();
                }
            }
            catch (Exception e)
            {
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
        }
        
        public async Task<CompanyEntity> RemoveCompanyAsync(int companyId)
        {
            if (companyId == default) return null;

            try
            {
                using (var context = new EfDbContext())
                {
                    var company = await context.Companies.Where(c => c.CompanyId == companyId && !c.IsDeleted)
                                                         .Include(c => c.Drivers.Select(driver => driver.Vehicles))
                                                         .Include(c => c.Vehicles)
                                                         .FirstOrDefaultAsync();

                    if (company == null) throw new NullReferenceException(nameof(CompanyEntity));

                    var drivers = company.Drivers.ToList();
                    drivers.ForEach(driver =>
                    {
                        driver.IsDeleted = true;
                        driver.SoftDeletedDate = DateTime.Now;
                        context.Drivers.AddOrUpdate(driver);
                    });

                    var vehicles = company.Vehicles.ToList();
                    vehicles.ForEach(vehicle =>
                    {
                        vehicle.IsDeleted = true;
                        vehicle.SoftDeletedDate = DateTime.Now;
                        context.Vehicles.AddOrUpdate(vehicle);
                    });

                    company.IsDeleted = true;
                    company.SoftDeletedDate = DateTime.Now;

                    context.Companies.AddOrUpdate(company);

                    await context.SaveChangesAsync();

                    return company;
                }
            }
            catch (Exception e)
            {
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
        }
    }
}