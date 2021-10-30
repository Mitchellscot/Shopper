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
        private readonly string _subject;

        public string TextBody { get; set; }
        public string HtmlBody { get; set; }
        public AwsEmail(string subject, List<Product> items, CsvStorage data)
        {
            _data = data;
            _subject = subject;
            this.CreateEmail(items);
        }

        private void CreateEmail(List<Product> items)
        {
            var _stringBuilder = new StringBuilder();
            _stringBuilder.AppendLine("<h3>Here is the latest...</h3>");
            _stringBuilder.AppendLine("<table><thead><tr><th><h4>Title</h4></th><th><h4>Price</h4></th><th><h4>Location</h4></th><th><h4>Date</h4></th></tr></thead><tbody>");
            foreach (var item in items)
            {
                _stringBuilder.AppendLine($"<tr><td><a href=\"{item.Link}\">{item.Title}</a></td><td>${item.Price}</td><td>{item.Location}</td><td>{item.ProductDate.ToShortDateString()}</td></tr>");
            }
            _stringBuilder.AppendLine("</tbody>");
            this.HtmlBody = _stringBuilder.ToString();
            _stringBuilder.Clear();
            _stringBuilder.AppendLine("Here is the latest...");
            foreach (var item in items)
            {
                _stringBuilder.AppendLine($"Title - {item.Title}");
                _stringBuilder.AppendLine($"Price - {item.Price}");
                _stringBuilder.AppendLine($"Location - {item.Location}");
                _stringBuilder.AppendLine($"Date - {item.ProductDate}");
                _stringBuilder.AppendLine($"Link - {item.Link}");
                _stringBuilder.AppendLine("---------------------");
            }
            this.TextBody = _stringBuilder.ToString();
            _stringBuilder.Clear();
        }

        public async Task SendEmail()
        {
            using (var client = new AmazonSimpleEmailServiceClient(Amazon.RegionEndpoint.USEast2))
            {
                var sendRequest = new SendEmailRequest
                {
                    Source = _data.GetSettingsFromFile().Result.FromAddress,
                    Destination = new Destination
                    {
                        ToAddresses = new List<string> { _data.GetSettingsFromFile().Result.ToAddress }
                    },
                    Message = new Message
                    {
                        Subject = new Content(_subject),
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
