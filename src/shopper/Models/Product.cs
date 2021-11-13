using System;

namespace shopper.Models
{
    public class Product
    {
        public string Title { get; set; }
        public DateTime ProductDate { get; set; }
        public decimal Price { get; set; }
        public string Location { get; set; }
        public string Link { get; set; }
    }
}
