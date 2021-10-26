using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static System.Console;

namespace Shopper
{
    public class CsvStorage
    {
        public static List<Product> ProcessFile(string path)
        {
            var query =
                 File.ReadAllLines(path)
                .Skip(1)
                .Where(l => l.Length > 1)
                .ToProduct();
            return query.ToList();
        }
        public static void CheckForCsvFile(string path)
        {
            if (!File.Exists(path))
            {
                var csv = new StringBuilder();
                WriteLine("Making a new csv file");
                csv.AppendLine("Title,Price,Location,ProductDate,Link");
                File.WriteAllText(path, csv.ToString());
                csv.Clear();
            }
        }
        public static void WriteToCsvFile(Product product, string path)
        {
            var csv = new StringBuilder();
            WriteLine("Adding item to file: " + product.Title.ToString());
            var newLine = string.Format($"{product.Title},{product.Price},{product.Location},{product.ProductDate},{product.Link}");
            csv.AppendLine(newLine);
            File.AppendAllText(path, csv.ToString());
            csv.Clear();
        }
        public static (string searchterm, string url) CheckForSearchCriteria()
        {
            if (!File.Exists("SearchCriteria.csv"))
            {
                var csv = new StringBuilder();
                WriteLine("Making a new csv file for Search Criteria");
                csv.AppendLine("Search Term,URL");
                csv.AppendLine("mountain,https://brainerd.craigslist.org/d/bicycles/search/bia?postal=56425&search_distance=50");
                File.WriteAllText("SearchCriteria.csv", csv.ToString());
                csv.Clear();
            }
            return GetSearchCriteria();
        }
        public static (string searchterm, string url) GetSearchCriteria()
        {
            var query = File.ReadAllLines("SearchCriteria.csv").Skip(1).Where(l => l.Length > 1).Last().Split(',');
            return (searchterm: query[0], url: query[1]);
        }
        public static void WriteSearchCriteriaFile(string criteria)
        {
            var csv = new StringBuilder();
            csv.AppendLine(criteria);
            File.AppendAllText("SearchCriteria.csv", csv.ToString());
            csv.Clear();
        }
    }
}
