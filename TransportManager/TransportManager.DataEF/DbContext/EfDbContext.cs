using System;
using System.Data.Entity;
using TransportManager.DataEF.EntitiesConfigurations;
using TransportManager.Entities;

namespace TransportManager.DataEF.DbContext
{
    public class EfDbContext : System.Data.Entity.DbContext
    {
        public EfDbContext() : base("DbConnect")
        {
            Database.SetInitializer(new EfDbInitializer());
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Properties<DateTime>()
                        .Configure(property => property.HasColumnType("datetime2"));

            modelBuilder.Configurations.Add(new UserEntityConfiguration());
            modelBuilder.Configurations.Add(new CompanyEntityConfiguration());
            modelBuilder.Configurations.Add(new DriverEntityConfiguration());
            modelBuilder.Configurations.Add(new VehicleEntityConfiguration());
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<CompanyEntity> Companies { get; set; }
        public DbSet<DriverEntity> Drivers { get; set; }
        public DbSet<VehicleEntity> Vehicles { get; set; }
    }
}