using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTN.API.EMAIL.Imap
{
    public class Quota
    {
        private string ressource;
        private string usage;
        private int used;
        private int max;
        public Quota(string ressourceName, string usage, int used, int max)
        {
            this.ressource = ressourceName;
            this.usage = usage;
            this.used = used;
            this.max = max;
        }
        public virtual int Used
        {
            get { return this.used; }
        }
        public virtual int Max
        {
            get { return this.max; }
        }
    }
}