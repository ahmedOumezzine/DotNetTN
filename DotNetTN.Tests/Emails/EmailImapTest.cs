using DotNetTN.API.EMAIL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetTN.Tests.Emails
{
    [TestClass]
    public class EmailImapTest
    {
        [TestMethod]
        public void GetMessageCount()
        {
            using (ImapClient imap = new ImapClient("imap-mail.outlook.com", "username@outlook.fr", "password", AuthMethods.Login, 993, true))
            {
                var count = imap.GetMessageCount();
            }
        }

        [TestMethod]
        public void GetMessages()
        {
            using (ImapClient imap = new ImapClient("imap-mail.outlook.com", "username@outlook.fr", "password", AuthMethods.Login, 993, true))
            {
                int numMessages = 10;
                var msgs = imap.GetMessages(0, numMessages - 1, false);
            }
        }
    }
}