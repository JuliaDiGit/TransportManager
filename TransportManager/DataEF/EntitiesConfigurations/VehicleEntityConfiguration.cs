using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Entities;

namespace DataEF.EntitiesConfigurations
{
    public class VehicleEntityConfiguration : EntityTypeConfiguration<VehicleEntity>
    {
        public VehicleEntityConfiguration()
        {
            ToTable("vehicles");
            Property(vehicle => vehicle.Model).IsRequired().HasMaxLength(80);
            Property(vehicle => vehicle.GovernmentNumber).IsRequired().HasMaxLength(9);
            Property(vehicle => vehicle.CreatedDate).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
        }
    }
}