using System;

namespace TelemetryStorageServer.Domain
{
    public class TelemetryPacket
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public DateTime Date { get; set; }
        public double Distance { get; set; }
        public double FuelConsumption { get; set; }
        public TimeSpan TravelTime { get; set; }
        
        public override string ToString()
        {
            return $"Id: {VehicleId}, Date: {Date}, Distance: {Math.Round(Distance, 2)}км, " +
                   $"Fuel consumption: {Math.Round(FuelConsumption, 2)}л, Travel time: {TravelTime}";
        }
    }
}