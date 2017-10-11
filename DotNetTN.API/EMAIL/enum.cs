using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTN.API.EMAIL
{
    public enum MailPriority
    {
        Normal = 3,
        High = 5,
        Low = 1
    }

    [System.Flags]
    public enum Flags
    {
        None = 0,
        Seen = 1,
        Answered = 2,
        Flagged = 4,
        Deleted = 8,
        Draft = 16
    }
    public enum AuthMethods
    {
        Login,
        CRAMMD5,
        SaslOAuth
    }

}
