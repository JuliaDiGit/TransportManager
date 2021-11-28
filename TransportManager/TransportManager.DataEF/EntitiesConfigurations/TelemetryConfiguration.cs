using System.Data.Entity.ModelConfiguration;
using TransportManager.Entities;

namespace TransportManager.DataEF.EntitiesConfigurations
{
    public class TelemetryConfiguration: EntityTypeConfiguration<TelemetryPacketEntity>
    {
        public TelemetryConfiguration()
        {
            ToTable("telemetry");
        }
    }
}