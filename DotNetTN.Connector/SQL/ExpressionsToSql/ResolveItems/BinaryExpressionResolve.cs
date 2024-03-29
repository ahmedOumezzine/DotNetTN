﻿using DotNetTN.Connector.SQL.ExpressionsToSql;
using System.Linq.Expressions;

namespace DotNetTN.Connector.SQL.SqlBuilderProvider
{
    public class BinaryExpressionResolve : BaseResolve
    {
        public BinaryExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var expression = this.Expression as BinaryExpression;
            var operatorValue = parameter.OperatorValue = ExpressionTool.GetOperator(expression.NodeType);
            var isEqual = expression.NodeType == ExpressionType.Equal;
            var isComparisonOperator = ExpressionTool.IsComparisonOperator(expression);
            base.ExactExpression = expression;
            var leftExpression = expression.Left;
            var rightExpression = expression.Right;
            var leftIsBinary = leftExpression is BinaryExpression;
            var rightBinary = rightExpression is BinaryExpression;
            var lbrs = leftIsBinary && !rightBinary;
            var lsrb = !leftIsBinary && rightBinary;
            var lbrb = rightBinary && leftIsBinary;
            var lsbs = !leftIsBinary && !rightBinary;
            var isAppend = !base.Context.Result.Contains(ExpressionConst.FormatSymbol);
            if (isAppend)
            {
                base.Context.Result.Append(ExpressionConst.LeftParenthesis);
                base.Context.Result.Append(ExpressionConst.FormatSymbol);
            }
            else
            {
                base.Context.Result.Replace(ExpressionConst.FormatSymbol, ExpressionConst.LeftParenthesis + ExpressionConst.FormatSymbol);
            }
            parameter.LeftExpression = leftExpression;
            parameter.RightExpression = rightExpression;
            base.Expression = leftExpression;
            base.IsLeft = true;
            base.Start();
            base.IsLeft = false;
            base.Expression = rightExpression;
            base.Start();
            base.IsLeft = null;
            if (lsbs && parameter.ValueIsNull)
            {
                base.Context.Result.Replace(ExpressionConst.ExpressionReplace + parameter.Index, isEqual ? "IS" : "IS NOT");
                base.Context.Result.Replace(ExpressionConst.ExpressionReplace + (parameter.Index + 1), isEqual ? "IS" : "IS NOT");
            }
            else
            {
                base.Context.Result.Replace(ExpressionConst.ExpressionReplace + parameter.Index, operatorValue);
                base.Context.Result.Replace(ExpressionConst.ExpressionReplace + (parameter.Index + 1), operatorValue);
            }
            base.Context.Result.Append(ExpressionConst.RightParenthesis);
            if (parameter.BaseExpression is BinaryExpression && parameter.IsLeft == true)
            {
                base.Context.Result.Append(" " + ExpressionConst.ExpressionReplace + parameter.BaseParameter.Index + " ");
            }
        }
    }
}