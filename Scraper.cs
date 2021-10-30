using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shopper.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static System.Console;

namespace shopper
{
    public class Scraper
    {
        private readonly CsvStorage _data;

        public Scraper(CsvStorage data) => (_data) = (data);

        public List<Product> GoShopping()
        {
            (string searchTerm, string url) = (_data.GetSettingsFromFile().Result.SearchTerm, _data.GetSettingsFromFile().Result.Url);
            string fileName = $"./Files/{searchTerm}.csv";
            //bring in data from csv file to prevent finding the same items over and over again
            var previouslyFoundItems = CsvStorage.GetProductListAsync(fileName);
            //go shopping at the given url
            var foundItems = scrape(url).Result;
            //find all items with given search term. If not found previously, add it to the list of new items
            var filteredItems = new List<Product>();
            foreach (var item in foundItems)
            {
                if (item.Title.ToLower().Contains(searchTerm))
                {
                    WriteLine($"Found an item: {item.Title} - {item.Price} - {item.Location} - {item.ProductDate} - {item.Link}");
                    if (!previouslyFoundItems.Result.Any(x => x.ProductDate == item.ProductDate))
                    {
                        WriteLine("NEW ITEM!");
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
            string Url = string.Empty;
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
                            Location = "None Given";
                        }
                        else
                        {
                            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                            Location = textInfo.ToTitleCase(Location.Substring(1, Location.Length - 2).ToLower()).Replace(",", "").Trim();
                        }
                    }
                    else
                    {
                        Location = "None Given";
                    }
                }
                var resultHeading = row.Descendants().Where(x => x.HasClass("result-heading"));
                foreach (var child in resultHeading)
                {
                    Title = HttpUtility.UrlDecode(child.Descendants().Where(x => x.Attributes.Contains("href")).First().InnerText);
                    Url = child.Descendants().Where(x => x.Attributes.Contains("href")).First().GetAttributeValue("href", "");
                }
                Date = DateTime.Parse(row.Descendants().Where(x => x.HasClass("result-date")).First().GetAttributeValue("datetime", "NOW"));
                var newProduct = new Product()
                {
                    Title = Title,
                    ProductDate = Date.Value,
                    Price = Price,
                    Location = Location,
                    Link = Url
                };
                products.Add(newProduct);
            }
            
            return products;
        }
        private async Task<HtmlDocument> goScraping(string url)
        {
            try
            {
                var htmlDoc = new HtmlWeb();
                WriteLine($" This is the user agent or whatever {htmlDoc.UserAgent}");
                var webPage = await htmlDoc.LoadFromWebAsync(url);
                return webPage;
            }
            catch (Exception ex)
            {
                WriteLine($"HEY MITCH - UNABLE TO GET THE WEB PAGE {ex.Message}");
                throw new Exception();
            }
        }
    }
}
