using System;
using System.Threading.Tasks;
using Domain;
using Enums;

namespace Services.Abstract
{
    public interface ITelemetryPacketsService
    {
        Task<TelemetryPacket> BuildStatisticAsync(DateTime fromDate,
                                                  DateTime toDate,
                                                  StatisticsType type,
                                                  int vehicleId = 0);
    }
}