using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static System.Console;
using Microsoft.Extensions.Logging;

namespace Shopper
{
    public class CsvStorage
    {
        private readonly ILogger<CsvStorage> _logger;
        private readonly StringBuilder _stringBuilder;

        public CsvStorage(ILogger<CsvStorage> logger, StringBuilder sb)
        {
            _logger = logger;
            _stringBuilder = sb;
        }
        public static List<Product> ProcessFile(string path)
        {
            var query =
                 File.ReadAllLines(path)
                .Skip(1)
                .Where(l => l.Length > 1)
                .ToProduct();
            return query.ToList();
        }
        public void CheckForCsvFile(string path)
        {
            if (!File.Exists(path))
            {
                _logger.LogInformation("Making a new csv file");
                _stringBuilder.AppendLine("Title,Price,Location,ProductDate,Link");
                File.WriteAllText(path, _stringBuilder.ToString());
                _stringBuilder.Clear();
            }
        }
        public void WriteToCsvFile(Product product, string path)
        {
            _logger.LogInformation("Adding item to file: " + product.Title.ToString());
            var newLine = string.Format($"{product.Title},{product.Price},{product.Location},{product.ProductDate},{product.Link}");
            _stringBuilder.AppendLine(newLine);
            File.AppendAllText(path, _stringBuilder.ToString());
            _stringBuilder.Clear();
        }
        public (string searchterm, string url) CheckForSearchCriteria()
        {
            if (!File.Exists("SearchCriteria.csv"))
            {
                _logger.LogInformation("Making a new csv file for Search Criteria");
                _stringBuilder.AppendLine("Search Term,URL");
                _stringBuilder.AppendLine("fat,https://brainerd.craigslist.org/d/bicycles/search/bia?postal=56425&search_distance=50");
                File.WriteAllText("SearchCriteria.csv", _stringBuilder.ToString());
                _stringBuilder.Clear();
            }
            return GetSearchCriteria();
        }
        public (string searchterm, string url) GetSearchCriteria()
        {
            var query = File.ReadAllLines("SearchCriteria.csv").Skip(1).Where(l => l.Length > 1).Last().Split(',');
            return (searchterm: query[0], url: query[1]);
        }
        public void WriteSearchCriteriaFile(string criteria)
        {
            _stringBuilder.AppendLine(criteria);
            File.AppendAllText("SearchCriteria.csv", _stringBuilder.ToString());
            _stringBuilder.Clear();
        }
    }
}
