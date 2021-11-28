using System;
using System.Threading.Tasks;
using TransportManager.Common.Enums;
using TransportManager.Mappers;
using TransportManager.Models;
using TransportManager.Services.Abstract;

namespace TransportManager.API.Controllers
{
    public class TelemetryPacketsController
    {
        private readonly ITelemetryPacketsService _telemetryPacketsService;

        public TelemetryPacketsController(ITelemetryPacketsService telemetryPacketsService)
        {
            _telemetryPacketsService = telemetryPacketsService;
        }

        public async Task<TelemetryPacketModel> GetStatisticAsync(DateTime fromDate,
                                                                  DateTime toDate,
                                                                  StatisticsType type, 
                                                                  int vehicleId = 0)
        {
            return (await _telemetryPacketsService.BuildStatisticAsync(fromDate, toDate, type, vehicleId))
                                                  .ToModel();
        }
    }
}