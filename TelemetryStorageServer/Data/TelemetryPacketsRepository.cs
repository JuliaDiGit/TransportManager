﻿using System;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using Domain;
using Entities;
using Mappers;

namespace Data
{
    public class TelemetryPacketsRepository
    {
        public async Task<TelemetryPacketEntity> AddTelemetryPacketAsync(TelemetryPacket telemetryPacket)
        {
            if (telemetryPacket == null) throw new ArgumentNullException(nameof(telemetryPacket));
            
            var telemetryEntity = telemetryPacket.ToEntity();

            try
            {
                using (var context = new TelemetryDbContext())
                {
                    context.Telemetry.AddOrUpdate(telemetryEntity);

                    await context.SaveChangesAsync();

                    return telemetryEntity;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return null;
            }
        }
    }
}