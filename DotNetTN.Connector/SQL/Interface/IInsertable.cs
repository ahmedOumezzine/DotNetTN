using System;
using System.Linq.Expressions;

namespace DotNetTN.Connector.SQL.Interface
{
    public interface IInsertable<T>
    {
        int ExecuteCommand();

        IInsertable<T> Where(T deleteObj);

        IInsertable<T> Where(Expression<Func<T, object>> expression);
    }
}