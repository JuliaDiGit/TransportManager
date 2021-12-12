using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MessageSenderEmulator.Domain;
using MessageSenderEmulator.Enums;
using MessageSenderEmulator.Mappers;
using MessageSenderEmulator.MessageSenders;
using MessageSenderEmulator.Models;

namespace MessageSenderEmulator.Services
{
    public class TelemetryPacketsService
    {
        private static List<TelemetryPacket> _allTelemetryPackets;
        private static List<TelemetryPacket> _telemetryPacketsForSending = new List<TelemetryPacket>();
        private Random _random = new Random();

        public TelemetryPacketsService()
        {
            //создаем список TelemetryPacket,
            //в методе Generate можно задать кол-во ТС и кол-во дней для увеличения/уменьшения записей в списке
            if (_allTelemetryPackets == null) _allTelemetryPackets = TelemetryPacketsGenerator.Generate(10, 3);
        }

        public TelemetryPacket GetOneTelemetryPacket()
        {
            if (_allTelemetryPackets == null || _allTelemetryPackets.Count == 0) return null;

            //выдаем рандомную TelemetryPacket, после чего удаляем её из списка
            TelemetryPacket telemetryPacket = _allTelemetryPackets[_random.Next(0, _allTelemetryPackets.Count)];
            _allTelemetryPackets.Remove(telemetryPacket);

            return telemetryPacket;
        }
        
        public TelemetryPacket AddTelemetryPacketToSendingList(TelemetryPacketModel telemetryPacketModel)
        {
            if (telemetryPacketModel == null) throw new ArgumentNullException(nameof(telemetryPacketModel));

            var telemetry = telemetryPacketModel.ToDomain();
            _telemetryPacketsForSending.Add(telemetry);

            return telemetry;
        }

        public List<TelemetryPacket> GetTelemetryPacketForSending()
        {
            return _telemetryPacketsForSending;
        }
        
        public async Task<string> SendAsync(SendingType sendingType)
        {
            if (_telemetryPacketsForSending.Count == 0) return null;
            
            var byteArrays = new List<byte[]>();
            
            //преобразуем статистики в массивы байт и добавляем их в новый список
            _telemetryPacketsForSending.ForEach(s => byteArrays.Add(PackTelemetryPacket(s)));
            
            var messageSender = new MessageSender();
            var result = await messageSender.SendAsync(sendingType, byteArrays);
            
            //т.к сообщения успешно отправились - обнуляем список для накопления новых сообщений
            if (result != null) _telemetryPacketsForSending = new List<TelemetryPacket>();

            return result;
        }

        private static byte[] PackTelemetryPacket(TelemetryPacket telemetryPacket)
        {
            if (telemetryPacket == null) throw new ArgumentNullException(nameof(telemetryPacket));
            
            const string packageWithTypesPrefix = "@T"; //префикс, дающий понять, что за тип пакета пришёл
            const string telemetryTypePrefix = "TT"; //префикс, дающий понять, что сейчас будет Телеметрия
            const string endOfPackagePrefix = "EN";

            byte[] package = new byte[38];

            using (var stream = new MemoryStream(package))
            {
                using (var binaryWriter = new BinaryWriter(stream))
                {
                    binaryWriter.Write(Encoding.ASCII.GetBytes(packageWithTypesPrefix)); //2 байта
                    binaryWriter.Write(Encoding.ASCII.GetBytes(telemetryTypePrefix)); //2 байта

                    binaryWriter.Write(telemetryPacket.VehicleId); //int - 4 байта
                    binaryWriter.Write(((DateTimeOffset) telemetryPacket.Date).ToUnixTimeSeconds()); //long - 8 байт
                    binaryWriter.Write(telemetryPacket.Distance); //double - 8 байт
                    binaryWriter.Write(telemetryPacket.FuelConsumption); //double - 8 байт
                    binaryWriter.Write((int)telemetryPacket.TravelTime.TotalSeconds); //int - 4 байта

                    binaryWriter.Write(Encoding.ASCII.GetBytes(endOfPackagePrefix)); //2 байта
                }
            }

            return package;
        }
    }
}