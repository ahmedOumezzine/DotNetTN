using DotNetTN.Connector.SQL.Common;
using DotNetTN.Connector.SQL.Entities;
using DotNetTN.Connector.SQL.ExpressionsToSql;
using System.Linq.Expressions;

namespace DotNetTN.Connector.SQL.SqlBuilderProvider
{
    public class ConstantExpressionResolve : BaseResolve
    {
        public ConstantExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var expression = base.Expression as ConstantExpression;
            var isLeft = parameter.IsLeft;
            object value = ExpressionTool.GetValue(expression.Value);
            var baseParameter = parameter.BaseParameter;
            baseParameter.ChildExpression = expression;
            switch (parameter.Context.ResolveType)
            {
                case ResolveExpressType.SelectSingle:
                case ResolveExpressType.Update:
                case ResolveExpressType.SelectMultiple:
                    baseParameter.CommonTempData = value;
                    break;

                case ResolveExpressType.WhereSingle:
                case ResolveExpressType.WhereMultiple:
                    var parentIsBinary = parameter.BaseParameter.CurrentExpression is BinaryExpression;
                    var parentIsRoot = parameter.BaseParameter.CurrentExpression is LambdaExpression;
                    var isBool = value != null && value.GetType() == UtilConstants.BoolType;
                    if (parentIsRoot && isBool)
                    {
                        this.Context.Result.Append(value.ObjToBool() ? this.Context.DbMehtods.True() : this.Context.DbMehtods.False());
                        break;
                    }
                    if (parentIsBinary && isBool)
                    {
                        var isLogicOperator =
                           parameter.BaseExpression.NodeType == ExpressionType.And ||
                           parameter.BaseExpression.NodeType == ExpressionType.AndAlso ||
                           parameter.BaseExpression.NodeType == ExpressionType.Or ||
                           parameter.BaseExpression.NodeType == ExpressionType.OrElse;
                        if (isLogicOperator)
                        {
                            AppendMember(parameter, isLeft, (value.ObjToBool() ? this.Context.DbMehtods.True() : this.Context.DbMehtods.False()));
                            break;
                        }
                    }
                    if (value == null && parentIsBinary)
                    {
                        parameter.BaseParameter.ValueIsNull = true;
                        value = "NULL";
                    }
                    AppendValue(parameter, isLeft, value);
                    break;

                case ResolveExpressType.FieldSingle:
                case ResolveExpressType.FieldMultiple:
                default:
                    break;
            }
        }
    }
}