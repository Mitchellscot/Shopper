using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static System.Console;

namespace shopper
{
    public class CsvStorage
    {
        private readonly StringBuilder _sb;
        public CsvStorage(StringBuilder sb) => _sb = sb;
        public static async Task<List<Product>> ProcessFileAsync(string path)
        {
            try
            {
                var file = await File.ReadAllLinesAsync(path);
                var lines = file.Skip(1)
                    .Where(l => l.Length > 1)
                    .ToProduct();
                return lines.ToList();
            }
            catch (Exception ex)
            {
                WriteLine($"HEY MITCH - ERROR PROCESSING THE CSV FILE - {ex.Message}");
                throw new Exception();
            }
        }
        public async void CheckForCsvFileAsync(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    WriteLine("Making a new csv file");
                    _sb.AppendLine("Title,Price,Location,ProductDate,Link");
                    await File.WriteAllTextAsync(path, _sb.ToString());
                    _sb.Clear();
                }
            }
            catch (Exception ex)
            {
                WriteLine($"HEY MITCH - ERROR CHECKING FOR THE CSV FILE - {ex.Message}");
            }
        }

        public async void WriteToCsvFileAsync(Product product, string path)
        {
            try
            {
                WriteLine("Adding item to file: " + product.Title.ToString());
                var newLine = string.Format($"{product.Title},{product.Price},{product.Location},{product.ProductDate},{product.Link}");
                _sb.AppendLine(newLine);
                await File.AppendAllTextAsync(path, _sb.ToString());
                _sb.Clear();
            }
            catch (Exception ex)
            {
                WriteLine($"HEY MITCH - UNABLE TO WRITE TO CSV FILE {ex.Message}");
                throw new Exception();
            }
        }

        public async Task<(string searchterm, string url)> GetSearchCriteriaAsync()
        {
            try
            {
                var line = await File.ReadAllLinesAsync("SearchCriteria.csv");
                var tuple = line.Skip(1).Where(l => l.Length > 1).Last().Split(',');
                return (searchterm: tuple[0], url: tuple[1]);
            }
            catch (FileNotFoundException)
            {
                WriteLine("Making a new csv file for Search Criteria");
                _sb.AppendLine("Search Term,URL");
                _sb.AppendLine("fat,https://brainerd.craigslist.org/d/bicycles/search/bia?postal=56425&search_distance=50");
                await File.WriteAllTextAsync("SearchCriteria.csv", _sb.ToString());
                _sb.Clear();
                return (searchterm: "fat", url: "https://brainerd.craigslist.org/d/bicycles/search/bia?postal=56425&search_distance=50");
            }
        }
        public async Task WriteSearchCriteriaFile(string criteria)
        {
            try
            {
                _sb.AppendLine(criteria);
                await File.AppendAllTextAsync("SearchCriteria.csv", _sb.ToString());
                _sb.Clear();
            }
            catch (Exception ex)
            {
                WriteLine($"HEY MITCH - COULDNT WRITE TO SEARCH CRITERIA FILE {ex.Message}");
                throw;
            }

        }
    }
}
