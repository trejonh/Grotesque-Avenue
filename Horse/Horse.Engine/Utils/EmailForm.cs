using System;
using System.Net.Mail;
using System.Windows.Forms;

namespace Horse.Engine.Utils
{
    public partial class EmailForm : Form
    {
        private EmailForm()
        {
            InitializeComponent();
            Text = AssetManager.GetMessage("FatalError");
        }

        private void EmailForm_Load(object sender, EventArgs e)
        {

        }

        public static void SendEmail()
        {
            new EmailForm();
        }
        
        private void SendBtnClicked(object sender, EventArgs e)
        {
            // Command line argument must the the SMTP host.
            var client = new SmtpClient("smtp.mail.yahoo.com");
            var to = senderAddr.Text;
            if (String.IsNullOrEmpty(to))
                to = "trejon_house@yahoo.com";
            var mailMessage = new MailMessage("trejon_house@yahoo.com", to);
            // Specify the e-mail sender.
            // Create a mailing address that includes a UTF8 character
            // in the display name.
            MailAddress from = new MailAddress("jane@contoso.com",
               "Jane " + (char)0xD8 + " Clayton",
            System.Text.Encoding.UTF8);
            // Set destinations for the e-mail message.
            MailAddress to = new MailAddress("ben@contoso.com");
            // Specify the message content.
            MailMessage message = new MailMessage(from, to);
            message.Body = "This is a test e-mail message sent by an application. ";
            // Include some non-ASCII characters in body and subject.
            string someArrows = new string(new char[] { '\u2190', '\u2191', '\u2192', '\u2193' });
            message.Body += Environment.NewLine + someArrows;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.Subject = "test message 1" + someArrows;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            // Set the method that is called back when the send operation ends.
            client.SendCompleted += new
            SendCompletedEventHandler(SendCompletedCallback);
            // The userState can be any object that allows your callback 
            // method to identify this send operation.
            // For this example, the userToken is a string constant.
            string userState = "test message1";
            client.SendAsync(message, userState);
            Console.WriteLine("Sending message... press c to cancel mail. Press any other key to exit.");
            string answer = Console.ReadLine();
            // If the user canceled the send, and mail hasn't been sent yet,
            // then cancel the pending operation.
            if (answer.StartsWith("c") && mailSent == false)
            {
                client.SendAsyncCancel();
            }
            // Clean up.
            message.Dispose();
            Console.WriteLine("Goodbye.");
        }

        private void CancelBtnClicked(object sender, EventArgs e)
        {

        }
    }
}
