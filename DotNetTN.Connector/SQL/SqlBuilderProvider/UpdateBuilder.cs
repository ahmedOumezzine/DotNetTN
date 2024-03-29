﻿using DotNetTN.Connector.SQL.Common;
using DotNetTN.Connector.SQL.Entities;
using DotNetTN.Connector.SQL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DotNetTN.Connector.SQL.SqlBuilderProvider
{
    public class UpdateBuilder
    {
        public virtual List<object> BigDataInValues { get; set; }
        public virtual string BigDataFiled { get; set; }

        public string WhereInOrTemplate
        {
            get
            {
                return "OR";
            }
        }

        public string WhereInAreaTemplate
        {
            get
            {
                return "({0})";
            }
        }

        public string WhereInAndTemplate
        {
            get
            {
                return "AND";
            }
        }

        public string WhereInEqualTemplate
        {
            get
            {
                return "[{0}]=N'{1}'";
            }
        }

        #region Fields

        private List<string> _WhereInfos;

        #endregion Fields

        public virtual List<string> WhereInfos
        {
            get
            {
                _WhereInfos = Extensions.IsNullReturnNew(_WhereInfos);
                return _WhereInfos;
            }
            set { _WhereInfos = value; }
        }

        public virtual string WhereInTemplate
        {
            get
            {
                return "{0} IN ({1})";
            }
        }

        public UpdateBuilder()
        {
            this.sql = new StringBuilder();
            this.DbColumnInfoList = new List<DbColumnInfo>();
            this.SetValues = new List<KeyValuePair<string, string>>();
            this.WhereValues = new List<string>();
            this.Parameters = new List<Parameter>();
        }

        public SqlClient Context { get; set; }
        public ILambdaExpressions LambdaExpressions { get; set; }
        public ISqlBuilder Builder { get; set; }
        public StringBuilder sql { get; set; }
        public List<Parameter> Parameters { get; set; }
        public string TableName { get; set; }
        public string TableWithString { get; set; }
        public List<DbColumnInfo> DbColumnInfoList { get; set; }
        public List<string> WhereValues { get; set; }
        public List<KeyValuePair<string, string>> SetValues { get; set; }
        public bool IsNoUpdateNull { get; set; }
        public List<string> PrimaryKeys { get; set; }
        public bool IsOffIdentity { get; set; }

        public virtual string SqlTemplate
        {
            get
            {
                return @"UPDATE {0} SET
           {1} {2}";
            }
        }

        public virtual string SqlTemplateBatch
        {
            get
            {
                return @"UPDATE S SET {0} FROM {1} S {2}   INNER JOIN ";
            }
        }

        public virtual string SqlTemplateJoin
        {
            get
            {
                return @"            (
              {0}

            ) T ON {1}
                ; ";
            }
        }

        public virtual string SqlTemplateBatchSet
        {
            get
            {
                return "{0} AS {1}";
            }
        }

        public virtual string SqlTemplateBatchSelect
        {
            get
            {
                return "{0} AS {1}";
            }
        }

        public virtual string SqlTemplateBatchUnion
        {
            get
            {
                return "\t\t\r\nUNION ALL ";
            }
        }

        public virtual void Clear()
        {
        }

        public virtual string GetTableNameString
        {
            get
            {
                var result = Builder.GetTranslationTableName(TableName);
                result += UtilConstants.Space;
                if (this.TableWithString.IsValuable())
                {
                    result += TableWithString + UtilConstants.Space;
                }
                return result;
            }
        }

        public virtual string GetTableNameStringNoWith
        {
            get
            {
                var result = Builder.GetTranslationTableName(TableName);
                return result;
            }
        }

        public virtual string ToSqlString()
        {
            if (IsNoUpdateNull)
            {
                DbColumnInfoList = DbColumnInfoList.Where(it => it.Value != null).ToList();
            }
            var groupList = DbColumnInfoList.GroupBy(it => it.TableId).ToList();
            var isSingle = groupList.Count() == 1;
            if (isSingle)
            {
                return ToSingleSqlString(groupList);
            }
            else
            {
                return TomultipleSqlString(groupList);
            }
        }

        protected virtual string TomultipleSqlString(List<IGrouping<int, DbColumnInfo>> groupList)
        {
            //   Check.Exception(PrimaryKeys == null || PrimaryKeys.Count == 0, " Update List<T> need Primary key");
            int pageSize = 200;
            int pageIndex = 1;
            int totalRecord = groupList.Count;
            int pageCount = (totalRecord + pageSize - 1) / pageSize;
            StringBuilder batchUpdateSql = new StringBuilder();
            while (pageCount >= pageIndex)
            {
                StringBuilder updateTable = new StringBuilder();
                string setValues = string.Join(",", groupList.First().Where(it => it.IsPrimarykey == false && (it.IsIdentity == false || (IsOffIdentity && it.IsIdentity))).Select(it =>
                {
                    if (SetValues.IsValuable())
                    {
                        var setValue = SetValues.Where(sv => sv.Key == Builder.GetTranslationColumnName(it.DbColumnName));
                        if (setValue != null && setValue.Any())
                        {
                            return setValue.First().Value;
                        }
                    }
                    var result = string.Format("S.{0}=T.{0}", Builder.GetTranslationColumnName(it.DbColumnName));
                    return result;
                }));
                batchUpdateSql.AppendFormat(SqlTemplateBatch.ToString(), setValues, GetTableNameStringNoWith, TableWithString);
                int i = 0;
                foreach (var columns in groupList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList())
                {
                    var isFirst = i == 0;
                    if (!isFirst)
                    {
                        updateTable.Append(SqlTemplateBatchUnion);
                    }
                    updateTable.Append("\r\n SELECT " + string.Join(",", columns.Select(it => string.Format(SqlTemplateBatchSelect, FormatValue(it.Value), Builder.GetTranslationColumnName(it.DbColumnName)))));
                    ++i;
                }
                pageIndex++;
                updateTable.Append("\r\n");
                string whereString = null;
                if (this.WhereValues.IsValuable())
                {
                    foreach (var item in WhereValues)
                    {
                        var isFirst = whereString == null;
                        whereString += (isFirst ? null : " AND ");
                        whereString += item;
                    }
                }
                else if (PrimaryKeys.IsValuable())
                {
                    foreach (var item in PrimaryKeys)
                    {
                        var isFirst = whereString == null;
                        whereString += (isFirst ? null : " AND ");
                        whereString += string.Format("S.{0}=T.{0}", Builder.GetTranslationColumnName(item));
                    }
                }
                batchUpdateSql.AppendFormat(SqlTemplateJoin, updateTable, whereString);
            }
            return batchUpdateSql.ToString();
        }

        protected virtual string ToSingleSqlString(List<IGrouping<int, DbColumnInfo>> groupList)
        {
            string columnsString = string.Join(",", groupList.First().Where(it => it.IsPrimarykey == false && (it.IsIdentity == false || (IsOffIdentity && it.IsIdentity))).Select(it =>
            {
                if (SetValues.IsValuable())
                {
                    var setValue = SetValues.Where(sv => it.IsPrimarykey == false && (it.IsIdentity == false || (IsOffIdentity && it.IsIdentity))).Where(sv => sv.Key == Builder.GetTranslationColumnName(it.DbColumnName));
                    if (setValue != null && setValue.Any())
                    {
                        return setValue.First().Value;
                    }
                }
                var result = Builder.GetTranslationColumnName(it.DbColumnName) + "=" + this.Context.Ado.SqlParameterKeyWord + it.DbColumnName;
                return result;
            }));
            string whereString = null;
            if (this.WhereValues.IsValuable())
            {
                foreach (var item in WhereValues)
                {
                    var isFirst = whereString == null;
                    whereString += (isFirst ? " WHERE " : " AND ");
                    whereString += item;
                }
            }
            else if (PrimaryKeys.IsValuable())
            {
                foreach (var item in PrimaryKeys)
                {
                    var isFirst = whereString == null;
                    whereString += (isFirst ? " WHERE " : " AND ");
                    whereString += Builder.GetTranslationColumnName(item) + "=" + this.Context.Ado.SqlParameterKeyWord + item;
                }
            }
            return string.Format(SqlTemplate, GetTableNameString, columnsString, whereString);
        }

        public virtual ExpressionResult GetExpressionValue(Expression expression, ResolveExpressType resolveType, bool isMapping = true)
        {
            ILambdaExpressions resolveExpress = this.LambdaExpressions;
            this.LambdaExpressions.Clear();
            if (isMapping)
            {
                resolveExpress.MappingColumns = Context.MappingColumns;
                resolveExpress.MappingTables = Context.MappingTables;
            }
            resolveExpress.Resolve(expression, resolveType);
            this.Parameters.AddRange(resolveExpress.Parameters);
            var reval = resolveExpress.Result;
            return reval;
        }

        public virtual object FormatValue(object value)
        {
            if (value == null)
            {
                return "NULL";
            }
            else
            {
                var type = value.GetType();
                if (type == UtilConstants.DateType)
                {
                    var date = value.ObjToDate();
                    if (date < Convert.ToDateTime("1900-1-1"))
                    {
                        date = Convert.ToDateTime("1900-1-1");
                    }
                    return "'" + date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
                }
                else if (type.IsEnum())
                {
                    return Convert.ToInt64(value);
                }
                else if (type == UtilConstants.BoolType)
                {
                    return value.ObjToBool() ? "1" : "0";
                }
                else if (type == UtilConstants.StringType || type == UtilConstants.ObjType)
                {
                    return "N'" + value.ToString().ToSqlFilter() + "'";
                }
                else
                {
                    return "N'" + value.ToString() + "'";
                }
            }
        }
    }
}