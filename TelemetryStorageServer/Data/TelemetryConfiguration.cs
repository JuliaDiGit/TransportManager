using System.Data.Entity.ModelConfiguration;
using Entities;

namespace Data
{
    public class TelemetryConfiguration : EntityTypeConfiguration<TelemetryPacketEntity>
    {
        public TelemetryConfiguration()
        {
            ToTable("telemetry");
        }
    }
}