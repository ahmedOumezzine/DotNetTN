using DotNetTN.Connector.SQL.Common;
using DotNetTN.Connector.SQL.Entities;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DotNetTN.Connector.SQL.Interface
{
    public partial interface ILambdaExpressions
    {
        MappingColumnList MappingColumns { get; set; }
        MappingTableList MappingTables { get; set; }
        List<JoinQueryInfo> JoinQueryInfos { get; set; }
        bool IsSingle { get; set; }
        SqlClient Context { get; set; }
        IDbMethods DbMehtods { get; set; }
        Expression Expression { get; set; }
        int Index { get; set; }
        int ParameterIndex { get; set; }
        List<Parameter> Parameters { get; set; }
        ExpressionResult Result { get; set; }
        string SqlParameterKeyWord { get; }

        string GetAsString(string fieldName, string fieldValue);

        void Resolve(Expression expression, ResolveExpressType resolveType);

        void Clear();
    }
}