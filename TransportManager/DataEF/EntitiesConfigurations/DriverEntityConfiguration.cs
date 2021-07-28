using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Entities;

namespace DataEF.EntitiesConfigurations
{
    public class DriverEntityConfiguration : EntityTypeConfiguration<DriverEntity>
    {
        public DriverEntityConfiguration()
        {
            ToTable("drivers");
            Property(driver => driver.Name).IsRequired().HasMaxLength(50);
            Property(driver => driver.CreatedDate).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
        }
    }
}