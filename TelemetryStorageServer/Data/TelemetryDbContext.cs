using System;
using System.Data.Entity;
using Entities;

namespace Data
{
    public class TelemetryDbContext : DbContext
    {
        public TelemetryDbContext() : base("TelemetryPacketDbConnect")
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