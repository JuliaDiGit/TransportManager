using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enums;
using Mappers;
using Models;
using Services;

namespace API
{
    public class EmulatorController
    {
        private readonly TelemetryPacketsService _packetsService;
        
        public EmulatorController(TelemetryPacketsService packetsService)
        {
            _packetsService = packetsService;
        }

        public TelemetryPacketModel GetOneTelemetryPacket()
        {
            return _packetsService.GetOneTelemetryPacket().ToModel();
        }
        
        public TelemetryPacketModel AddTelemetryToSendingList(TelemetryPacketModel telemetryPacketModel)
        {
            if (telemetryPacketModel == null) throw new ArgumentNullException(nameof(telemetryPacketModel));
            
            return _packetsService.AddTelemetryPacketToSendingList(telemetryPacketModel)
                                  .ToModel();
        }

        public List<TelemetryPacketModel> GetTelemetryPacketsListForSending()
        {
            return _packetsService.GetTelemetryPacketForSending().Select(t => t.ToModel())
                                                                 .ToList();
        }

        public async Task<string> SendAsync(SendingType sendingType)
        {
            return await _packetsService.SendAsync(sendingType);
        }
    }
}