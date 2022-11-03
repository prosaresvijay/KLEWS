using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeSeviceConsoleApp.Entity
{
    public class EmailTemplateEntity
    {
        public long QueueID { get; set; }

        public Int64 UserID { get; set; }

        public string EmailType { get; set; }

        public string Name { get; set; }

        public string LoginID { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the From property
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the FromName property
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// Gets or sets the To property
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Gets or sets the ToName property
        /// </summary>
        public string ToName { get; set; }

        /// <summary>
        /// Gets or sets the ReplyTo property
        /// </summary>
        public string ReplyTo { get; set; }

        /// <summary>
        /// Gets or sets the ReplyToName property
        /// </summary>
        public string ReplyToName { get; set; }

        /// <summary>
        /// Gets or sets the CC
        /// </summary>
        public string CC { get; set; }

        /// <summary>
        /// Gets or sets the Bcc
        /// </summary>
        public string Bcc { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.
        /// </summary>
        public string AttachmentFileName { get; set; }

        /// <summary>
        /// Gets or sets the download identifier of attached file
        /// </summary>
        public byte[] AttachmentBinaryFile { get; set; }

        /// <summary>
        /// Gets or sets Is Delivery Receipt Required
        /// </summary>
        public bool IsDeliveryReceiptRequired { get; set; }

        /// <summary>
        /// Gets or sets Is Read Receipt Required
        /// </summary>
        public bool IsReadReceiptRequired { get; set; }

        /// <summary>
        /// Gets or sets the send tries
        /// </summary>
        public int SentTries { get; set; }

        /// <summary>
        /// Gets or sets the sent date and time
        /// </summary>
        public DateTime SentOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the used email account identifier
        /// </summary>
        public Int64 EmailAccountId { get; set; }

        public string ClientName { get; set; }
        public string RequestNo { get; set; }

        public string ManagerName { get; set; }

        public DateTime InvitationDate { get; set; }

        public string Value { get; set; }

        public List<AttachmentEntity> EmailAttachments { get; set; }

    }
    public class AttachmentEntity 
    {
        public Int64 QueuedEmailId { get; set; }
        /// <summary>
        /// Gets or sets the attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.
        /// </summary>
        public string AttachmentFileName { get; set; }

        /// <summary>
        /// Gets or sets the download identifier of attached file
        /// </summary>
        public byte[] AttachmentBinaryFile { get; set; }
    }
    public class ErrorLogEntity
    {
        #region Properties

        public Guid ErrorGuid { get; set; }

        public string Source { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public string RequestType { get; set; }

        public string TargetSite { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public string Exception { get; set; }

        public DateTime ErrorDateTime { get; set; }

        public string FunctionName { get; set; }

        public string HResult { get; set; }

        public string InnerException { get; set; }

        public string ExceptionMessage { get; set; }

        #endregion
    }
    public class SendMailEntity
    {
        public long ID { get; set; }
        public long QueueID { get; set; }

        public Int64 UserID { get; set; }

        public string EmailType { get; set; }

        public string Name { get; set; }

        public string LoginID { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the From property
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the FromName property
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// Gets or sets the To property
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Gets or sets the ToName property
        /// </summary>
        public string ToName { get; set; }

        /// <summary>
        /// Gets or sets the ReplyTo property
        /// </summary>
        public string ReplyTo { get; set; }

        /// <summary>
        /// Gets or sets the ReplyToName property
        /// </summary>
        public string ReplyToName { get; set; }

        /// <summary>
        /// Gets or sets the CC
        /// </summary>
        public string CC { get; set; }

        /// <summary>
        /// Gets or sets the Bcc
        /// </summary>
        public string Bcc { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.
        /// </summary>
        public string AttachmentFileName { get; set; }

        /// <summary>
        /// Gets or sets the download identifier of attached file
        /// </summary>
        public byte[] AttachmentBinaryFile { get; set; }

        /// <summary>
        /// Gets or sets Is Delivery Receipt Required
        /// </summary>
        public bool IsDeliveryReceiptRequired { get; set; }

        /// <summary>
        /// Gets or sets Is Read Receipt Required
        /// </summary>
        public bool IsReadReceiptRequired { get; set; }

        /// <summary>
        /// Gets or sets the send tries
        /// </summary>
        public int SentTries { get; set; }

        /// <summary>
        /// Gets or sets the sent date and time
        /// </summary>
        public DateTime SentOnUtc { get; set; }

        public bool IsCalenderInvite { get; set; }

        public string BodyCalendar { get; set; }
    }
    public class EmailAccountEntity
    {
        #region Properties

        /// <summary>
        /// Gets or sets an email address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets an email display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets an email host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets an email port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets an email user name
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets an email password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value that controls whether the SmtpClient uses Secure Sockets Layer (SSL) to encrypt the connection
        /// </summary>
        public bool EnableSsl { get; set; }

        /// <summary>
        /// Gets or sets a value that controls whether the default system credentials of the application are sent with requests.
        /// </summary>
        public bool UseDefaultCredentials { get; set; }

        /// <summary>
        /// Gets a friendly email account name
        /// </summary>
        public string FriendlyName
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(this.DisplayName))
                    return this.Email + " (" + this.DisplayName + ")";
                return this.Email;
            }
        }

        #endregion
    }
    public class Foo
    {
        public string EmailId { get; set; }
        public string LastSync { get; set; }
    }
}
