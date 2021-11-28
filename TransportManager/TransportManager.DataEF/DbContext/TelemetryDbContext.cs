using System;
using System.Data.Entity;
using TransportManager.DataEF.EntitiesConfigurations;
using TransportManager.Entities;

namespace TransportManager.DataEF.DbContext
{
    public class TelemetryDbContext: System.Data.Entity.DbContext
    {
        public TelemetryDbContext() : base("TelemetryDbConnect")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<TelemetryDbContext>());
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Properties<DateTime>()
                        .Configure(property => property.HasColumnType("datetime2"));

            modelBuilder.Configurations.Add(new TelemetryConfiguration());
        }
        
        public DbSet<TelemetryPacketEntity> Telemetry { get; set; }
    }
}