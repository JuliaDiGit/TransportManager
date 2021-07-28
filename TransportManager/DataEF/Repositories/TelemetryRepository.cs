using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DataEF.DbContext;
using DataEF.Repositories.Abstract;
using Entities;
using Exceptions;

namespace DataEF.Repositories
{
    public class TelemetryRepository : ITelemetryRepository
    {
        public async Task<List<TelemetryPacketEntity>> GetTelemetryAsync()
        {
            try
            {
                using (var context = new TelemetryDbContext())
                {
                    return await context.Telemetry
                                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                if (e is SqlException) throw new SourceNotAvailableException();
                return null;
            }
        }
    }
}