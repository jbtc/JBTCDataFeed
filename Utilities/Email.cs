using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace Utilities
{
    public class Email
    {
        /// <summary>
        /// Sends the email with attachment.
        /// </summary>
        /// <param name="filenameAndLocation">The filename and location (if null or empty no attachment).</param>
        /// <param name="fromAddress">From address.</param>
        /// <param name="toAddress">To address.</param>
        /// <param name="emailSubject">The email subject.</param>
        /// <param name="emailBody">The email body.</param>
        /// <param name="smtServerString">The SMT server string.</param>
        /// <param name="smtpServerPort">The SMTP server port.</param>
        /// <param name="emailServerUserName">Name of the email server user.</param>
        /// <param name="emailServerUserPassword">The email server user password.</param>
        /// <returns>if true success if false failed</returns>
        public static bool SendEmailWithAttachment(string filenameAndLocation, string fromAddress, string toAddress, string emailSubject, string emailBody, string smtServerString, int smtpServerPort, string emailServerUserName, string emailServerUserPassword)
        {
            try
            {
                SmtpClient mailServer = new SmtpClient(smtServerString, smtpServerPort);//SmtpClient mailServer = new SmtpClient("smtp.gmail.com", 587);
                mailServer.EnableSsl = true;

                mailServer.Credentials = new System.Net.NetworkCredential(emailServerUserName, emailServerUserPassword);//new System.Net.NetworkCredential("myemail@gmail.com", "mypassword");
                
                MailMessage msg = new MailMessage(fromAddress, toAddress);
                msg.Subject = emailSubject;
                msg.Body = emailBody;
                if (!string.IsNullOrEmpty(filenameAndLocation))
                {
                    msg.Attachments.Add(new Attachment(filenameAndLocation));//msg.Attachments.Add(new Attachment("D:\\myfile.txt"));
                }
                mailServer.Send(msg);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
