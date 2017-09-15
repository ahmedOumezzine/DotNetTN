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
    public class DeleteBuilder 
    {
        #region Fields
        private List<string> _WhereInfos;
        #endregion

        #region Common Properties
        public EntityInfo EntityInfo { get; set; }
        public SqlClient Context { get; set; }
         public List<Parameter> Parameters { get; set; }
        public StringBuilder sql { get; set; }
        public ISqlBuilder Builder { get; set; }
        public string TableWithString { get; set; }
        public virtual List<string> WhereInfos
        {
            get
            {
                _WhereInfos = Extensions.IsNullReturnNew(_WhereInfos);
                return _WhereInfos;
            }
            set { _WhereInfos = value; }
        }
        public virtual List<object> BigDataInValues { get; set; }
        public virtual string BigDataFiled { get; set; }
        #endregion

        #region Sql Template
        public virtual string SqlTemplate
        {
            get
            {
                return "DELETE FROM {0}{1}";
            }
        }
        public virtual string WhereInTemplate
        {
            get
            {
                return "{0} IN ({1})";
            }
        }
        public string WhereInOrTemplate
        {
            get
            {
                return "OR";
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
        public string WhereInAreaTemplate
        {
            get
            {
                return "({0})";
            }
        }
        #endregion

        #region Get Sql
        public virtual string GetTableNameString
        {
            get
            {
                var result = Builder.GetTranslationTableName(EntityInfo.EntityName);
                result += UtilConstants.Space;
                if (this.TableWithString.IsValuable())
                {
                    result += TableWithString + UtilConstants.Space;
                }
                return result;
            }
        }
        public virtual string GetWhereString
        {
            get
            {
                if (_WhereInfos == null || _WhereInfos.Count == 0) return null;
                string whereString = null;
                int i = 0;
                foreach (var item in _WhereInfos)
                {
                    var isFirst = i == 0;
                    whereString += isFirst ? "WHERE " : "AND ";
                    whereString += (item + UtilConstants.Space);
                    ++i;
                }
                return whereString;
            }
        }

        #endregion

        #region Public methods
        public virtual void Clear()
        {
        }
        public virtual string ToSqlString()
        {
            if (this.BigDataInValues.IsNullOrEmpty())
            {
                return string.Format(SqlTemplate, GetTableNameString, GetWhereString);
            }
            else//big data
            {
                var whereString = GetWhereString;
                var sql = string.Format(SqlTemplate, GetTableNameString, whereString);
                sql += whereString.IsNullOrEmpty() ? " WHERE " : " AND ";
                StringBuilder batchDeleteSql = new StringBuilder();
                int pageSize = 1000;
                int pageIndex = 1;
                int totalRecord = this.BigDataInValues.Count;
                int pageCount = (totalRecord + pageSize - 1) / pageSize;
                while (pageCount >= pageIndex)
                {
                    var inValues = this.BigDataInValues.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                    batchDeleteSql.Append(sql + string.Format(WhereInTemplate, BigDataFiled, inValues.ToArray().ToJoinSqlInVals()));
                    batchDeleteSql.Append(";");
                    pageIndex++;
                }
                return batchDeleteSql.ToString();
            }
        }
     
        #endregion
    }
}
