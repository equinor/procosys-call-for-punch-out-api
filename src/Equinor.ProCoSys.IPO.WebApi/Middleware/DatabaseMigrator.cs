﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.IPO.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Equinor.ProCoSys.IPO.WebApi.Middleware
{
    public class DatabaseMigrator : IHostedService
    {
        private readonly IServiceScopeFactory _serviceProvider;

        public DatabaseMigrator(IServiceScopeFactory serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<IPOContext>();
                await dbContext.Database.MigrateAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
