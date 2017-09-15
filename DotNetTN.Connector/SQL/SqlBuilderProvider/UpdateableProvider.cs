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
    public class UpdateableProvider<T> : IInsertable<T> where T : class, new()
    {
        public SqlClient Context { get; internal set; }
        public EntityInfo EntityInfo { get; internal set; }
        public ISqlBuilder SqlBuilder { get; internal set; }
        public UpdateBuilder UpdateBuilder { get; set; }
        public IAdo Ado { get { return Context.Ado; } }
        public T UpdateObjs { get; set; }
        public bool IsMappingTable { get { return this.Context.MappingTables != null && this.Context.MappingTables.Any(); } }
        public bool IsMappingColumns { get { return this.Context.MappingColumns != null && this.Context.MappingColumns.Any(); } }
        public List<Column> MappingColumnList { get; set; }
        private List<string> IgnoreColumnNameList { get; set; }
        private List<string> WhereColumnList { get; set; }
        private bool IsOffIdentity { get; set; }
        public MappingTableList OldMappingTableList { get; set; }
        public bool IsAs { get; set; }
        public int ExecuteCommand()
        {
            PreToSql();
            string sql = UpdateBuilder.ToSqlString();
          
          //  Check.Exception(UpdateBuilder.WhereValues.IsNullOrEmpty() && GetPrimaryKeys().IsNullOrEmpty(), "You cannot have no primary key and no conditions");
            return this.Ado.ExecuteCommand(sql, UpdateBuilder.Parameters == null ? null : UpdateBuilder.Parameters.ToArray());
        }
     
  
        private void PreToSql()
        {
            UpdateBuilder.PrimaryKeys = GetPrimaryKeys();
           
                foreach (var item in this.UpdateBuilder.DbColumnInfoList)
                {
                    if (this.UpdateBuilder.Parameters == null) this.UpdateBuilder.Parameters = new List<Parameter>();
                    this.UpdateBuilder.Parameters.Add(new Parameter(this.SqlBuilder.SqlParameterKeyWord + item.DbColumnName, item.Value, item.PropertyType));
                }
            

            #region Identities
            List<string> identities = GetIdentityKeys();
            if (identities != null && identities.Any())
            {
                this.UpdateBuilder.DbColumnInfoList.ForEach(it =>
                {
                    var mappingInfo = identities.SingleOrDefault(i => it.DbColumnName.Equals(i, StringComparison.CurrentCultureIgnoreCase));
                    if (mappingInfo != null && mappingInfo.Any())
                    {
                        it.IsIdentity = true;
                    }
                });
            }
            #endregion
            List<string> primaryKey = GetPrimaryKeys();
            if (primaryKey != null && primaryKey.Count > 0)
            {
                this.UpdateBuilder.DbColumnInfoList.ForEach(it =>
                {
                    var mappingInfo = primaryKey.SingleOrDefault(i => it.DbColumnName.Equals(i, StringComparison.CurrentCultureIgnoreCase));
                    if (mappingInfo != null && mappingInfo.Any())
                    {
                        it.IsPrimarykey = true;
                    }
                });
            }
            if (this.UpdateBuilder.Parameters.IsValuable() && this.UpdateBuilder.SetValues.IsValuable())
            {
                this.UpdateBuilder.Parameters.RemoveAll(it => this.UpdateBuilder.SetValues.Any(v => (SqlBuilder.SqlParameterKeyWord + SqlBuilder.GetNoTranslationColumnName(v.Key)) == it.ParameterName));
            }
        }
        internal void Init()
        {
            this.UpdateBuilder.TableName = EntityInfo.EntityName;
            if (IsMappingTable)
            {
                var mappingInfo = this.Context.MappingTables.SingleOrDefault(it => it.EntityName == EntityInfo.EntityName);
                if (mappingInfo != null)
                {
                    this.UpdateBuilder.TableName = mappingInfo.DbTableName;
                }
            }
          //  Check.Exception(UpdateObjs == null || UpdateObjs.Count() == 0, "UpdateObjs is null");
            int i = 0;
           
                List<DbColumnInfo> updateItem = new List<DbColumnInfo>();
                foreach (var column in EntityInfo.Columns)
                {
                    var columnInfo = new DbColumnInfo()
                    {
                        Value = column.PropertyInfo.GetValue(UpdateObjs, null),
                        DbColumnName = GetDbColumnName(column.PropertyName),
                        PropertyName = column.PropertyName,
                        PropertyType = Extensions.GetUnderType(column.PropertyInfo),
                        TableId = i
                    };
                    if (columnInfo.PropertyType.IsEnum())
                    {
                        columnInfo.Value = Convert.ToInt64(columnInfo.Value);
                    }
                    updateItem.Add(columnInfo);
                }
                this.UpdateBuilder.DbColumnInfoList.AddRange(updateItem);
                ++i;
           
        }

        private string GetDbColumnName(string entityName)
        {
            if (!IsMappingColumns)
            {
                return entityName;
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

        private List<string> GetPrimaryKeys()
        {
            return this.EntityInfo.Columns.Where(it => it.IsPrimarykey).Select(it => it.DbColumnName).ToList();

        }

        public IInsertable<T> Where(T deleteObj)
        {
            throw new NotImplementedException();
        }
        public IInsertable<T> UpdateColumns(Func<string, bool> updateColumMethod)
        {
            List<string> primaryKeys = GetPrimaryKeys();
            foreach (var item in this.UpdateBuilder.DbColumnInfoList)
            {
                var mappingInfo = primaryKeys.SingleOrDefault(i => item.DbColumnName.Equals(i, StringComparison.CurrentCultureIgnoreCase));
                if (mappingInfo != null && mappingInfo.Any())
                {
                    item.IsPrimarykey = true;
                }
            }
            this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => updateColumMethod(it.PropertyName) || it.IsPrimarykey || it.IsIdentity).ToList();
            return this;
        }

     
    }
}
