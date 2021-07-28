using System.Data.Entity.ModelConfiguration;
using Entities;

namespace DataEF.EntitiesConfigurations
{
    public class TelemetryConfiguration: EntityTypeConfiguration<TelemetryPacketEntity>
    {
        public TelemetryConfiguration()
        {
            ToTable("telemetry");
        }
    }
}