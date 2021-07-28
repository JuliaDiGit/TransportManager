using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Data;
using Domain;
using Mappers;

namespace Services
{
    public class TelemetryPacketsService
    {
        private readonly TelemetryPacketsRepository _telemetryPacketsPacketRepository;
        
        public TelemetryPacketsService(TelemetryPacketsRepository telemetryPacketsPacketRepository)
        {
            _telemetryPacketsPacketRepository = telemetryPacketsPacketRepository;
        }
        
        public TelemetryPacket Unpack(byte[] package)
        {
            const string packageWithTypesPrefix = "@T"; //префикс, дающий понять, что за тип пакета пришёл
            const string telemetryTypePrefix = "TT"; //префикс, дающий понять, что сейчас будет TelemetryPacket

            TelemetryPacket telemetryPacketForRead = new TelemetryPacket();

            try
            {
                using (MemoryStream readStream = new MemoryStream(package))
                {
                    using (BinaryReader binaryReader = new BinaryReader(readStream))
                    {
                        var packagePrefixBytes = binaryReader.ReadBytes(2);
                        var packagePrefix = Encoding.ASCII.GetString(packagePrefixBytes);

                        if (packagePrefix.Equals(packageWithTypesPrefix))
                        {
                            var typePrefixBytes = binaryReader.ReadBytes(2);
                            var typePrefix = Encoding.ASCII.GetString(typePrefixBytes);

                            if (typePrefix == telemetryTypePrefix) ReadTelemetryPacket(binaryReader, out telemetryPacketForRead);
                        }
                    }
                }

                return telemetryPacketForRead;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        private static void ReadTelemetryPacket(BinaryReader reader, out TelemetryPacket telemetryPacket)
        {
            var vehicleId = reader.ReadInt32();
            var date = (DateTimeOffset.FromUnixTimeSeconds((long)reader.ReadUInt64())).LocalDateTime;
            var distance = reader.ReadDouble();
            var fuelConsumption = reader.ReadDouble();
            var travelTime = TimeSpan.FromSeconds(reader.ReadInt32());

            telemetryPacket = new TelemetryPacket
            {
                VehicleId = vehicleId,
                Date = date,
                Distance = distance,
                FuelConsumption = fuelConsumption,
                TravelTime = travelTime
            };
        }

        public async Task<TelemetryPacket> AddTelemetryPacketAsync(TelemetryPacket telemetryPacket)
        {
            if (telemetryPacket == null) throw new ArgumentNullException(nameof(telemetryPacket));

            return (await _telemetryPacketsPacketRepository.AddTelemetryPacketAsync(telemetryPacket))
                                              .ToDomain();
        }
    }
}