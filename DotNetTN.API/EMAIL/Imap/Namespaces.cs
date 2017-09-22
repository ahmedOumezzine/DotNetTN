using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTN.API.EMAIL.Imap
{
    public class Namespaces
    {
        private Collection<Namespace> _servernamespace = new Collection<Namespace>();
        private Collection<Namespace> _usernamespace = new Collection<Namespace>();
        private Collection<Namespace> _sharednamespace = new Collection<Namespace>();

        public virtual Collection<Namespace> ServerNamespace
        {
            get { return this._servernamespace; }
        }
        public virtual Collection<Namespace> UserNamespace
        {
            get { return this._usernamespace; }
        }
        public virtual Collection<Namespace> SharedNamespace
        {
            get { return this._sharednamespace; }
        }
    }

    public class Namespace
    {
        public Namespace(string prefix, string delimiter)
        {
            Prefix = prefix;
            Delimiter = delimiter;
        }
        public Namespace() { }
        public virtual string Prefix { get; internal set; }
        public virtual string Delimiter { get; internal set; }
    }
}