using DotNetTN.Connector.SQL.Common;
using DotNetTN.Connector.SQL.SqlBuilderProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTN.Connector.SQL.Interface
{
    public partial interface ISqlBuilder
    {
        SqlClient Context { get; set; }
        CommandType CommandType { get; set; }
        String AppendWhereOrAnd(bool isWhere, string sqlString);
        string AppendHaving(string sqlString);
        InsertBuilder InsertBuilder { get; set; }
               SqlQueryBuilder SqlQueryBuilder { get; set; }

        UpdateBuilder UpdateBuilder { get; set; }

        DeleteBuilder DeleteBuilder { get; set; }

        string SqlParameterKeyWord { get; }
        string SqlFalse { get; }
        string SqlDateNow { get; }

        string GetTranslationTableName(string name);
        string GetTranslationColumnName(string entityName, string propertyName);
        string GetTranslationColumnName(string propertyName);
        string GetNoTranslationColumnName(string name);
        string GetPackTable(string sql, string shortName);
        void RepairReplicationParameters(ref string appendSql, Parameter[] parameters, int addIndex);
    }
}
