using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace Shopper
{
    public class SearchCriteria
    {
        private readonly ILogger<SearchCriteria> _logger;

        public SearchCriteria(ILogger<SearchCriteria> logger)
        {
            _logger = logger;
        }
        public void ConfirmSearchCriteria()
        {
            var searchCriteria = CsvStorage.CheckForSearchCriteria();
            _logger.LogInformation($"Here is the search term: \"{searchCriteria.searchterm}\" and url: \"{searchCriteria.url}\"");


        }
    }
}
