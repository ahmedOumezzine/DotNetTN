using DotNetTN.API.EMAIL;
using DotNetTN.API.EMAIL.Imap;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DotNetTN.Tests.Emails
{
    [TestClass]
    public class EmailImapTest
    {
        /// <summary>
        ///  imap-mail.outlook.com
        ///  imap.gmail.com
        ///  imap.mail.yahoo.com
        /// </summary>
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

        [TestMethod]
        public void Message_With_Attachments()
        {
            using (ImapClient imap = new ImapClient("imap-mail.outlook.com", "username@outlook.fr", "password", AuthMethods.Login, 993, true))
            {
                var msg = imap.SearchMessages(SearchCondition.Larger(100 * 1000)).FirstOrDefault().Value;

                var att = msg.Attachments;
            }
        }
    }
}