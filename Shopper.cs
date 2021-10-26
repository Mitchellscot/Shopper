using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Shopper
{
    public class Shopper
    {
        private readonly CsvStorage _csvStorage;
        private readonly ILogger<Shopper> _logger;
        private readonly HtmlWeb _htmlDoc;

        public Shopper(CsvStorage csvStorage, ILogger<Shopper> logger, HtmlWeb htmlDoc)
        {
            _csvStorage = csvStorage;
            _logger = logger;
            _htmlDoc = htmlDoc;
        }
        public void GoShopping()
        {
            var searchCriteria = _csvStorage.GetSearchCriteria();
            string fileName = searchCriteria.searchterm.ToString() + ".csv";
            _csvStorage.CheckForCsvFile(fileName);
            //bring in data from csv file to prevent finding the same items over and over again
            var previouslyFoundItems = CsvStorage.ProcessFile(fileName);
            //got shopping at the given url
            var foundItems = Shop(searchCriteria.url);
            //find all items with given search term. If not found previously, add it to the list of new items
            var filteredItems = new List<Product>();
            foreach (var item in foundItems)
            {
                if (item.Title.ToLower().Contains(searchCriteria.searchterm))
                {
                    _logger.LogInformation($"Found an item: {item.Title} - {item.Price} - {item.Location} - {item.ProductDate} - {item.Link}");
                    if (!previouslyFoundItems.Any(x => x.ProductDate == item.ProductDate))
                    {
                        _logger.LogInformation($"NEW ITEM!");
                        filteredItems.Add(item);
                    }
                }
            }
            //write items to file
            if (filteredItems.Count > 0)
            {
                foreach (var item in filteredItems)
                {
                    _csvStorage.WriteToCsvFile(item, fileName);
                }
                //email results to myself
                var subject = $"You have {filteredItems.Count} item(s) for review";
                new AwsEmail(subject, filteredItems, null, null).SendEmail();
            }
        }

        public List<Product> Shop(string url)
        {
            var products = new List<Product>();
            decimal Price = 0M;
            string Title = string.Empty;
            string Location = string.Empty;
            string Url = string.Empty;
            DateTime? Date = null;

            var web = _htmlDoc.Load(url);

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
                    Title = child.Descendants().Where(x => x.Attributes.Contains("href")).First().InnerText;
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
    }
}
