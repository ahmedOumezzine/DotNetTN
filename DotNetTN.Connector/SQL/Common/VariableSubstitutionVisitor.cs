using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
