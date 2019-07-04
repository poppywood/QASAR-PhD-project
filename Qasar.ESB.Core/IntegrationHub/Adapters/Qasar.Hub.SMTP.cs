using System;
using System.Data;
using System.Configuration;

namespace Qasar.ESB.Adapter
{
    /// <summary>
    /// SMTP Adapter
    /// </summary>
    public class SMTPAdapter
    {
        public SMTPAdapter()
        {
        }
        public void Submit(string from, string to, string subject, string message)
        {
            System.Net.Mail.MailMessage mailmessage = new System.Net.Mail.MailMessage(from, to, subject, message);
            System.Net.Mail.SmtpClient mailClient =
                new System.Net.Mail.SmtpClient(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SmtpClient"]));
            mailClient.Send(mailmessage);
        }
    }
}
