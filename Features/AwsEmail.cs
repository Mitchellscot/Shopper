using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shopper.Models;
using static System.Console;

namespace shopper
{
    public class AwsEmail
    {
        private readonly CsvStorage _data;
        private readonly int _productCount;

        public string TextBody { get; set; }
        public string HtmlBody { get; set; }
        public AwsEmail(CsvStorage data, List<Product> products)
        {
            _data = data;
            _productCount = products.Count;
            this.createEmail(products);
        }

        private void createEmail(List<Product> products)
        {
            var _stringBuilder = new StringBuilder();
            _stringBuilder.AppendLine("<h3>Here is the latest...</h3>");
            _stringBuilder.AppendLine("<table><thead><tr><th><h4>Title</h4></th><th><h4>Price</h4></th><th><h4>Location</h4></th><th><h4>Date</h4></th></tr></thead><tbody>");
            foreach (var product in products)
            {
                _stringBuilder.AppendLine($"<tr><td><a href=\"{product.Link}\">{product.Title}</a></td><td>${product.Price}</td><td>{product.Location}</td><td>{product.ProductDate.ToShortDateString()}</td></tr>");
            }
            _stringBuilder.AppendLine("</tbody>");
            this.HtmlBody = _stringBuilder.ToString();
            _stringBuilder.Clear();
            _stringBuilder.AppendLine("Here is the latest...");
            foreach (var product in products)
            {
                _stringBuilder.AppendLine($"Title - {product.Title}");
                _stringBuilder.AppendLine($"Price - {product.Price}");
                _stringBuilder.AppendLine($"Location - {product.Location}");
                _stringBuilder.AppendLine($"Date - {product.ProductDate}");
                _stringBuilder.AppendLine($"Link - {product.Link}");
                _stringBuilder.AppendLine("---------------------");
            }
            this.TextBody = _stringBuilder.ToString();
            _stringBuilder.Clear();
        }

        public async Task SendEmail()
        {
            (string from, string to) = (_data.GetSettingsFromFile().Result.FromAddress, _data.GetSettingsFromFile().Result.ToAddress);
            using (var client = new AmazonSimpleEmailServiceClient(Amazon.RegionEndpoint.USEast2))
            {
                var sendRequest = new SendEmailRequest
                {
                    Source = from.ToLower(),
                    Destination = new Destination
                    {
                        ToAddresses = new List<string> { to.ToLower() }
                    },
                    Message = new Message
                    {
                        Subject = new Content(
                            _productCount > 1 ? $"You have {_productCount} items for review" : "There is a new item to review"),
                        Body = new Body
                        {
                            Html = new Content
                            {
                                Charset = "UTF-8",
                                Data = this.HtmlBody
                            },
                            Text = new Content
                            {
                                Charset = "UTF-8",
                                Data = this.TextBody
                            }
                        }
                    }
                };
                try
                {
                    var response = await client.SendEmailAsync(sendRequest);
                    WriteLine($"Email sent! {response.HttpStatusCode}");
                }
                catch (Exception ex)
                {
                    WriteLine($"HEY MITCH - ERROR SENDING EMAIL {ex.Message}");
                }
            }
        }

    }
}
