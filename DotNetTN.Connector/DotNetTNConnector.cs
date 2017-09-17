using DotNetTN.Connector.SQL;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DotNetTN.Connector
{
    public class DotNetTNConnector
    {
        protected SqlClient Context { get; set; }

        private DotNetTNConnector()
        {
        }

        public DotNetTNConnector(SqlClient context)
        {
            this.Context = context;
        }

        public T GetById<T>(dynamic id) where T : class, new()
        {
            return Context.Queryable<T>().InSingle(id);
        }

        public List<T> GetList<T>() where T : class, new()
        {
            return Context.Queryable<T>().ToList();
        }

        public List<T> GetList<T>(Expression<Func<T, bool>> whereExpression) where T : class, new()
        {
            return Context.Queryable<T>().Where(whereExpression).ToList();
        }

        public bool Insert<T>(T insertObj) where T : class, new()
        {
            return this.Context.Insertable(insertObj).ExecuteCommand() > 0;
        }

        public bool Update<T>(T updateObj) where T : class, new()
        {
            return this.Context.Updateable(updateObj).ExecuteCommand() > 0;
        }

        public bool Delete<T>(T deleteObj) where T : class, new()
        {
            //return true;
            return this.Context.Deleteable<T>().Where(deleteObj).ExecuteCommand() > 0;
        }

        public bool DeleteById<T>(dynamic id) where T : class, new()
        {
            return this.Context.Deleteable<T>().In(id).ExecuteCommand() > 0;
        }
    }
}