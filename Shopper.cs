using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shopper.Models;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace shopper
{
    public class Shopper : BackgroundService
    {
        private readonly ILogger<Shopper> _logger;
        private readonly Schedule _schedule;
        private readonly IOptions<Settings> _settings;

        public Shopper(ILogger<Shopper> logger, Schedule schedule, IOptions<Settings> settings) => (_logger, _schedule, _settings) = (logger, schedule, settings);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var _csv = new CsvStorage(new StringBuilder(), _settings);
            var scraper = new Scraper(_csv, _settings);

            while (!stoppingToken.IsCancellationRequested)
            {
                await _schedule.StartTimer();
                scraper.GoShopping();
            }
        }
    }
}
