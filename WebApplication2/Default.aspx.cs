using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Utils;
using System;
using System.Web.UI;
using WebApplication2.MineMail;

namespace WebApplication2
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSendEmail_Click(object sender, EventArgs ea)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Joey", txtFromEmail.Text));
                message.To.Add(new MailboxAddress("Alice", txtToEmail.Text));
                message.Subject = "How you doin?";

                var builder = new BodyBuilder();

                // Set the plain-text version of the message text
                builder.TextBody = @"Hey Alice,

            What are you up to this weekend? Monica is throwing one of her parties on
            Saturday and I was hoping you could make it.

            Will you be my +1?

            -- Joey
            ";

                // In order to reference selfie.jpg from the html text, we'll need to add it
                // to builder.LinkedResources and then use its Content-Id value in the img src.
                var image = builder.LinkedResources.Add(System.Web.Hosting.HostingEnvironment.MapPath("~/TempFiles/image.png"));
                image.ContentId = MimeUtils.GenerateMessageId();

                // Set the html version of the message text
                builder.HtmlBody = string.Format(@"<p>Hey Alice,<br>
            <p>What are you up to this weekend? Monica is throwing one of her parties on
            Saturday and I was hoping you could make it.<br>
            <p>Will you be my +1?<br>
            <p>-- Joey<br>
            <center><img src=""cid:{0}""></center>", image.ContentId);

                // We may also want to attach a calendar event for Monica's party...
                builder.Attachments.Add(System.Web.Hosting.HostingEnvironment.MapPath("~/TempFiles/msg.msg"));

                // now set the multipart/mixed as the message body
                message.Body = builder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);

                    client.Authenticate("hellomeonet@gmail.com", "P@ssword123!");

                    client.Send(message);

                    client.Disconnect(true);

                    lblMessage.Text = "Sent successful!";
                }
            } catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        protected void btnSendSignEmail_Click(object sender, EventArgs ea)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Joey", txtFromEmail.Text));
                message.To.Add(new MailboxAddress("Alice", txtToEmail.Text));
                message.Subject = "How you doin?";

                var builder = new BodyBuilder();

                // Set the plain-text version of the message text
                builder.TextBody = @"Hey Alice,

            What are you up to this weekend? Monica is throwing one of her parties on
            Saturday and I was hoping you could make it.

            Will you be my +1?

            -- Joey
            ";

                // In order to reference selfie.jpg from the html text, we'll need to add it
                // to builder.LinkedResources and then use its Content-Id value in the img src.
                var image = builder.LinkedResources.Add(System.Web.Hosting.HostingEnvironment.MapPath("~/TempFiles/image.png"));
                image.ContentId = MimeUtils.GenerateMessageId();

                // Set the html version of the message text
                builder.HtmlBody = string.Format(@"<p>Hey Alice,<br>
            <p>What are you up to this weekend? Monica is throwing one of her parties on
            Saturday and I was hoping you could make it.<br>
            <p>Will you be my +1?<br>
            <p>-- Joey<br>
            <center><img src=""cid:{0}""></center>", image.ContentId);

                // We may also want to attach a calendar event for Monica's party...
                builder.Attachments.Add(System.Web.Hosting.HostingEnvironment.MapPath("~/TempFiles/msg.msg"));

                // now set the multipart/mixed as the message body
                message.Body = builder.ToMessageBody();

                message = PGPUtils.Sign(message);

                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);

                    client.Authenticate("hellomeonet@gmail.com", "P@ssword123!");

                    client.Send(message);

                    client.Disconnect(true);

                    lblMessage.Text = "Sent successful!";
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }
    }
}