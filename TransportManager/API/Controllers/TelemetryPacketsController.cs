using System;
using System.Threading.Tasks;
using Enums;
using Mappers;
using Models;
using Services.Abstract;

namespace API.Controllers
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