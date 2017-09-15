using DotNetTN.Connector.SQL.Common;
using DotNetTN.Connector.SQL.SqlBuilderProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTN.Connector.SQL.Interface
{
    public interface IInsertable<T>
    {
        int ExecuteCommand();
        IInsertable<T> Where(T deleteObj);

    }
}