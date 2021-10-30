using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shopper.Models;
using System;
using System.Linq;
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
        private readonly Settings _settings;

        public Shopper(ILogger<Shopper> logger, Schedule schedule, Settings settings) => (_logger, _schedule, _settings) = (logger, schedule, settings);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var _csv = new CsvStorage(_settings);
            var scraper = new Scraper(_csv);

            while (!stoppingToken.IsCancellationRequested)
            {
                await _schedule.StartTimer();
                //add a check here to see if file exists, or whatever....
                var products = scraper.GoShopping();
                if(products.Any())
                {
                    foreach (var item in products)
                    {
                        _csv.WriteToProductListFile(item);
                    }
                    var emailReponse = new AwsEmail(_csv, products).SendEmail();
                }
            }
        }
    }
}
