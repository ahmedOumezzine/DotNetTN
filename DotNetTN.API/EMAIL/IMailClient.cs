using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTN.API.EMAIL
{
    public interface IMailClient : IDisposable
    {
        int GetMessageCount();
        MailMessage GetMessage(int index, bool headersonly = false);
        MailMessage GetMessage(string uid, bool headersonly = false);
        void DeleteMessage(string uid);
        void DeleteMessage(MailMessage msg);
        void Disconnect();

        event EventHandler<WarningEventArgs> Warning;
    }
}
