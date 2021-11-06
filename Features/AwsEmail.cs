using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using shopper.Data;
using shopper.Models;
using static System.Console;

namespace shopper.Features
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
            _stringBuilder.AppendLine(EmailTop);

            foreach (var product in products.OrderByDescending(x=> x.ProductDate))
            {
                _stringBuilder.AppendLine($"<tr style=\"\" bgcolor=\"#f2f2f2\"><td class=\"text-decoration-none text-center\" style=\"line-height: 24px; font-size: 16px; margin: 0;padding: 12px; border: 1px solid #e2e8f0;\" align=\"center\" valign=\"top\">" + 
                    $"<a href =\"{product.Link}\" style=\"color: #0d6efd;\">" +
                    $"{product.Title}" +
                    "</a></td><td class=\"text-center\" style=\"line-height: 24px; font-size: 16px; margin: 0; padding: 12px; border: 1px solid #e2e8f0;\" align=\"center\" valign=\"top\"><table class=\"badge rounded-pill bg-success\" align=\"left\" role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"\" bgcolor=\"#198754\"><tbody><tr><td style = \"line-height: 1; font-size: 75%; display: inline-block; font-weight: 700; white-space: nowrap; border-radius: 4px; margin: 0; padding: 4px 6.4px;\" align=\"center\" bgcolor=\"#198754\" valign=\"baseline\"><span>$" +
                       $"{ product.Price }" +
            "</span></td></tr></tbody></table></td><td class=\"text-center\" style=\"line-height: 24px; font-size: 16px; margin: 0; padding: 12px; border: 1px solid #e2e8f0;\" align=\"center\" valign=\"top\">" +
            $"{product.Location}" +
            "</td><td class=\"text-center\" style=\"line-height: 24px; font-size: 16px; margin: 0; padding: 12px; border: 1px solid #e2e8f0;\" align=\"center\" valign=\"top\">" +
            $"{ product.ProductDate.ToShortDateString()}" +
            "</td></tr>");

            }
            _stringBuilder.AppendLine(EmailEnd);
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
            using (var client=new AmazonSimpleEmailServiceClient(Amazon.RegionEndpoint.USEast2))
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
                        Subject=new Content(
                            _productCount> 1 ? $"You have {_productCount} items for review" : "There is a new item to review"),
                        Body=new Body
                        {
                            Html=new Content
                            {
                                Charset = "UTF-8",
                                Data = this.HtmlBody
                            },
                            Text=new Content
                            {
                                Charset = "UTF-8",
                                Data=this.TextBody
                            }
                        }
                    }
                };
                try
                {
                    var response=await client.SendEmailAsync(sendRequest);
                    WriteLine($"Email sent! {response.HttpStatusCode}");
                }
                catch (Exception ex)
                {
                    WriteLine($"HEY MITCH - ERROR SENDING EMAIL {ex.Message}");
                }
            }
        }
        private string EmailTop = "<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"http://www.w3.org/TR/REC-html40/loose.dtd\"><html><head><meta http-equiv=\"x-ua-compatible\" content=\"ie=edge\"><meta name=\"x-apple-disable-message-reformatting\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"><meta name=\"format-detection\" content=\"telephone=no, date=no, address=no, email=no\"><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"><style type=\"text/css\">body, table, td {font-family: Helvetica,Arial,sans-serif !important}.ExternalClass {width: 100%}.ExternalClass, .ExternalClass p, .ExternalClass span, .ExternalClass font, .ExternalClass td, .ExternalClass div {line-height: 150%}a {text-decoration: none}* {color: inherit}a[x-apple-data-detectors], u + #body a, #MessageViewBody a {color: inherit;text-decoration: none;font-size: inherit;font-family: inherit;font-weight: inherit;line-height: inherit}img {-ms-interpolation-mode: bicubic}table:not([class^=s-]) {font-family: Helvetica,Arial,sans-serif;mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-spacing: 0px;border-collapse: collapse}table:not([class^=s-]) td {border-spacing: 0px;border-collapse: collapse}@media screen and (max-width: 600px) {.w-full, .w-full > tbody > tr > td {width: 100% !important}*[class*=s-lg-] > tbody > tr > td {font-size: 0 !important;line-height: 0 !important;height: 0 !important}.s-5 > tbody > tr > td {font-size: 20px !important;line-height: 20px !important;: 20px !important}}</style></head><body class=\"bg-light\" style=\"outline: 0; width: 100%; min-width: 100%; height: 100%; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; font-family: Helvetica, Arial, sans-serif; line-height: 24px; font-weight: normal; font-size: 16px; -moz-box-sizing: border-box; -webkit-box-sizing: border-box; box-sizing: border-box; color: #000000; margin: 0; padding: 0; border: 0;\" bgcolor=\"#f7fafc\"><table class=\"bg-light body\" valign=\"top\" role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"outline: 0; width: 100%; min-width: 100%; height: 100%; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; font-family: Helvetica, Arial, sans-serif; line-height: 24px; font-weight: normal; font-size: 16px; -moz-box-sizing: border-box; -webkit-box-sizing: border-box; box-sizing: border-box; color: #000000; margin: 0; padding: 0; border: 0;\" bgcolor=\"#f7fafc\"><tbody><tr><td valign = \"top\" style=\"line-height: 24px; font-size: 16px; margin: 0;\" align=\"left\" bgcolor=\"#f7fafc\"><table class=\"container\" role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"width: 100%;\"><tbody><tr><td align = \"center\" style=\"line-height: 24px; font-size: 16px; margin: 0; padding: 0 16px;\"><table align = \"center\" role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"width: 100%; max-width: 600px; margin: 0 auto;\"><tbody><tr><td style = \"line-height: 24px; font-size: 16px; margin: 0;\" align=\"left\"><table class=\"s-5 w-full\" role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"width: 100%;\" width=\"100%\"><tbody><tr><td style = \"line-height: 20px; font-size: 20px; width: 100%; height: 20px; margin: 0;\" align=\"left\" width=\"100%\" height=\"20\">&#160;</td></tr></tbody></table><h1 class=\"text-center text-info font-monospace\" style=\"color: #0dcaf0; padding-top: 0; padding-bottom: 0; font-weight: 500; vertical-align: baseline; font-size: 36px; line-height: 43.2px; margin: 0;\" align=\"center\">Here is the latest...</h1><table class=\"s-5 w-full\" role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"width: 100%;\" width=\"100%\"><tbody><tr><td style = \"line-height: 20px; font-size: 20px; width: 100%; height: 20px; margin: 0;\" align=\"left\" width=\"100%\" height=\"20\">&#160;</td></tr></tbody></table><div class=\"table-responsive\"><table class=\"table table-striped table-bordered align-middle text-center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"width: 100%; max-width: 100%; text-align: center !important; border: 1px solid #e2e8f0;\"><thead class=\"table-light\" style=\"background-color: white;\"><tr><th scope = \"col\" class=\"text-center\" style=\"line-height: 24px; font-size: 16px; margin: 0; padding: 12px; border-color: #e2e8f0; border-style: solid; border-width: 1px 1px 2px;\" align=\"center\" valign=\"top\">Title</th><th scope = \"col\" class=\"text-center\" style=\"line-height: 24px; font-size: 16px; margin: 0; padding: 12px; border-color: #e2e8f0; border-style: solid; border-width: 1px 1px 2px;\" align=\"center\" valign=\"top\">Price</th><th scope = \"col\" class=\"text-center\" style=\"line-height: 24px; font-size: 16px; margin: 0; padding: 12px; border-color: #e2e8f0; border-style: solid; border-width: 1px 1px 2px;\" align=\"center\" valign=\"top\">Location</th><th scope = \"col\" class=\"text-center\" style=\"line-height: 24px; font-size: 16px; margin: 0; padding: 12px; border-color: #e2e8f0; border-style: solid; border-width: 1px 1px 2px;\" align=\"center\" valign=\"top\">Date</th></tr></thead><tbody class=\"table-light\" style=\"background-color: white;\">";


            private string EmailEnd ="</tbody></table></div></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></body></html>";

    }
}
