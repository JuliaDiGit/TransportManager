using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;

namespace DataEF.Repositories.Abstract
{
    public interface ITelemetryRepository
    {
        Task<List<TelemetryPacketEntity>> GetTelemetryAsync();
    }
}