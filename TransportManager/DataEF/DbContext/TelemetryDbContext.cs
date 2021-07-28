using System;
using System.Data.Entity;
using DataEF.EntitiesConfigurations;
using Entities;

namespace DataEF.DbContext
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