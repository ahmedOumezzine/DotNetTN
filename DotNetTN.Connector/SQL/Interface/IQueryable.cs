using DotNetTN.Connector.SQL.SqlBuilderProvider;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DotNetTN.Connector.SQL.Interface
{
    public partial interface IQueryable<T>
    {
        SqlClient Context { get; set; }
        ISqlBuilder SqlBuilder { get; set; }
        QueryBuilder QueryBuilder { get; set; }

        T InSingle(object pkValue);

        List<T> ToList();

        IQueryable<T> AS(string tableName);

        IQueryable<T> Where(Expression<Func<T, bool>> expression);

        IQueryable<T1> Select<T1>(string v); 
    }
}