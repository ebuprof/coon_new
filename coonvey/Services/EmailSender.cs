using MimeKit;
using MimeKit.Utils;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace coonvey.Services
{
    public class EmailSender : IEmailSender
    {
        public EmailSender()
        { }

        public async Task SendEmailAsync(string email, string subject, string message)
        {

            // Plug in your email service here to send an email.
            try
            {
                //return Task.FromResult(0);
                // Credentials:
                var sentFrom = "coonvey@gmail.com";
                var pwd = "c3@@o0n4v2e5y5";
                var emailMessage = new MimeMessage();

                emailMessage.From.Add(new MailboxAddress("Coonvey", sentFrom));
                emailMessage.To.Add(new MailboxAddress("", email));
                emailMessage.Subject = subject;

                var builder = new BodyBuilder();
                string webPath = HostingEnvironment.ApplicationPhysicalPath; //_env.WebRootPath;
                var image = builder.LinkedResources.Add(webPath + @"Content\assets\img\logo.png");
                image.ContentId = MimeUtils.GenerateMessageId();
                builder.HtmlBody = string.Format(@"<left><img src=""cid:{0}""></left><br><p>{1}</p>", image.ContentId, message);

                emailMessage.Body = builder.ToMessageBody();

                using (var client = new SmtpClient())
                {

                    await client.ConnectAsync("smtp.gmail.com", 587, false); //465
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(sentFrom, pwd);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}