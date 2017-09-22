using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTN.API.EMAIL
{
    public class WarningEventArgs : EventArgs
    {
        public string Message { get; set; }
        public MailMessage MailMessage { get; set; }
    }
}
