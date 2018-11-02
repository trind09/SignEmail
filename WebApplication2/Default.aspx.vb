Imports MailKit.Net.Smtp
Imports MimeKit
Imports MimeKit.Cryptography
Imports MimeKit.Utils

Partial Public Class _Default
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
    End Sub

    Protected Sub btnSendEmail_Click(ByVal sender As Object, ByVal ea As EventArgs)
        Try
            Dim message = New MimeMessage()
            message.From.Add(New MailboxAddress("Joey", txtFromEmail.Text))
            message.[To].Add(New MailboxAddress("Alice", txtToEmail.Text))
            message.Subject = "How you doin?"
            Dim builder = New BodyBuilder()
            builder.TextBody = "Hey Alice,

        What are you up to this weekend? Monica is throwing one of her parties on
        Saturday and I was hoping you could make it.

        Will you be my +1?

        -- Joey
        "
            Dim image = builder.LinkedResources.Add(System.Web.Hosting.HostingEnvironment.MapPath("~/TempFiles/image.png"))
            image.ContentId = MimeUtils.GenerateMessageId()
            builder.HtmlBody = String.Format("<p>Hey Alice,<br>
        <p>What are you up to this weekend? Monica is throwing one of her parties on
        Saturday and I was hoping you could make it.<br>
        <p>Will you be my +1?<br>
        <p>-- Joey<br>
        <center><img src=""cid:{0}""></center>", image.ContentId)
            builder.Attachments.Add(System.Web.Hosting.HostingEnvironment.MapPath("~/TempFiles/msg.msg"))
            message.Body = builder.ToMessageBody()
            SendMessage(message)
        Catch ex As Exception
            lblMessage.Text = ex.Message
        End Try
    End Sub

    Protected Sub btnSendSignedEmail_Click(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim message = New MimeMessage()
            message.From.Add(New MailboxAddress("Joey", txtFromEmail.Text))
            message.[To].Add(New MailboxAddress("Alice", txtToEmail.Text))
            message.Subject = "How you doin?"
            Dim builder = New BodyBuilder()
            builder.TextBody = "Hey Alice,

        What are you up to this weekend? Monica is throwing one of her parties on
        Saturday and I was hoping you could make it.

        Will you be my +1?

        -- Joey
        "
            Dim image = builder.LinkedResources.Add(System.Web.Hosting.HostingEnvironment.MapPath("~/TempFiles/image.png"))
            image.ContentId = MimeUtils.GenerateMessageId()
            builder.HtmlBody = String.Format("<p>Hey Alice,<br>
        <p>What are you up to this weekend? Monica is throwing one of her parties on
        Saturday and I was hoping you could make it.<br>
        <p>Will you be my +1?<br>
        <p>-- Joey<br>
        <center><img src=""cid:{0}""></center>", image.ContentId)
            builder.Attachments.Add(System.Web.Hosting.HostingEnvironment.MapPath("~/TempFiles/msg.msg"))
            message.Body = builder.ToMessageBody()
            message = SignMessage(message)
            SendMessage(message)
        Catch ex As Exception
            lblMessage.Text = ex.Message
        End Try
    End Sub

    Private Sub SendMessage(ByVal message As MimeMessage)
        Using client = New SmtpClient()
            client.Connect("smtp.gmail.com", 465, True)
            client.Authenticate("hellomeonet@gmail.com", "P@ssword123!")
            client.Send(message)
            client.Disconnect(True)
            lblMessage.Text = "Sent successful!"
        End Using
    End Sub

    Private Function SignMessage(ByVal message As MimeMessage) As MimeMessage
        Dim headersToSign As HeaderId() = New HeaderId() {HeaderId.From, HeaderId.Subject, HeaderId.Date}
        Dim privateKey As String = System.Web.Hosting.HostingEnvironment.MapPath("~/TempFiles/privatekey.pem")
        Dim domain As String = "gmail.com"
        Dim selector As String = "1540894865.gmail"
        Dim signer As DkimSigner = New DkimSigner(privateKey, domain, selector) With {
            .SignatureAlgorithm = DkimSignatureAlgorithm.RsaSha1,
            .AgentOrUserIdentifier = "@gmail.com",
            .QueryMethod = "dns/txt"
        }
        message.Prepare(EncodingConstraint.SevenBit)
        message.Sign(signer, headersToSign, DkimCanonicalizationAlgorithm.Relaxed, DkimCanonicalizationAlgorithm.Simple)
        Return message
    End Function
End Class
