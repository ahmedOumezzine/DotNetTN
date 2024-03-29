﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTN.API.EMAIL.Imap
{
    public class Mailbox
    {
        public Mailbox() : this(string.Empty) { }
        public Mailbox(string name)
        {
            Name = name;
            Flags = new string[0];
        }
        public virtual string Name { get; internal set; }
        public virtual int NumNewMsg { get; internal set; }
        public virtual int NumMsg { get; internal set; }
        public virtual int NumUnSeen { get; internal set; }
        public virtual int UIDValidity { get; internal set; }
        public virtual string[] Flags { get; internal set; }
        public virtual bool IsWritable { get; internal set; }

        internal void SetFlags(string flags)
        {
            Flags = flags.Split(' ');
        }
    }
}

