using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportManager.Entities;

namespace TransportManager.DataEF.Repositories.Abstract
{
    public interface ITelemetryRepository
    {
        Task<List<TelemetryPacketEntity>> GetTelemetryAsync();
    }
}