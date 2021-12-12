using TelemetryStorageServer.Domain;
using TelemetryStorageServer.Entities;

namespace TelemetryStorageServer.Mappers
{
    public static class TelemetryPacketMapper
    {
        public static TelemetryPacket ToDomain(this TelemetryPacketEntity telemetryPacketEntity)
        {
            if (telemetryPacketEntity == null) return null;
            
            return new TelemetryPacket
            {
                VehicleId = telemetryPacketEntity.VehicleId,
                Date = telemetryPacketEntity.Date,
                Distance = telemetryPacketEntity.Distance,
                FuelConsumption = telemetryPacketEntity.FuelConsumption,
                TravelTime = telemetryPacketEntity.TravelTime
            };
        }
        
        public static TelemetryPacketEntity ToEntity(this TelemetryPacket telemetryPacket)
        {
            if (telemetryPacket == null) return null;
            
            return new TelemetryPacketEntity
            {
                VehicleId = telemetryPacket.VehicleId,
                Date = telemetryPacket.Date,
                Distance = telemetryPacket.Distance,
                FuelConsumption = telemetryPacket.FuelConsumption,
                TravelTime = telemetryPacket.TravelTime
            };
        }
    }
}