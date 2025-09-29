using System.Net.Mail;
using System.Net;
using RealState.DAL.Models.Identity;

namespace RealState.BLL.Common.Services.EmailSettings
{
    public class EmailSettings : IEmailSettings
    {
        public void SendEmail(Email email)
        {
            var Client = new SmtpClient("smtp.gmail.com", 587);
            Client.EnableSsl = true;
            //Sender -Reciver
            //
            Client.Credentials = new NetworkCredential("fadywageih14@gmail.com", "optqpppnxitbxxdj");
            Client.Send("fadywageih14@gmail.com", email.To, email.Subject, email.Body);
        }
    }
}
