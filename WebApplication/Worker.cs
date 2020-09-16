/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Design Automation team for Inventor
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MigrationApp
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly Migration _migration;

        public Worker(Migration migration, ILogger<Worker> logger)
        {
            _migration = migration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await MigrateAll();
                }
                catch(Exception e)
                {
                    _logger.LogError(e, "Fatal error during migration process !!!");
                }

                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task MigrateAll()
        {
            _logger.LogInformation("Scanning buckets for migration");
            List<MigrationJob> jobs = await _migration.ScanBuckets();
            _logger.LogInformation($"Migration is performing {jobs.Count} operations");
            await _migration.Migrate(jobs);
            _logger.LogInformation("Migration finished");
        }
    }
}
