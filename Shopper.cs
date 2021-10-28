using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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

        public Shopper(ILogger<Shopper> logger, Schedule schedule) => (_logger, _schedule) = (logger, schedule);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            WriteLine($"Shopper running at: {DateTimeOffset.Now}");
            var _csv = new CsvStorage(new StringBuilder());
            var scraper = new Scraper(_csv);

            while (!stoppingToken.IsCancellationRequested)
            {
                var searchCriteria = await _csv.GetSearchCriteriaAsync();
                WriteLine($"Here is the search term: \"{searchCriteria.searchterm}\" and url: \"{searchCriteria.url}\" - Please edit the CSV File if you want another search term or URL to search.");
                await _schedule.StartTimer();
                scraper.GoShopping();
            }
        }
    }
}
