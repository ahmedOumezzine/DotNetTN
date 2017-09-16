using DotNetTN.Connector.SQL.ExpressionsToSql;
using System.Linq.Expressions;

namespace DotNetTN.Connector.SQL.SqlBuilderProvider
{
    public class LambdaExpressionResolve : BaseResolve
    {
        public LambdaExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            LambdaExpression lambda = base.Expression as LambdaExpression;
            var expression = lambda.Body;
            base.Expression = expression;
            base.Start();
        }
    }
}