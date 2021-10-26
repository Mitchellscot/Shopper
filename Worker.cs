using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shopper
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly CsvStorage _csvStorage;
        private readonly Schedule _schedule;

        public Worker(ILogger<Worker> logger, CsvStorage csvStorage, Schedule schedule)
        {
            _logger = logger;
            _csvStorage = csvStorage;
            _schedule = schedule;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                //await Task.Delay(1000, stoppingToken);
                try
                {
                    var searchCriteria = _csvStorage.CheckForSearchCriteria();
                    _logger.LogInformation($"Here is the search term: \"{searchCriteria.searchterm}\" and url: \"{searchCriteria.url}\" - Please edit the CSV File if you want another search term or URL to search.");
                    await Task.Run(() => _schedule.StartTimer(), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Unable to get the program to run - {ex.Message}");

                }
            }
        }
    }
}
