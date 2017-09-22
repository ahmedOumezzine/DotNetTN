using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTN.API.EMAIL
{
    public class ImapClientExceptionEventArgs : System.EventArgs
    {
        public ImapClientExceptionEventArgs(System.Exception Exception)
        {
            this.Exception = Exception;
        }

        public System.Exception Exception { get; set; }
    }
}
