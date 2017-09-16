using DotNetTN.Connector.SQL.Common;
using DotNetTN.Connector.SQL.Entities;
using DotNetTN.Connector.SQL.Interface;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace DotNetTN.Connector.SQL.SqlBuilderProvider
{
    public class SqlQueryBuilder
    {
        #region Fields

        private string _Fields;
        private StringBuilder _Sql;
        private List<Parameter> _Parameters;

        #endregion Fields

        #region Properties

        public SqlClient Context { get; set; }
        public ISqlBuilder Builder { get; set; }

        public ILambdaExpressions LambdaExpressions { get; set; }

        public virtual ExpressionResult GetExpressionValue(Expression expression, ResolveExpressType resolveType)
        {
            ILambdaExpressions resolveExpress = this.LambdaExpressions;
            this.LambdaExpressions.Clear();
            resolveExpress.JoinQueryInfos = Builder.QueryBuilder.JoinQueryInfos;
            resolveExpress.IsSingle = IsSingle();
            resolveExpress.MappingColumns = Context.MappingColumns;
            resolveExpress.MappingTables = Context.MappingTables;

            resolveExpress.Resolve(expression, resolveType);
            this.Parameters.AddRange(resolveExpress.Parameters);
            var reval = resolveExpress.Result;
            return reval;
        }

        public bool IsSingle()
        {
            var isSingle = Builder.QueryBuilder.JoinQueryInfos.IsNullOrEmpty();
            return isSingle;
        }

        public string Fields
        {
            get
            {
                if (this._Fields.IsNullOrEmpty())
                {
                    this._Fields = Regex.Match(this.sql.ObjToString().Replace("\n", string.Empty).Replace("\r", string.Empty).Trim(), @"select(.*?)from", RegexOptions.IgnoreCase).Groups[1].Value;
                    if (this._Fields.IsNullOrEmpty())
                    {
                        this._Fields = "*";
                    }
                }
                return this._Fields;
            }
            set
            {
                _Fields = value;
            }
        }

        public StringBuilder sql
        {
            get
            {
                _Sql = Extensions.IsNullReturnNew(_Sql);
                return _Sql;
            }
            set
            {
                _Sql = value;
            }
        }

        public string SqlTemplate
        {
            get
            {
                return null;
            }
        }

        public List<Parameter> Parameters
        {
            get
            {
                _Parameters = Extensions.IsNullReturnNew(_Parameters);
                return _Parameters;
            }
            set
            {
                _Parameters = value;
            }
        }

        #endregion Properties

        #region Methods

        public string ToSqlString()
        {
            return sql.ToString();
        }

        public void Clear()
        {
            this.sql = null;
        }

        #endregion Methods
    }
}