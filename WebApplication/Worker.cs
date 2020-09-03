using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApplication.Services;

namespace MigrationApp
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IForgeOSS _forgeOSS;

        public  Worker(ILogger<Worker> logger, IForgeOSS forgeOSS)
        {
            _logger = logger;
            _forgeOSS = forgeOSS;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
