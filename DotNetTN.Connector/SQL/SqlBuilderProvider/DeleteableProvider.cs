using DotNetTN.Connector.SQL.Common;
using DotNetTN.Connector.SQL.Entities;
using DotNetTN.Connector.SQL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTN.Connector.SQL.SqlBuilderProvider
{
    public class DeleteableProvider<T> : IInsertable<T> where T : class, new()
    {
        public SqlClient Context { get; set; }
        public IAdo Db { get { return Context.Ado; } }
        public ISqlBuilder SqlBuilder { get; set; }
        public DeleteBuilder DeleteBuilder { get; set; }
        public bool IsAs { get; set; }
        public EntityInfo EntityInfo
        {
            get
            {
                return this.Context.EntityMaintenance.GetEntityInfo<T>();
            }
        }
        public int ExecuteCommand()
        {
            DeleteBuilder.EntityInfo = this.Context.EntityMaintenance.GetEntityInfo<T>();
            string sql = DeleteBuilder.ToSqlString();
            var paramters = DeleteBuilder.Parameters == null ? null : DeleteBuilder.Parameters.ToArray();
            return Db.ExecuteCommand(sql, paramters);
        }
     
    

        public KeyValuePair<string, List<Parameter>> ToSql()
        {
            DeleteBuilder.EntityInfo = this.Context.EntityMaintenance.GetEntityInfo<T>();
            string sql = DeleteBuilder.ToSqlString();
            var paramters = DeleteBuilder.Parameters == null ? null : DeleteBuilder.Parameters.ToList();
            return new KeyValuePair<string, List<Parameter>>(sql, paramters);
        }

        public IInsertable<T> Where(T deleteObj)
        {
            throw new NotImplementedException();
        }

        private List<string> GetIdentityKeys()
        {
            return this.EntityInfo.Columns.Where(it => it.IsIdentity).Select(it => it.DbColumnName).ToList();

        }

        private List<string> GetPrimaryKeys()
        {
            return this.EntityInfo.Columns.Where(it => it.IsPrimarykey).Select(it => it.DbColumnName).ToList();

        }

    }
}
