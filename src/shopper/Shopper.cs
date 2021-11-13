using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using shopper.Features;
using shopper.Data;
using shopper.Models;

namespace shopper
{
    public class Shopper : BackgroundService
    {
        private readonly Schedule _schedule;
        private readonly Settings _settings;
        const string SETTINGS_FILE = "./Files/Settings.csv";


        public Shopper(Schedule schedule, Settings settings) => (_schedule, _settings) = (schedule, settings);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var _csv = new CsvStorage(_settings);
            var scraper = new Scraper(_csv, SETTINGS_FILE);

            while (!stoppingToken.IsCancellationRequested)
            {
                await _schedule.StartTimer();
                var products = scraper.GoShopping();
                if (products.Any())
                {
                    foreach (var item in products)
                    {
                        _csv.WriteToProductListFile(item, SETTINGS_FILE);
                    }
                    var emailReponse = new AwsEmail(_csv, products, SETTINGS_FILE).SendEmail();
                }
            }
        }
    }
}
