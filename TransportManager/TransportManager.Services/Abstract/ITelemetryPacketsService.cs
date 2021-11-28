using System;
using System.Threading.Tasks;
using TransportManager.Common.Enums;
using TransportManager.Domain;

namespace TransportManager.Services.Abstract
{
    public interface ITelemetryPacketsService
    {
        Task<TelemetryPacket> BuildStatisticAsync(DateTime fromDate,
                                                  DateTime toDate,
                                                  StatisticsType type,
                                                  int vehicleId = 0);
    }
}