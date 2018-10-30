using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Cryptography;
using MimeKit.Utils;
using System;
using System.Net.Mail;
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

                SendMessage(message);
            } catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        protected void btnSendSignedEmail_Click(object sender, EventArgs e)
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

                message = SignMessage(message);

                SendMessage(message);
            } catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        private void SendMessage(MimeMessage message)
        {
            //Send message using MailKit
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect("smtp.gmail.com", 465, true);
                client.Authenticate("hellomeonet@gmail.com", "P@ssword123!");
                client.Send(message);
                client.Disconnect(true);
                lblMessage.Text = "Sent successful!";
            }
        }

        private MimeMessage SignMessage(MimeMessage message)
        {
            HeaderId[] headersToSign = new HeaderId[] { HeaderId.From, HeaderId.Subject, HeaderId.Date };

            string domain = "example.net";
            string selector = "brisbane";

            DkimSigner signer = new DkimSigner("C:\\my-dkim-key.pem", domain, selector)
            {
                SignatureAlgorithm = DkimSignatureAlgorithm.RsaSha1,
                AgentOrUserIdentifier = "@eng.example.com",
                QueryMethod = "dns/txt",
            };

            // Prepare the message body to be sent over a 7bit transport (such as 
            // older versions of SMTP). This is VERY important because the message
            // cannot be modified once we DKIM-sign our message!
            //
            // Note: If the SMTP server you will be sending the message over 
            // supports the 8BITMIME extension, then you can use
            // `EncodingConstraint.EightBit` instead.
            message.Prepare(EncodingConstraint.SevenBit);

            message.Sign(signer, headersToSign,
                DkimCanonicalizationAlgorithm.Relaxed,
                DkimCanonicalizationAlgorithm.Simple);
            return message;
        }
    }
}