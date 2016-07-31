using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace Durados.Cms.DataAccess
{
    public class Email
    {
        public Email()
        {
        }

        static SmtpClient smtpClient = null;

        private static SmtpClient GetSmtp(string host, bool useDefaultCredentials, string username, string password, int port, bool sync)
        {
            if (!sync || smtpClient == null)
            {
                smtpClient = new SmtpClient(host);

                smtpClient.UseDefaultCredentials = false;
                if (!useDefaultCredentials)
                {
                    System.Net.NetworkCredential cred = new System.Net.NetworkCredential(username, password);
                    smtpClient.Credentials = cred;
                }

                smtpClient.Port = port;

                smtpClient.SendCompleted += new SendCompletedEventHandler(smtpClient_SendCompleted);
            }
            return smtpClient;
        }

        static void smtpClient_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

        }

        public static void Send(string host, bool useDefaultCredentials, int port, string username, string password, bool useSsl, string[] to, string[] cc, string[] bcc, string subject, string message, string fromEmail, string fromNick, string anonymousEmail, bool dontSend, Durados.Diagnostics.ILogger logger)
        {
            Send(host, useDefaultCredentials, port, username, password, useSsl, to, cc, bcc, subject, message, fromEmail, fromNick, anonymousEmail, dontSend, new string[0], logger);
        }

        public static void Send(string host, bool useDefaultCredentials, int port, string username, string password, bool useSsl, string[] to, string[] cc, string[] bcc, string subject, string message, string fromEmail, string fromNick, string anonymousEmail, bool dontSend, string[] files, Durados.Diagnostics.ILogger logger)
        {
            
            System.Threading.Tasks.Task.Run(() =>Send(host, useDefaultCredentials, port, username, password, useSsl, to, cc, bcc, subject, message, fromEmail, fromNick, anonymousEmail, dontSend, files, logger, false));

        }

        public static void Send(string host, bool useDefaultCredentials, int port, string username, string password, bool useSsl, string[] to, string[] cc, string[] bcc, string subject, string message, string fromEmail, string fromNick, string anonymousEmail, bool dontSend, string[] files, Durados.Diagnostics.ILogger logger, bool sync)
        {
            if (logger != null)
                logger.Log("Email", "Send", "start", null, 150, "");
            if (dontSend)
                return;

            MailMessage mailMessage = new MailMessage();

            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = true;
            mailMessage.Subject = subject;

            if (bcc != null)
            {
                foreach (string bccMail in bcc)
                {
                    if (!string.IsNullOrEmpty(bccMail))
                        mailMessage.Bcc.Add(new MailAddress(bccMail));
                }
            }

            if (to != null)
            {
                foreach (string toMail in to)
                {
                    if (!string.IsNullOrEmpty(toMail))
                        mailMessage.To.Add(new MailAddress(toMail));
                }
            }

            if (cc != null)
            {
                foreach (string ccMail in cc)
                {
                    if (!string.IsNullOrEmpty(ccMail))
                        mailMessage.CC.Add(new MailAddress(ccMail));
                }
            }

            if (!string.IsNullOrEmpty(fromEmail))
            {
                if (string.IsNullOrEmpty(fromNick))
                {
                    mailMessage.From = new MailAddress(fromEmail);
                }
                else
                {
                    mailMessage.From = new MailAddress(fromEmail, fromNick);
                }
            }
            else
            {
                mailMessage.From = new MailAddress(anonymousEmail, "Anonymous");

            }

            if (files != null)
            {
                foreach (string file in files)
                {
                    if (!string.IsNullOrEmpty(file))
                    {
                        mailMessage.Attachments.Add(new Attachment(file));
                    }
                }
            }
            if (logger != null)
                logger.Log("Email", "Send", "before get smtp", null, 150, "");

            SmtpClient smtp = null;

            try
            {
                smtp = GetSmtp(host, useDefaultCredentials, username, password, port, sync);
                if (logger != null)
                    logger.Log("Email", "Send", "success in get smtp", null, 150, "");

            }
            catch (Exception exception)
            {
                if (logger != null)
                    logger.Log("Email", "Send", "get smtp", exception, 1, "");
            }

            smtp.EnableSsl = useSsl;

            try
            {
                if (sync)
                    smtp.Send(mailMessage);
                else
                    smtp.SendAsync(mailMessage, null);
                if (logger != null)
                    logger.Log("Email", "Send", "succes in send", null, 150, "");
            }
            catch (Exception exception)
            {
                if (logger != null)
                    logger.Log("Email", "Send", sync ? "sync" : "async", exception, 1, "");
            }



        }
    }
}
