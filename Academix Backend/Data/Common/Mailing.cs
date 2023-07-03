using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Data.Entities;

namespace Data.Common
{
    public class Mailing
    {
        public void SendEmail(string to, string subject, string body, string title)
        {
            string fromMail = "tiendads3@gmail.com"; // Mail
            string fromPassword = "zxtmhqsjwwwwhisz"; // Shared password from GMAIL

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };

            string templatePath = "";

            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\", "site\\wwwroot\\Templates\\MailLayout.html"))) templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\", "site\\wwwroot\\Templates\\MailLayout.html");
            else templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates\\MailLayout.html");

            string template = System.IO.File.ReadAllText(templatePath);
            
            template = Regex.Replace(template, "{{titulo}}", title);
            template = Regex.Replace(template, "{{cuerpo}}", body);

            var mailMessage = new MailMessage(fromMail, to, subject, template);
            mailMessage.IsBodyHtml = true;

            smtpClient.Send(mailMessage);
        }
    }
}
