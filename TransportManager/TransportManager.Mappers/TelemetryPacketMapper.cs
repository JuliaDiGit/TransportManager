using TransportManager.Domain;
using TransportManager.Entities;
using TransportManager.Models;

namespace TransportManager.Mappers
{
    public static class TelemetryPacketMapper
    {
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
        
        public static TelemetryPacket ToDomain(this TelemetryPacketModel telemetryPacketModel)
        {
            if (telemetryPacketModel == null) return null;
            
            return new TelemetryPacket
            {
                VehicleId = telemetryPacketModel.VehicleId,
                Date = telemetryPacketModel.Date,
                Distance = telemetryPacketModel.Distance,
                FuelConsumption = telemetryPacketModel.FuelConsumption,
                TravelTime = telemetryPacketModel.TravelTime
            };
        }
        
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

        public static TelemetryPacketModel ToModel(this TelemetryPacket telemetryPacket)
        {
            if (telemetryPacket == null) return null;
            
            return new TelemetryPacketModel()
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