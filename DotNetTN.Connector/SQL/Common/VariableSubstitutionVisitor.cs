using System.Linq.Expressions;

namespace DotNetTN.Connector.SQL.Common
{
    public class VariableSubstitutionVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;

        public VariableSubstitutionVisitor(ParameterExpression parameter)
        {
            _parameter = parameter;
        }
    }
}