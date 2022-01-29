using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPenguin.Shared
{
    public static class Utils
    {
        public static void SendEmail(string subject, string body, string to = "4027304808@vtext.com", string from = "ungusc73@gmail.com")
        {
            var message = new MailMessage();
            message.From = new MailAddress(from);

            message.To.Add(new MailAddress(to));//See carrier destinations below

            message.Subject = subject;
            message.Body = body;

            var client = new SmtpClient();
            client.Send(message);
        }
    }
}
