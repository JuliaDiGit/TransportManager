using System;

namespace Models
{
    public class TelemetryPacketModel
    {
        public int VehicleId { get; set; }
        public DateTime Date { get; set; }
        public double Distance { get; set; }
        public double FuelConsumption { get; set; }
        public TimeSpan TravelTime { get; set; }
    }
}