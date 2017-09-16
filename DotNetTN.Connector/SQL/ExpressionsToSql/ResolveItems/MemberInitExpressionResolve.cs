using DotNetTN.Connector.SQL.Common;
using DotNetTN.Connector.SQL.Entities;
using DotNetTN.Connector.SQL.ExpressionsToSql;
using System;
using System.Linq.Expressions;

namespace DotNetTN.Connector.SQL.SqlBuilderProvider
{
    public class MemberInitExpressionResolve : BaseResolve
    {
        public MemberInitExpressionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var expression = base.Expression as MemberInitExpression;
            switch (parameter.Context.ResolveType)
            {
                case ResolveExpressType.WhereSingle:
                    break;

                case ResolveExpressType.WhereMultiple:
                    break;

                case ResolveExpressType.SelectSingle:
                    Select(expression, parameter, true);
                    break;

                case ResolveExpressType.SelectMultiple:
                    Select(expression, parameter, false);
                    break;

                case ResolveExpressType.FieldSingle:
                    break;

                case ResolveExpressType.FieldMultiple:
                    break;

                case ResolveExpressType.Update:
                    Update(expression, parameter);
                    break;

                default:
                    break;
            }
        }

        private void Update(MemberInitExpression expression, ExpressionParameter parameter)
        {
            int i = 0;
            foreach (MemberBinding binding in expression.Bindings)
            {
                ++i;
                if (binding.BindingType != MemberBindingType.Assignment)
                {
                    throw new NotSupportedException();
                }
                MemberAssignment memberAssignment = (MemberAssignment)binding;
                var memberName = memberAssignment.Member.Name;
                var item = memberAssignment.Expression;

                if (item is MethodCallExpression)
                {
                    base.Expression = item;
                    base.Start();
                    parameter.Context.Result.Append(base.Context.GetEqString(memberName, parameter.CommonTempData.ObjToString()));
                }
                else if (item is MemberExpression)
                {
                    if (base.Context.Result.IsLockCurrentParameter == false)
                    {
                        base.Context.Result.CurrentParameter = parameter;
                        base.Context.Result.IsLockCurrentParameter = true;
                        parameter.IsAppendTempDate();
                        base.Expression = item;
                        base.Start();
                        parameter.IsAppendResult();
                        parameter.Context.Result.Append(base.Context.GetEqString(memberName, parameter.CommonTempData.ObjToString()));
                        base.Context.Result.CurrentParameter = null;
                    }
                }
                else if (item is BinaryExpression)
                {
                    var result = GetNewExpressionValue(item);
                    this.Context.Result.Append(base.Context.GetEqString(memberName, result));
                }
            }
        }

        private void Select(MemberInitExpression expression, ExpressionParameter parameter, bool isSingle)
        {
            foreach (MemberBinding binding in expression.Bindings)
            {
                if (binding.BindingType != MemberBindingType.Assignment)
                {
                    throw new NotSupportedException();
                }
                MemberAssignment memberAssignment = (MemberAssignment)binding;
                var memberName = memberAssignment.Member.Name;
                var item = memberAssignment.Expression;
                ResolveNewExpressions(parameter, item, memberName);
            }
        }
    }
}