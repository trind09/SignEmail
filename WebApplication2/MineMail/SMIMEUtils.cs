﻿using MimeKit;
using MimeKit.Cryptography;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace WebApplication2.MineMail
{
    public class SMIMEUtils
    {
        public void Encrypt(MimeMessage message)
        {
            // encrypt our message body using our custom S/MIME cryptography context
            using (var ctx = new MySecureMimeContext())
            {
                // Note: this assumes that each of the recipients has an S/MIME certificate
                // with an X.509 Subject Email identifier that matches their email address.
                // 
                // If this is not the case, you can use SecureMailboxAddresses instead of
                // normal MailboxAddresses which would allow you to specify the fingerprint
                // of their certificates. You could also choose to use one of the Encrypt()
                // overloads that take a list of CmsRecipients, instead.
                message.Body = ApplicationPkcs7Mime.Encrypt(ctx, message.To.Mailboxes, message.Body);
            }
        }

        public MimeEntity Decrypt(MimeMessage message)
        {
            var pkcs7 = message.Body as ApplicationPkcs7Mime;

            if (pkcs7 != null && pkcs7.SecureMimeType == SecureMimeType.EnvelopedData)
            {
                // the top-level MIME part of the message is encrypted using S/MIME
                return pkcs7.Decrypt();
            }
            else
            {
                // the top-level MIME part is not encrypted
                return message.Body;
            }
        }

        public void MultipartSign(MimeMessage message)
        {
            // digitally sign our message body using our custom S/MIME cryptography context
            using (var ctx = new MySecureMimeContext())
            {
                // Note: this assumes that the Sender address has an S/MIME signing certificate
                // and private key with an X.509 Subject Email identifier that matches the
                // sender's email address.
                var sender = message.From.Mailboxes.FirstOrDefault();

                message.Body = MultipartSigned.Create(ctx, sender, DigestAlgorithm.Sha1, message.Body);
            }
        }

        public void MultipartSign(MimeMessage message, X509Certificate2 certificate)
        {
            // digitally sign our message body using our custom S/MIME cryptography context
            using (var ctx = new MySecureMimeContext())
            {
                var signer = new CmsSigner(certificate)
                {
                    DigestAlgorithm = DigestAlgorithm.Sha1
                };

                message.Body = MultipartSigned.Create(ctx, signer, message.Body);
            }
        }

        public void Pkcs7Sign(MimeMessage message)
        {
            // digitally sign our message body using our custom S/MIME cryptography context
            using (var ctx = new MySecureMimeContext())
            {
                // Note: this assumes that the Sender address has an S/MIME signing certificate
                // and private key with an X.509 Subject Email identifier that matches the
                // sender's email address.
                var sender = message.From.Mailboxes.FirstOrDefault();

                message.Body = ApplicationPkcs7Mime.Sign(ctx, sender, DigestAlgorithm.Sha1, message.Body);
            }
        }

        public void VerifyMultipartSigned(MimeMessage message)
        {
            if (message.Body is MultipartSigned)
            {
                var signed = (MultipartSigned)message.Body;

                foreach (var signature in signed.Verify())
                {
                    try
                    {
                        bool valid = signature.Verify();

                        // If valid is true, then it signifies that the signed content
                        // has not been modified since this particular signer signed the
                        // content.
                        // 
                        // However, if it is false, then it indicates that the signed
                        // content has been modified.
                    }
                    catch (DigitalSignatureVerifyException)
                    {
                        // There was an error verifying the signature.
                    }
                }
            }
        }

        public void VerifyPkcs7(MimeMessage message)
        {
            var pkcs7 = message.Body as ApplicationPkcs7Mime;

            if (pkcs7 != null && pkcs7.SecureMimeType == SecureMimeType.SignedData)
            {
                // extract the original content and get a list of signatures
                MimeEntity original;

                // Note: if you are rendering the message, you'll want to render the
                // original mime part rather than the application/pkcs7-mime part.
                foreach (var signature in pkcs7.Verify(out original))
                {
                    try
                    {
                        bool valid = signature.Verify();

                        // If valid is true, then it signifies that the signed content
                        // has not been modified since this particular signer signed the
                        // content.
                        // 
                        // However, if it is false, then it indicates that the signed
                        // content has been modified.
                    }
                    catch (DigitalSignatureVerifyException)
                    {
                        // There was an error verifying the signature.
                    }
                }
            }
        }
    }
}