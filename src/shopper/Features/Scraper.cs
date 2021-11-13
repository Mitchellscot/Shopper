using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using shopper.Data;
using shopper.Models;
using static System.Console;

namespace shopper.Features
{
    public class Scraper
    {
        private readonly CsvStorage _data;
        private string _settingsFilePath;

        public Scraper(CsvStorage data, string settingsFilePath) => (_data, _settingsFilePath) = (data, settingsFilePath);

        public List<Product> GoShopping()
        {
            (string searchTerm, string url) = (_data.GetSettingsFromFile(_settingsFilePath).Result.SearchTerm, _data.GetSettingsFromFile(_settingsFilePath).Result.Url);
            string fileName = $"./Files/{searchTerm}.csv";
            var previouslyFoundItems = CsvStorage.GetProductListAsync(fileName);
            var foundItems = scrape(url).Result;
            List<Product> filteredItems = new();
            foreach (var item in foundItems)
            {
                if (item.Title.ToLower().Contains(searchTerm))
                {
                    if (previouslyFoundItems.Result == null)
                    {
                        filteredItems.Add(item);
                    }
                    if (!previouslyFoundItems.Result.Any(x => x.Title == item.Title))
                    {
                        filteredItems.Add(item);
                    }
                }
            }
            return filteredItems;
        }

        private async Task<List<Product>> scrape(string url)
        {
            var products = new List<Product>();
            decimal Price = 0M;
            string Title = string.Empty;
            string Location = string.Empty;
            string Link = string.Empty;
            DateTime? Date = null;
            var web = await goScraping(url);

            var rows = web.DocumentNode.SelectNodes("//*[@class=\"result-row\"]");
            foreach (var row in rows)
            {
                var resultMetaData = row.Descendants("span").Where(x => x.HasClass("result-meta"));
                foreach (var child in resultMetaData)
                {
                    Price = Convert.ToDecimal(child.Descendants("span").SingleOrDefault(x => x.HasClass("result-price")).InnerText.Replace("$", "").Replace(",", "").Trim());
                    if (child.Descendants("span").Any(x => x.HasClass("result-hood")))
                    {
                        Location = child.Descendants("span").SingleOrDefault(x => x.HasClass("result-hood")).InnerText.Trim();
                        if (Location == "(  )")
                        {
                            Location = null;
                        }
                        else
                        {
                            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                            Location = textInfo.ToTitleCase(Location.Substring(1, Location.Length - 2).ToLower()).Replace(",", "").Trim();
                        }
                    }
                    else
                    {
                        Location = null;
                    }
                }
                var resultHeading = row.Descendants().Where(x => x.HasClass("result-heading"));
                foreach (var child in resultHeading)
                {
                    Title = HttpUtility.UrlDecode(child.Descendants().Where(x => x.Attributes.Contains("href")).First().InnerText);
                    Link = child.Descendants().Where(x => x.Attributes.Contains("href")).First().GetAttributeValue("href", "");
                }
                Date = DateTime.Parse(row.Descendants().Where(x => x.HasClass("result-date")).First().GetAttributeValue("datetime", "NOW"));
                if (string.IsNullOrEmpty(Location)){
                    TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                    Location ??= textInfo.ToTitleCase(Link.Remove(Link.IndexOf(".")).Substring(Link.IndexOf("/") + 2));
                }
                var newProduct = new Product()
                {
                    Title = Title,
                    ProductDate = Date.Value,
                    Price = Price,
                    Location = Location,
                    Link = Link
                };
                products.Add(newProduct);
            }

            return products;
        }
        private async Task<HtmlDocument> goScraping(string url)
        {
            Random rando = new();
            try
            {
                HtmlWeb htmlDoc = new();
                htmlDoc.UserAgent = assignUserAgent().Result.ElementAt(rando.Next(1, 1000));
                var webPage = await htmlDoc.LoadFromWebAsync(url);
                return webPage;
            }
            catch (Exception ex)
            {
                WriteLine($"HEY MITCH - UNABLE TO GET THE WEB PAGE {ex.Message}");
                throw new Exception();
            }
        }
        private async Task<List<string>> assignUserAgent()
        {
            List<string> lines = new();
            foreach (var line in await File.ReadAllLinesAsync("./Files/user-agents.txt"))
            {
                lines.Add(line);
            }
            return lines;
        }
    }
}
