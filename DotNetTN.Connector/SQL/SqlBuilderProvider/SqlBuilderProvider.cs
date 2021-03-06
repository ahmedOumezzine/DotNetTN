﻿using DotNetTN.Connector.SQL.Common;
using DotNetTN.Connector.SQL.Interface;
using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace DotNetTN.Connector.SQL.SqlBuilderProvider
{
    public abstract partial class SqlBuilderProvider : ISqlBuilder
    {
        public SqlBuilderProvider()
        {
        }

        #region Properties

        public SqlClient Context { get; set; }
        public CommandType CommandType { get; set; }
        public DeleteBuilder DeleteBuilder { get; set; }
        public InsertBuilder InsertBuilder { get; set; }
        public QueryBuilder QueryBuilder { get; set; }
        public UpdateBuilder UpdateBuilder { get; set; }
        //public SqlQueryBuilder SqlQueryBuilder
        //{
        //    get
        //    {
        //        base._SqlQueryBuilder = UtilMethods.IsNullReturnNew(base._SqlQueryBuilder);
        //        return base._SqlQueryBuilder;
        //    }
        //    set { base._SqlQueryBuilder = value; }
        //}

        #endregion Properties

        #region abstract Methods

        public virtual string GetTranslationTableName(string name)
        {
            //     Check.ArgumentNullException(name, string.Format(ErrorMessage.ObjNotExist, "Table Name"));
            if (name.IsContainsIn("(", ")", SqlTranslationLeft))
            {
                return name;
            }
            var context = this.Context;
            var mappingInfo = context
                .MappingTables
                .FirstOrDefault(it => it.EntityName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            name = (mappingInfo == null ? name : mappingInfo.DbTableName);
            if (name.IsContainsIn("(", ")", SqlTranslationLeft))
            {
                return name;
            }
            return SqlTranslationLeft + name + SqlTranslationRight;
        }

        public virtual string GetTranslationColumnName(string entityName, string propertyName)
        {
            //  Check.ArgumentNullException(entityName, string.Format(ErrorMessage.ObjNotExist, "Table Name"));
            // Check.ArgumentNullException(propertyName, string.Format(ErrorMessage.ObjNotExist, "Column Name"));
            var context = this.Context;
            var mappingInfo = context
                 .MappingColumns
                 .FirstOrDefault(it =>
                 it.EntityName.Equals(entityName, StringComparison.CurrentCultureIgnoreCase) &&
                 it.PropertyName.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));
            return (mappingInfo == null ? SqlTranslationLeft + propertyName + SqlTranslationRight : SqlTranslationLeft + mappingInfo.DbColumnName + SqlTranslationRight);
        }

        public virtual string GetTranslationColumnName(string propertyName)
        {
            if (propertyName.Contains(SqlTranslationLeft)) return propertyName;
            else
                return SqlTranslationLeft + propertyName + SqlTranslationRight;
        }

        public virtual string GetNoTranslationColumnName(string name)
        {
            if (!name.Contains(SqlTranslationLeft)) return name;
            return name == null ? string.Empty : Regex.Match(name, @".*" + "\\" + SqlTranslationLeft + "(.*?)" + "\\" + SqlTranslationRight + "").Groups[1].Value;
        }

        public virtual string GetPackTable(string sql, string shortName)
        {
            return Extensions.GetPackTable(sql, shortName);
        }

        public virtual void RepairReplicationParameters(ref string appendSql, Parameter[] parameters, int addIndex)
        {
            Extensions.RepairReplicationParameters(ref appendSql, parameters, addIndex);
        }

        #endregion abstract Methods

        #region Common SqlTemplate

        public string AppendWhereOrAnd(bool isWhere, string sqlString)
        {
            return isWhere ? (" WHERE " + sqlString) : (" AND " + sqlString);
        }

        public string AppendHaving(string sqlString)
        {
            return " HAVING " + sqlString;
        }

        public virtual string SqlParameterKeyWord { get { return "@"; } }
        public abstract string SqlTranslationLeft { get; }
        public abstract string SqlTranslationRight { get; }
        public virtual string SqlFalse { get { return "1=2 "; } }
        public virtual string SqlDateNow { get { return "GETDATE()"; } }

        public SqlQueryBuilder SqlQueryBuilder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion Common SqlTemplate
    }
}