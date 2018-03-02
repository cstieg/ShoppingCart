using Cstieg.Sales.Models;
using RazorEngine;
using RazorEngine.Templating;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Cstieg.Sales
{
    public partial class ShoppingCartService
    {
        public async Task SendConfirmationEmailAsync(Order order, string baseUrl, string templatePath) 
        {
            string body = await RenderOrderSuccessEmailBodyAsync(order, baseUrl, templatePath);
            MailMessage message = GetOrderSuccessEmailMessage(order, body);
            await SendEmail(message);
        }

        private async Task<string> RenderOrderSuccessEmailBodyAsync(Order order, string baseUrl, string templatePath)
        {
            // Add baseURL for images to viewBag
            var viewBag = new DynamicViewBag();
            viewBag.AddValue("baseURL", baseUrl);

            // render email body
            var sr = new StreamReader(templatePath);
            string body = Engine.Razor.RunCompile(await sr.ReadToEndAsync(), "OrderSuccessEmail", null, order, viewBag);

            return body;
        }

        private MailMessage GetOrderSuccessEmailMessage(Order order, string body)
        {
            var message = new MailMessage
            {
                Subject = "Order confirmation - " + order.Description,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(order.Customer.EmailAddress));

            return message;
        }

        private async Task SendEmail(MailMessage message)
        {
            using (var smtp = new SmtpClient())
            {
                // get sender account from web.config - system.net
                string from = ((NetworkCredential)smtp.Credentials).UserName;
                message.From = new MailAddress(from);

                await smtp.SendMailAsync(message);
            }
        }
    }
}
