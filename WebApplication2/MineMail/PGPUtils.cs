using System.Collections.Generic;
using System.IO;
using System.Linq;
using MimeKit;
using MimeKit.Cryptography;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace WebApplication2.MineMail
{
    public class PGPUtils
    {
        public void TraversingAMessage(MimeMessage message)
        {
            var attachments = new List<MimePart>();
            var multiparts = new List<Multipart>();

            using (var iter = new MimeIterator(message))
            {
                // collect our list of attachments and their parent multiparts
                while (iter.MoveNext())
                {
                    var multipart = iter.Parent as Multipart;
                    var part = iter.Current as MimePart;

                    if (multipart != null && part != null && part.IsAttachment)
                    {
                        // keep track of each attachment's parent multipart
                        multiparts.Add(multipart);
                        attachments.Add(part);
                    }
                }
            }

            // now remove each attachment from its parent multipart...
            for (int i = 0; i < attachments.Count; i++)
                multiparts[i].Remove(attachments[i]);
        }

        public static MimeMessage Encrypt(MimeMessage message)
        {
            // encrypt our message body using our custom GnuPG cryptography context
            using (var ctx = new MyGnuPGContext())
            {
                // Note: this assumes that each of the recipients has a PGP key associated
                // with their email address in the user's public keyring.
                // 
                // If this is not the case, you can use SecureMailboxAddresses instead of
                // normal MailboxAddresses which would allow you to specify the fingerprint
                // of their PGP keys. You could also choose to use one of the Encrypt()
                // overloads that take a list of PgpPublicKeys.
                message.Body = MultipartEncrypted.Encrypt(ctx, message.To.Mailboxes, message.Body);

                return message;
            }
        }

        public MimeEntity Decrypt(MimeMessage message, MimeEntity entity)
        {
            if (message.Body is MultipartEncrypted)
            {
                // the top-level MIME part of the message is encrypted using PGP/MIME
                var encrypted = (MultipartEncrypted)entity;

                return encrypted.Decrypt();
            }
            else
            {
                // the top-level MIME part is not encrypted
                return message.Body;
            }
        }

        public void Sign(MimeMessage message, PgpSecretKey key)
        {
            // digitally sign our message body using our custom GnuPG cryptography context
            using (var ctx = new MyGnuPGContext())
            {
                message.Body = MultipartSigned.Create(ctx, key, DigestAlgorithm.Sha1, message.Body);
            }
        }

        public static MimeMessage Sign(MimeMessage message)
        {
            // digitally sign our message body using our custom GnuPG cryptography context
            using (var ctx = new MyGnuPGContext())
            {
                // Note: this assumes that the Sender address has an S/MIME signing certificate
                // and private key with an X.509 Subject Email identifier that matches the
                // sender's email address.
                // 
                // If this is not the case, you can use a SecureMailboxAddress instead of a
                // normal MailboxAddress which would allow you to specify the fingerprint
                // of the sender's private PGP key. You could also choose to use one of the
                // Create() overloads that take a PgpSecretKey, instead.
                var sender = message.From.Mailboxes.FirstOrDefault();

                message.Body = MultipartSigned.Create(ctx, sender, DigestAlgorithm.Sha1, message.Body);

                return message;
            }
        }

        public void Render(MimeMessage message)
        {
            var tmpDir = Path.Combine(Path.GetTempPath(), message.MessageId);
            var visitor = new HtmlPreviewVisitor(tmpDir);

            Directory.CreateDirectory(tmpDir);

            message.Accept(visitor);
        }

        public void Verify(MimeMessage message)
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
    }
}