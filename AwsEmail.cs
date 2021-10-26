using System;
using System.Collections.Generic;
using System.Text;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Logging;
using static System.Console;

namespace Shopper
{
    public class AwsEmail
    {
        const string SENDING = "mitchellscott@me.com";
        const string RECIEVING = "mittscotchell@gmail.com";
        private readonly ILogger<AwsEmail> _logger;
        private readonly string _subject;
        private readonly StringBuilder _stringBuilder;

        public string TextBody { get; set; }
        public string HtmlBody { get; set; }
        public AwsEmail(string subject, List<Product> items, ILogger<AwsEmail> logger, StringBuilder stringBuilder)
        {
            _logger = logger;
            _subject = subject;
            this.CreateEmail(items);
            _stringBuilder = stringBuilder;

        }

        private void CreateEmail(List<Product> items)
        {
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

        public async void SendEmail()
        {
            using (var client = new AmazonSimpleEmailServiceClient(Amazon.RegionEndpoint.USEast2))
            {
                var sendRequest = new SendEmailRequest
                {
                    Source = SENDING,
                    Destination = new Destination
                    {
                        ToAddresses = new List<string> { RECIEVING }
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
                    _logger.LogInformation($"Preparing to send an email...");
                    var response = await client.SendEmailAsync(sendRequest);
                    _logger.LogInformation($"Email sent! {response.HttpStatusCode}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"HEY MITCH - ERROR SENDING EMAIL {ex.Message}");
                }
            }
        }

    }
}
