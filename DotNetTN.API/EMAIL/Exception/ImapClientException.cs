using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTN.API.EMAIL
{
    [Serializable()]
    public class ImapClientException : Exception
    {
        public ImapClientException() : base() { }
        public ImapClientException(string message) : base(message) { }
        public ImapClientException(string message, Exception inner) : base(message, inner) { }

        protected ImapClientException(System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context)
        { }
    }
}
