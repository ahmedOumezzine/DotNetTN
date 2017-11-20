using DotNetTN.Connector.SQL.Common;
using DotNetTN.Connector.SQL.Entities;
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

        List<T> ToListJOIN();

        IQueryable<T> AS(string tableName);

        IQueryable<T> Where(Expression<Func<T, bool>> expression);

        IQueryable<T1> Select<T1>(string v);
    }

    public partial interface IQueryable<T, T2> : IQueryable<T>
    {
        #region Where

        new IQueryable<T, T2> Where(Expression<Func<T, bool>> expression);

        IQueryable<T, T2> Where(Expression<Func<T, T2, bool>> expression);

        new IQueryable<T, T2> WhereIF(bool isWhere, Expression<Func<T, bool>> expression);

        IQueryable<T, T2> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression);

        new IQueryable<T, T2> Where(string whereString, object whereObj = null);

        new IQueryable<T, T2> WhereIF(bool isWhere, string whereString, object whereObj = null);

        #endregion Where

        #region Select

        IQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression);

        #endregion Select

        #region OrderBy

        new IQueryable<T, T2> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);

        IQueryable<T, T2> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc);

        #endregion OrderBy

        #region GroupBy

        new IQueryable<T, T2> GroupBy(Expression<Func<T, object>> expression);

        IQueryable<T, T2> GroupBy(Expression<Func<T, T2, object>> expression);

        #endregion GroupBy

        #region Aggr

        TResult Max<TResult>(Expression<Func<T, T2, TResult>> expression);

        TResult Min<TResult>(Expression<Func<T, T2, TResult>> expression);

        TResult Sum<TResult>(Expression<Func<T, T2, TResult>> expression);

        TResult Avg<TResult>(Expression<Func<T, T2, TResult>> expression);

        #endregion Aggr

        #region In

        new IQueryable<T, T2> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues);

        new IQueryable<T, T2> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues);

        new IQueryable<T, T2> In<FieldType>(Expression<Func<T, object>> expression, IQueryable<FieldType> childQueryExpression);

        #endregion In

        #region Other

        new IQueryable<T, T2> AS<AsT>(string tableName);

        new IQueryable<T, T2> AS(string tableName);

        new IQueryable<T, T2> Filter(string FilterName, bool isDisabledGobalFilter = false);

        new IQueryable<T, T2> AddParameters(object parameters);

        new IQueryable<T, T2> AddParameters(Parameter[] parameters);

        new IQueryable<T, T2> AddParameters(List<Parameter> parameters);

        new IQueryable<T, T2> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left);

        new IQueryable<T, T2> With(string withString);

        #endregion Other
    }
}