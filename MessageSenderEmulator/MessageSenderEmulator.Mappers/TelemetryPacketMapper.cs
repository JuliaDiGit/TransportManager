using MessageSenderEmulator.Domain;
using MessageSenderEmulator.Models;

namespace MessageSenderEmulator.Mappers
{
    public static class TelemetryPacketMapper
    {
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