using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static System.Console;
using Microsoft.Extensions.Options;
using Shopper.Models;

namespace shopper
{
    public class CsvStorage
    {
        const string SEARCHTERM_FILE = "./Files/SearchCriteria.csv";
        const string SETTINGS_FILE = "./Files/Settings.csv";
        private readonly StringBuilder _sb;
        private readonly IOptions<Settings> _settings;

        public CsvStorage(StringBuilder sb, IOptions<Settings> settings) => (_sb, _settings) = (sb, settings);

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

        public async void CheckForProductListFile(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    WriteLine("Making a new csv file" + path);
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

        public async void WriteToProductListFile(Product product, string path)
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
                var line = await File.ReadAllLinesAsync(SEARCHTERM_FILE);
                var tuple = line.Skip(1).Where(l => l.Length > 1).Last().Split(',');
                return (searchterm: tuple[0], url: tuple[1]);
            }
            catch (FileNotFoundException)
            {
                WriteLine("Making a new csv file for Search Criteria");
                _sb.AppendLine("Search Term,URL");
                _sb.AppendLine($"{_settings.Value.SearchTerm},{_settings.Value.Url}");
                await File.WriteAllTextAsync(SEARCHTERM_FILE, _sb.ToString());
                _sb.Clear();
                return (searchterm: _settings.Value.SearchTerm, url: _settings.Value.Url);
            }
        }

        public async Task WriteSearchCriteriaFile(string criteria)
        {
            try
            {
                _sb.AppendLine(criteria);
                await File.AppendAllTextAsync(SEARCHTERM_FILE, _sb.ToString());
                _sb.Clear();
            }
            catch (Exception ex)
            {
                WriteLine($"HEY MITCH - COULDNT WRITE TO SEARCH CRITERIA FILE {ex.Message}");
                throw;
            }
        }
        public async Task<Settings> GetSettingsFromFile()
        {
            try
            {
                var line = await File.ReadAllLinesAsync(SEARCHTERM_FILE);
                var values = line.Skip(1).Where(l => l.Length > 1).Last().Split(',');
                var settings = new Settings()
                {
                    FromAddress = values[0],
                    ToAddress = values[1],
                    SearchTerm = values[2],
                    Url = values[3]
                };
                return settings;
            }
            catch (FileNotFoundException)
            {
                WriteLine("Making a new settings file");
                _sb.AppendLine("ToAddress,FromAddress,SearchTerm,Url");
                _sb.AppendLine(_settings.Value.ToAddress + "," + _settings.Value.FromAddress + "," + _settings.Value.SearchTerm + "," + _settings.Value.Url);
                await File.WriteAllTextAsync(SETTINGS_FILE, _sb.ToString());
                _sb.Clear();
                var settings = new Settings()
                {
                    FromAddress = _settings.Value.ToAddress,
                    ToAddress = _settings.Value.FromAddress,
                    SearchTerm = _settings.Value.SearchTerm,
                    Url = _settings.Value.Url
                };
                return settings;
            }
            catch (Exception ex)
            {
                WriteLine($"HEY MITCH - ERROR ACCESSING SETTINGS FILE {ex.Message}");
                throw;
            }
        }
    }
}
