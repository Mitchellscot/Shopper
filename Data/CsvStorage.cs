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
        private readonly Settings _settings;

        public CsvStorage(Settings settings) {
            _settings = settings;
        }

        public static async Task<List<Product>> GetProductListAsync(string path)
        {
            try
            {
                var file = await File.ReadAllLinesAsync(path);
                var lines = file.Skip(1)
                    .Where(l => l.Length > 1)
                    .ToProduct();
                return lines.ToList();
            }
            catch (FileNotFoundException)
            {
                var _sb = new StringBuilder();
                WriteLine($"Making a new csv file {path} ");
                _sb.AppendLine("Title,Price,Location,ProductDate,Link");
                await File.WriteAllTextAsync(path, _sb.ToString());
                _sb.Clear();
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

        public async void WriteToProductListFile(Product product)
        {
            var ProductListPath = $"./Files/{_settings.SearchTerm}.csv";
            try
            {
                var _sb = new StringBuilder();
                WriteLine("Adding item to file: " + product.Title.ToString());
                var newLine = string.Format($"{product.Title},{product.Price},{product.Location},{product.ProductDate},{product.Link}");
                _sb.AppendLine(newLine);
                await File.AppendAllTextAsync(ProductListPath, _sb.ToString());
                _sb.Clear();
            }
            catch (FileNotFoundException)
            {
                var _sb = new StringBuilder();
                WriteLine("Making a new csv file {ProductListPath}");
                _sb.AppendLine("Title,Price,Location,ProductDate,Link");
                await File.WriteAllTextAsync(ProductListPath, _sb.ToString());
                _sb.Clear();
            }
            catch
            {
                WriteLine($"HEY MITCH - UNABLE TO WRITE TO CSV FILE");
                throw new Exception();
            }
        }
        public async Task<Settings> GetSettingsFromFile()
        {
            try
            {
                var line = await File.ReadAllLinesAsync(SETTINGS_FILE);
                var values = line.Skip(1).Where(l => l.Length > 1).Last().Split(',');
                var settings = new Settings()
                {
                    ToAddress = values[0],
                    FromAddress = values[1],
                    SearchTerm = values[2],
                    Url = values[3]
                };
                return settings;
            }
            catch (FileNotFoundException)
            {
                var _sb = new StringBuilder();
                WriteLine("Making a new settings file");
                _sb.AppendLine("ToAddress,FromAddress,SearchTerm,Url");
                _sb.AppendLine(_settings.ToAddress + "," + _settings.FromAddress + "," + _settings.SearchTerm + "," + _settings.Url);
                await File.WriteAllTextAsync(SETTINGS_FILE, _sb.ToString());
                _sb.Clear();
                var settings = new Settings()
                {
                    FromAddress = _settings.ToAddress,
                    ToAddress = _settings.FromAddress,
                    SearchTerm = _settings.SearchTerm,
                    Url = _settings.Url
                };
                return settings;
            }
            catch (Exception ex)
            {
                WriteLine($"HEY MITCH - ERROR ACCESSING SETTINGS FILE {ex.Message}");
                throw;
            }
        }
        //This is outdated - search criteria will be written to file with a new console program
        public async Task WriteSearchCriteriaFile(string criteria)
        {
            try
            {
                var _sb = new StringBuilder();
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
        //outdated
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
                var _sb = new StringBuilder();
                WriteLine("Making a new csv file for Search Criteria");
                _sb.AppendLine("Search Term,URL");
                _sb.AppendLine($"{_settings.SearchTerm},{_settings.Url}");
                await File.WriteAllTextAsync(SEARCHTERM_FILE, _sb.ToString());
                _sb.Clear();
                return (searchterm: _settings.SearchTerm, url: _settings.Url);
            }
        }
    }
}
