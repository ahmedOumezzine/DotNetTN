using DotNetTN.Connector.SQL.Common;
using DotNetTN.Connector.SQL.Entities;
using DotNetTN.Connector.SQL.Interface;
using DotNetTN.Connector.SQL.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTN.Connector.SQL.SqlBuilderProvider
{
    public class InsertableProvider<T> : IInsertable<T> where T : class, new()
    {
        public InsertableProvider()
        {
        }

        public SqlClient Context { get; set; }
        public IAdo Ado { get { return Context.Ado; } }

        public ISqlBuilder SqlBuilder { get; set; }
        public InsertBuilder InsertBuilder { get; set; }

        public bool IsMappingTable => this.Context.MappingTables != null && this.Context.MappingTables.Any();
        public bool IsMappingColumns { get { return this.Context.MappingColumns != null && this.Context.MappingColumns.Any(); } }
       
        public EntityInfo EntityInfo { get; set; }
        public List<MappingColumn> MappingColumnList { get; set; }
        private bool IsOffIdentity { get; set; }
        public T InsertObjs { get; set; }

        public MappingTableList OldMappingTableList { get; set; }
        public bool IsAs { get; set; }

        #region Core
        public int ExecuteCommand()
        {
            InsertBuilder.IsReturnIdentity = false;
            PreToSql();
            string sql = InsertBuilder.ToSqlString();
           
            return Ado.ExecuteCommand(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
        }

        internal void Init()
        {
            InsertBuilder.EntityInfo = this.EntityInfo;
         //   Check.Exception(InsertObjs == null || InsertObjs.Count() == 0, "InsertObjs is null");
            int i = 0;
         
                List<DbColumnInfo> insertItem = new List<DbColumnInfo>();
                foreach (var column in EntityInfo.Columns)
                {
                    var columnInfo = new DbColumnInfo()
                    {
                        Value = column.PropertyInfo.GetValue(InsertObjs, null),
                        DbColumnName = GetDbColumnName(column.PropertyName),
                        PropertyName = column.PropertyName,
                        PropertyType = Extensions.GetUnderType(column.PropertyInfo),
                        TableId = i
                    };
                    if (columnInfo.PropertyType.IsEnum())
                    {
                        columnInfo.Value = Convert.ToInt64(columnInfo.Value);
                    }
                    insertItem.Add(columnInfo);
                }
                this.InsertBuilder.DbColumnInfoList.AddRange(insertItem);
                ++i;
            
        }

        public int ExecuteReturnIdentity()
        {
            InsertBuilder.IsReturnIdentity = true;
            PreToSql();
            string sql = InsertBuilder.ToSqlString();
            return Ado.GetInt(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
        }
        public long ExecuteReturnBigIdentity()
        {
            InsertBuilder.IsReturnIdentity = true;
            PreToSql();
            string sql = InsertBuilder.ToSqlString();
            return Convert.ToInt64(Ado.GetScalar(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray()));
        }
        

        #endregion

        #region Setting
       


        
        public IInsertable<T> With(string lockString)
        {
            if (this.Context.Config.DbType == DbType.SqlServer)
                this.InsertBuilder.TableWithString = lockString;
            return this;
        }

     
        #endregion

        #region Private Methods
        private void PreToSql()
        {
            #region Identities
            if (!IsOffIdentity)
            {
                List<string> identities = GetIdentityKeys();
                if (identities != null && identities.Any())
                {
                    this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it =>
                    {
                        return !identities.Any(i => it.DbColumnName.Equals(i, StringComparison.CurrentCultureIgnoreCase));
                    }).ToList();
                }
            }
            #endregion

      
                foreach (var item in this.InsertBuilder.DbColumnInfoList)
                {
                    if (this.InsertBuilder.Parameters == null) this.InsertBuilder.Parameters = new List<Parameter>();
                    var paramters = new Parameter(this.SqlBuilder.SqlParameterKeyWord + item.DbColumnName, item.Value, item.PropertyType);
                    if (InsertBuilder.IsNoInsertNull && paramters.Value == null)
                    {
                        continue;
                    }
                    this.InsertBuilder.Parameters.Add(paramters);
                }
            
        }
        private string GetDbColumnName(string entityName)
        {
            if (!IsMappingColumns)
            {
                return entityName;
            }
            if (this.Context.MappingColumns.Any(it => it.EntityName.Equals(EntityInfo.EntityName, StringComparison.CurrentCultureIgnoreCase)))
            {
                this.MappingColumnList = this.Context.MappingColumns.Where(it => it.EntityName.Equals(EntityInfo.EntityName, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
            if (MappingColumnList == null || !MappingColumnList.Any())
            {
                return entityName;
            }
            else
            {
                var mappInfo = this.Context.MappingColumns.FirstOrDefault(it => it.PropertyName.Equals(entityName, StringComparison.CurrentCultureIgnoreCase));
                return mappInfo == null ? entityName : mappInfo.DbColumnName;
            }
        }


        private List<string> GetIdentityKeys()
        {
                return this.EntityInfo.Columns.Where(it => it.IsIdentity).Select(it => it.DbColumnName).ToList();
            
        }

        public IInsertable<T> Where(T deleteObj)
        {
            throw new NotImplementedException();
        }

        public IInsertable<T> UpdateColumns(Func<string, bool> updateColumMethod)
        {
            throw new NotImplementedException();
        }

        public IInsertable<T> Where(bool isNoUpdateNull, bool IsOffIdentity = false)
        {
            throw new NotImplementedException();
        }

        public IInsertable<T> Where(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public IInsertable<T> Where(Expression<Func<T, string>> expression)
        {
            throw new NotImplementedException();
        }

        public IInsertable<T> Where(Expression<Func<T, object>> expression)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
