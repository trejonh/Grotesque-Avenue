using System;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Windows.Forms;

namespace Horse.Engine.Utils
{
    public partial class EmailForm : Form
    {
        private static string _logFile;
        private EmailForm()
        {
            InitializeComponent();
            Text = AssetManager.GetMessage("FatalError");
        }

        private void EmailForm_Load(object sender, EventArgs e)
        {

        }

        public static void SendEmail(string logFile)
        {
            _logFile = logFile;
            var em = new EmailForm();
            em.ShowDialog();
        }
        
        private void SendBtnClicked(object sender, EventArgs e)
        {
            try
            {
                // Command line argument must the the SMTP host.
                var data = new Attachment(_logFile);
                var to = senderAddr.Text;
                if (string.IsNullOrEmpty(to) || to.Equals("foo@bar.com"))
                    to = "trejon_house@yahoo.com";
                var mailMessage = new MailMessage
                {
                    Body = _emailBody.Text,
                    IsBodyHtml = false,
                    From = new MailAddress("trejon_house@yahoo.com")
                };
                mailMessage.To.Add("trejon_house@yahoo.com");
                mailMessage.To.Add("coding.with.casa@gmail.com");
                mailMessage.CC.Add(to);
                mailMessage.Attachments.Add(data);
                var client =
                    new SmtpClient()
                    {
                        Host = "smtp.yahoo.com",
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        Port = 587,
                        EnableSsl = true,
                        UseDefaultCredentials = false
                    };
                client.Send(mailMessage);
                Console.WriteLine(@"connected");
                Console.ReadLine();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(@"connected");
                Console.WriteLine(@"connected");
                Console.ReadLine();
            }
            finally
            {
                Close();
                Environment.Exit(0);
            }
        }

        private void CancelBtnClicked(object sender, EventArgs e)
        {
            Console.WriteLine(@"connected");
            Close();
            Environment.Exit(0);
        }
    }
}
