using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Data.Common
{
    public class Mailing
    {
        public void SendEmail(string to, string subject, string body)
        {
            string fromMail = "tiendads3@gmail.com"; // Mail
            string fromPassword = "zxtmhqsjwwwwhisz"; // Shared password from GMAIL

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage(fromMail, to, subject, body);
            mailMessage.IsBodyHtml = true;

            smtpClient.Send(mailMessage);
        }
    }
}
