using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Entities;

namespace DataEF.EntitiesConfigurations
{
    public class CompanyEntityConfiguration : EntityTypeConfiguration<CompanyEntity>
    {
        public CompanyEntityConfiguration()
        {
            ToTable("companies");
            HasKey(company => company.CompanyId);
            Property(company => company.CompanyName).IsRequired().HasMaxLength(80);
            Property(company => company.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(company => company.CreatedDate).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
        }
    }
}