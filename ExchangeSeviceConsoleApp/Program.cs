using CsvHelper;
using CsvHelper.Configuration;
using ExchangeSeviceConsoleApp.Entity;
using ExchangeSeviceConsoleApp.Helper;
using Microsoft.Exchange.WebServices.Data;
using System.Collections;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using Attachment = Microsoft.Exchange.WebServices.Data.Attachment;

namespace EmailAutomationTesting
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //private static readonly string readUserReadCredentialFile = @"D:\R & d\ExchangeSeviceConsoleApp\Credentials.csv";
        //private static readonly string UserWriteFile = @"D:\R & d\ExchangeSeviceConsoleApp\UserData.csv";
        static void Main(string[] args)
        {
            try
            {
                LogHelper.InitLog();
                LogHelper.LogMessage("Process Started : " + DateTime.Now);
                var path = AppDomain.CurrentDomain.BaseDirectory;

                var CredentialsOfUsers = File.ReadAllLines(path + "\\Credentials.csv")
                                 .Select(x => x.Split(','))
                                 .ToArray();
                var UserDetailsLogged = File.ReadAllLines(path + "\\UserData.csv")
                                 .Select(x => x.Split(','))
                                 .ToArray();
                for (int c = 0; c < CredentialsOfUsers.Count(); c++)
                {
                    var Credentials = CredentialsOfUsers[c];
                    if (Credentials != null)
                    {
                        string SearchFilterDatetime = "";
                        string TimeTobeSaved = "";
                        #region Read User Data 
                        LogHelper.LogMessage("Total Source Emails : " + CredentialsOfUsers.Length);

                        if (UserDetailsLogged == null || UserDetailsLogged.Length == 0)
                        {
                            SearchFilterDatetime = DateTime.Now.AddHours(-1).ToString("yyyy/MM/dd HH:mm:ss.fff", null);
                            TimeTobeSaved = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff", null);
                            var records = new List<Foo>
                                        {
                                            new Foo { EmailId = $"{Credentials[0].ToString()}", LastSync= $"{TimeTobeSaved}" },
                                        };
                            bool WriteCs = WriteInCSV(path + "\\UserData.csv", records);
                        }
                        else
                        {
                            for (int lo = 1; lo < UserDetailsLogged.Count(); lo++)
                            {
                                var UserDetails = UserDetailsLogged[lo];
                                bool Isuser = UserDetails.Any(s => Credentials[0].Contains(s));
                                if (Isuser != false)
                                {
                                    if (UserDetails[1] != null)
                                    {
                                        LogHelper.LogMessage("Last Sync Date : " + UserDetails[1]);
                                        SearchFilterDatetime = UserDetails[1];
                                        TimeTobeSaved = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff", null);
                                        var records = new List<Foo>
                                        {
                                            new Foo { EmailId = $"{Credentials[0].ToString()}", LastSync = $"{TimeTobeSaved}" },
                                        };
                                        bool WriteCs = WriteInCSV(path + "\\UserData.csv", records);
                                        break;
                                    }
                                    else
                                    {
                                        SearchFilterDatetime = DateTime.Now.AddHours(-1).ToString("yyyy/MM/dd HH:mm:ss.fff", null);
                                        TimeTobeSaved = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff", null);
                                        var records = new List<Foo>
                                        {
                                            new Foo { EmailId = $"{Credentials[0].ToString()}", LastSync = $"{TimeTobeSaved}" },
                                        };
                                        bool WriteCs = WriteInCSV(path + "\\UserData.csv", records);
                                    }
                                    
                                }
                                else
                                {
                                    SearchFilterDatetime = DateTime.Now.AddHours(-1).ToString("yyyy/MM/dd HH:mm:ss.fff", null);
                                    TimeTobeSaved = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff", null);
                                    var records = new List<Foo>
                                        {
                                            new Foo { EmailId = $"{Credentials[0].ToString()}", LastSync= $"{TimeTobeSaved}" },
                                        };
                                    bool WriteCs = WriteInCSV(path + "\\UserData.csv", records);
                                }
                            }
                        }
                        #endregion
                        try
                        {
                            #region Mail Read with Filtered Date
                            ExchangeService exchange = new ExchangeService(ExchangeVersion.Exchange2016);//source read -to copy
                            string URL = ConfigurationManager.AppSettings["URL"];//URL
                                                                                 //string pass = Decrypt(Credentials[1].ToString());
                            LogHelper.LogMessage("Reading mails from : " + Credentials[0].ToString());
                            //byte[] data = Convert.FromBase64String(Credentials[1].ToString());
                            //string decodedPassword= Encoding.UTF8.GetString(data);
                            exchange.Credentials = new WebCredentials(Credentials[0].ToString(), Credentials[1].ToString());//from 
                            exchange.Url = new Uri(URL);

                            //TimeSpan ts = new TimeSpan(0, -1, 0, 0);
                            //DateTime date = DateTime.Now.Add(ts);
                            //SearchFilter.IsGreaterThanOrEqualTo filter = new SearchFilter.IsGreaterThanOrEqualTo(ItemSchema.DateTimeReceived, date);
                            #endregion
                            if (exchange != null)
                            {
                                string DatetimeSearchFilter = SearchFilterDatetime;
                                SearchFilter sfs = new SearchFilter.IsGreaterThan(ItemSchema.DateTimeReceived, DateTime.ParseExact(DatetimeSearchFilter, "yyyy/MM/dd HH:mm:ss.fff", null));
                                FindItemsResults<Item> findResults = exchange.FindItems(WellKnownFolderName.Inbox, sfs, new ItemView(100000));
                                LogHelper.LogMessage("Total Emails Found : " + findResults.Count());

                                foreach (Item item in findResults)
                                {
                                    try
                                    {
                                        #region Mail Message Detais with Attatchment
                                        EmailMessage message = EmailMessage.Bind(exchange, item.Id);
                                        Console.WriteLine(message.ConversationTopic);
                                        LogHelper.LogMessage("Date Time Received : " + message.DateTimeReceived.ToString());
                                        LogHelper.LogMessage("From Address : " + message.From.Address.ToString());
                                        string ToRecipients = "";
                                        var Recipients = new string[message.ToRecipients.Count];
                                        for (int iIdx = 0; iIdx < message.ToRecipients.Count; iIdx++)
                                        {
                                            ToRecipients = message.ToRecipients[iIdx].Address + ", " + ToRecipients;
                                        }
                                        string CcRecipients = "";
                                        for (int iIdx = 0; iIdx < message.CcRecipients.Count; iIdx++)
                                        {
                                            CcRecipients = message.CcRecipients[iIdx].Address + ", " + CcRecipients;
                                        }
                                        LogHelper.LogMessage("ToRecipients : " + ToRecipients.ToString());
                                        LogHelper.LogMessage("CcRecipients : " + CcRecipients.ToString());
                                        LogHelper.LogMessage("Subject : " + message.Subject.ToString());
                                        LogHelper.LogMessage("Attachments : " + ((message.HasAttachments) ? "Yes" : "No"));
                                        LogHelper.LogMessage("Message Id: " + message.Id.ToString());
                                        List<AttachmentEntity> atAttachmentsList = new List<AttachmentEntity>();
                                        List<FileAttachment> fileAttachmentsEWS = new List<FileAttachment>();
                                        #region Attatchment Get
                                        foreach (Attachment attachment in message.Attachments)
                                        {
                                            if (attachment is FileAttachment)
                                            {
                                                attachment.Load();
                                                FileAttachment fileAttachment1 = (FileAttachment)attachment;
                                                fileAttachment1.Load();
                                                fileAttachmentsEWS.Add(fileAttachment1);
                                                AttachmentEntity NAttachmentEntity = new AttachmentEntity();
                                                NAttachmentEntity.AttachmentFileName = fileAttachment1.Name;
                                                NAttachmentEntity.AttachmentBinaryFile = fileAttachment1.Content;
                                                atAttachmentsList.Add(NAttachmentEntity);
                                                Console.WriteLine("File attachment name: " + fileAttachment1.Name);
                                            }
                                            else // Attachment is an item attachment.
                                            {
                                                attachment.Load();
                                                FileAttachment fileAttachment1 = (FileAttachment)attachment;
                                                fileAttachment1.Load();
                                                fileAttachmentsEWS.Add(fileAttachment1);
                                                AttachmentEntity NAttachmentEntity = new AttachmentEntity();
                                                NAttachmentEntity.AttachmentFileName = fileAttachment1.Name;
                                                NAttachmentEntity.AttachmentBinaryFile = fileAttachment1.Content;
                                                atAttachmentsList.Add(NAttachmentEntity);
                                                Console.WriteLine("File attachment name: " + fileAttachment1.Name);
                                            }
                                        }
                                        #endregion
                                        EmailTemplateEntity Template = new EmailTemplateEntity();
                                        Template.Subject = message.Subject.ToString();
                                        StringBuilder mailBody = new StringBuilder();
                                        mailBody.Append("From Address :" + message.From.Address.ToString());
                                        mailBody.Append("<br />");
                                        mailBody.Append("To Address :" + ToRecipients);
                                        mailBody.Append("<br />");
                                        mailBody.Append("Cc Recipients :" + CcRecipients);
                                        mailBody.Append("<br />");
                                        #region mail Body with attatchment and images
                                        message.Body.BodyType = BodyType.HTML;
                                        string sHTMLCOntent = message.Body.Text;
                                        FileAttachment[] attachments = null;
                                        if (message.Attachments.Count != 0)
                                        {
                                            attachments = new FileAttachment[message.Attachments.Count];
                                            for (int i = 0; i < message.Attachments.Count; i++)
                                            {
                                                try
                                                {
                                                    if (message.Attachments[i].IsInline)
                                                    {
                                                        string sType = message.Attachments[i].ContentType.ToLower();
                                                        if (sType.Contains("image"))
                                                        {
                                                            attachments[i] = (FileAttachment)message.Attachments[i];
                                                            attachments[i].Load();
                                                            string sId = attachments[i].ContentId;
                                                            sType = sType.Replace("image/", "");
                                                            string oldString = "cid:" + sId;
                                                            string imagem =
                                                                Convert.ToBase64String(attachments[i].Content);
                                                            sHTMLCOntent = sHTMLCOntent.Replace(oldString,
                                                                "data:image/" + sType + ";base64," + imagem);
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    LogHelper.LogError("Error Message: " + ex.Message.ToString());
                                                }
                                            }
                                        }
                                        #endregion
                                        //mailBody.Append(message.Body);
                                        mailBody.Append(sHTMLCOntent);
                                        mailBody.Append("<br />");
                                        Template.Body = mailBody.ToString();
                                        Template.EmailAttachments = atAttachmentsList;
                                        string ToMail = Credentials[2];
                                        var sent = SendMail(Template, ToMail, "", "", Credentials);//Send Mail
                                        #endregion
                                    }
                                    catch (Exception ex)
                                    {
                                        LogHelper.LogError("Error Message: " + ex.Message.ToString());
                                    }
                                }
                                if (findResults.Items.Count <= 0)
                                {
                                    Console.WriteLine("No Messages found!!");
                                }
                                LogHelper.LogMessage("**Completed Reading mails For :" + Credentials[0].ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.LogError("Error Message: " + ex.Message.ToString());
                        }
                    }
                    else
                    {
                        Console.WriteLine("No Credentails Found");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError("Error Message: " + ex.Message.ToString());
            }
        }
        #region SendMail 
        private static bool SendMail(EmailTemplateEntity templateEntity, string To, string CC = "", string BCC = "", string[] Credentials = null)
        {
            bool flag = true;
            try
            {
                string Email = ConfigurationManager.AppSettings["Email"];
                //string DisplayName = ConfigurationManager.AppSettings["DisplayName"];
                string Host = ConfigurationManager.AppSettings["Host"];
                int Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
                string Password = ConfigurationManager.AppSettings["Password"];
                bool EnableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSSL"]);
                bool UseDefaultCredentials = Convert.ToBoolean(ConfigurationManager.AppSettings["UseDefaultCredentials"]);
                SendMailEntity mailEntity = new SendMailEntity();
                MailMessage message = new MailMessage();
                if (templateEntity != null)
                {
                    var emailAccountEntity = new EmailAccountEntity();
                    emailAccountEntity.Email = Email;
                    emailAccountEntity.Host = Host;
                    emailAccountEntity.Port = Port;
                    emailAccountEntity.UseDefaultCredentials = UseDefaultCredentials;
                    emailAccountEntity.EnableSsl = EnableSSL;
                    emailAccountEntity.Password = Password;
                    //emailAccountEntity.DisplayName = DisplayName;
                    string Email_Subject = string.Empty;
                    string Email_Body = string.Empty;

                    Email_Subject = templateEntity.Subject;
                    Email_Body = templateEntity.Body;

                    mailEntity.From = emailAccountEntity.Email;
                    mailEntity.Subject = Email_Subject;
                    mailEntity.Body = Email_Body;
                    mailEntity.To = To;
                    mailEntity.CC = CC;
                    mailEntity.Bcc = BCC;

                    #region Subject

                    message.Subject = mailEntity.Subject;

                    #endregion

                    #region  DeliveryNotificationOptions

                    //  string SMTPUserID = _configuration.GetValue<string>("AuthenticationType:SMTPUserID");
                    //message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure | DeliveryNotificationOptions.OnSuccess | DeliveryNotificationOptions.Delay;
                    //message.Headers.Add("Disposition-Notification-To", emailAccountEntity.Email);
                    #endregion

                    #region From

                    if (mailEntity.From != null && mailEntity.From != "")
                    {
                        message.From = new MailAddress(mailEntity.From);
                    }

                    #endregion

                    #region To

                    if (mailEntity.To != null && mailEntity.To != "")
                    {
                        string[] ToAddresses = mailEntity.To.Split(',');

                        foreach (string value in ToAddresses)
                        {
                            if (!string.IsNullOrEmpty(value))
                            {
                                message.To.Add(new MailAddress(value));
                            }
                        }
                    }

                    #endregion

                    #region CC

                    if (mailEntity.CC != null && mailEntity.CC != "")
                    {
                        string[] CCAddresses = mailEntity.CC.Split(',');

                        foreach (string value in CCAddresses)
                        {
                            if (!string.IsNullOrEmpty(value))
                            {
                                message.CC.Add(new MailAddress(value));
                            }
                        }
                    }

                    #endregion

                    #region Bcc

                    if (mailEntity.Bcc != null && mailEntity.Bcc != "")
                    {
                        string[] BccAddresses = mailEntity.Bcc.Split(',');

                        foreach (string value in BccAddresses)
                        {
                            message.Bcc.Add(new MailAddress(value));
                        }
                    }

                    #endregion

                    #region Other

                    message.IsBodyHtml = true;

                    #endregion

                    #region Attachments

                    if (templateEntity.EmailAttachments.Count > 0)
                    {
                        for (int i = 0; i < templateEntity.EmailAttachments.Count; ++i)
                        {
                            message.Attachments.Add(new System.Net.Mail.Attachment(new MemoryStream(templateEntity.EmailAttachments[i].AttachmentBinaryFile), templateEntity.EmailAttachments[i].AttachmentFileName));
                        }
                    }

                    #endregion

                    #region Send Mail Code
                    message.Body = mailEntity.Body;
                    message.IsBodyHtml = true;

                    //send email
                    using (var smtpClient = new SmtpClient())
                    {
                        smtpClient.UseDefaultCredentials = emailAccountEntity.UseDefaultCredentials;
                        smtpClient.Host = Convert.ToString(emailAccountEntity.Host);
                        smtpClient.Port = Convert.ToInt32(emailAccountEntity.Port);
                        smtpClient.EnableSsl = emailAccountEntity.EnableSsl;

                        smtpClient.Credentials = new NetworkCredential(Convert.ToString(emailAccountEntity.Email), Convert.ToString(emailAccountEntity.Password));

                        smtpClient.Send(message);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                flag = false;
                //throw;
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                LogHelper.LogError(ex.Source);
            }
            return flag;
        }
        #endregion

        #region CSV Read Write Files
        private static bool WriteInCSV(string absolutePath, List<Foo> foos)
        {
            try
            {
                List<Foo> records;

                using (var reader = new StreamReader(absolutePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    records = csv.GetRecords<Foo>().ToList();

                    for (int i = 0; i < records.Count; ++i)
                    {
                        if (records[i].EmailId == foos[0].EmailId)
                        {
                            records.RemoveAt(i);
                        }
                    }
                }
                using (var writer = new StreamWriter(absolutePath))
                using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteRecords(records);
                }
                //// Append to the file.
                bool append = true;
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    // Don't write the header again.
                    HasHeaderRecord = false,
                };

                using (var writer = new StreamWriter(absolutePath, append))
                {
                    using (var csv = new CsvWriter(writer, config))
                    {
                        csv.WriteRecords(foos);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex.Message);
                LogHelper.LogError(ex.StackTrace);
                LogHelper.LogError(ex.Source);
                return false;
            }
        }
        #endregion

        public static string Decrypt(string textToDecrypt)
        {
            try
            {
                string ToReturn = "";
                string publickey = "santhosh";
                string privatekey = "engineer";
                byte[] privatekeyByte = { };
                privatekeyByte = System.Text.Encoding.UTF8.GetBytes(privatekey);
                byte[] publickeybyte = { };
                publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = new byte[textToDecrypt.Replace(" ", "+").Length];
                inputbyteArray = Convert.FromBase64String(textToDecrypt.Replace(" ", "+"));
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateDecryptor(publickeybyte, privatekeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    Encoding encoding = Encoding.UTF8;
                    ToReturn = encoding.GetString(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception ae)
            {
                throw new Exception(ae.Message, ae.InnerException);
            }
        }
    }
}
