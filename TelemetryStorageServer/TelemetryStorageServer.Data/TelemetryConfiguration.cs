using System.Data.Entity.ModelConfiguration;
using TelemetryStorageServer.Entities;

namespace TelemetryStorageServer.Data
{
    public class TelemetryConfiguration : EntityTypeConfiguration<TelemetryPacketEntity>
    {
        public TelemetryConfiguration()
        {
            ToTable("telemetry");
        }
    }
}