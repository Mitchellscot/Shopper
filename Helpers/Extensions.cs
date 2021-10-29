using System;
using System.Collections.Generic;

namespace shopper
{
    public static class Extensions
    {
        public static IEnumerable<Product> ToProduct(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(',');
                yield return new Product()
                {
                    Title = columns[0],
                    Price = Convert.ToDecimal(columns[1]),
                    Location = columns[2],
                    ProductDate = DateTime.Parse(columns[3]),
                    Link = columns[4]
                };
            }
        }
    }
}
