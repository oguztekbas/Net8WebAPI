using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
    public class EmailHelper
    {
        public static async Task SendMail(string emailText)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("");

            //receiver email adress
            mailMessage.To.Add("");

            //subject of the email
            mailMessage.Subject = "RabbitMQ Mesajı";

            //attach the file
            mailMessage.Body = emailText;
            mailMessage.IsBodyHtml = true;

            //SMTP client
            SmtpClient smtpClient = new SmtpClient("smtp.office365.com");
            //port number for Hot mail
            smtpClient.Port = 587;
            //credentials to login in to hotmail account
            smtpClient.Credentials = new NetworkCredential("", "");
            //enabled SSL
            smtpClient.EnableSsl = true;
            //Send an email
            await smtpClient.SendMailAsync(mailMessage);
        }

    }
}
