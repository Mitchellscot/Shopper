using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace shopper
{
    public class Shopper : BackgroundService
    {
        private readonly ILogger<Shopper> _logger;
        private readonly CsvStorage _csv;

        public Shopper(ILogger<Shopper> logger, CsvStorage csv) => (_logger, _csv) = (logger, csv);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                WriteLine("Shopper running at: {time}", DateTimeOffset.Now);
                var searchCriteria = await _csv.GetSearchCriteriaAsync();
                WriteLine($"Here is the search term: \"{searchCriteria.searchterm}\" and url: \"{searchCriteria.url}\" - Please edit the CSV File if you want another search term or URL to search.");
            }
        }
    }
}
