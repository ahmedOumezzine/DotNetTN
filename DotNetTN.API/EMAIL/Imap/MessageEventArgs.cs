using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTN.API.EMAIL.Imap
{
    public class MessageEventArgs : EventArgs
    {
        public virtual int MessageCount { get; set; }
        internal ImapClient Client { get; set; }
    }
}
