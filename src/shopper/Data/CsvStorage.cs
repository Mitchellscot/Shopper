using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using shopper.Models;
using shopper.Helpers;
using static System.Console;
#nullable enable


namespace shopper.Data
{
    public class CsvStorage
    {
        private readonly Settings _settings;

        public CsvStorage(Settings settings) {
            _settings = settings;
        }

        public static async Task<List<Product>?> GetProductListAsync(string path)
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
                createNewProductList(path);
                return new List<Product>();
            }
            catch (Exception ex)
            {
                WriteLine($"HEY MITCH - ERROR PROCESSING THE CSV FILE - {ex.Message}");
                throw new Exception(ex.Message);
            }
        }
        private static async void createNewProductList(string path)
        {
            var _sb = new StringBuilder();
            WriteLine($"Making a new csv file {path} ");
            _sb.AppendLine("Title,Price,Location,ProductDate,Link");
            await File.WriteAllTextAsync(path, _sb.ToString());
            _sb.Clear();
        }

        public async void WriteToProductListFile(Product product, string settingsFilePath)
        {
            var ProductListPath = $"./Files/{GetSettingsFromFile(settingsFilePath).Result.SearchTerm}.csv";
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
        public async Task<Settings> GetSettingsFromFile(string settingsFilePath)
        {
            try
            {
                var line = await File.ReadAllLinesAsync(settingsFilePath);
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
                await File.WriteAllTextAsync(settingsFilePath, _sb.ToString());
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
    }
}
