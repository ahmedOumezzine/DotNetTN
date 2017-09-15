﻿using DotNetTN.Connector.SQL.Entities;
using DotNetTN.Connector.SQL.ExpressionsToSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTN.Connector.SQL.SqlBuilderProvider
{
    public class NewArrayExpessionResolve : BaseResolve
    {
        public NewArrayExpessionResolve(ExpressionParameter parameter) : base(parameter)
        {
            var expression = (NewArrayExpression)base.Expression;
            switch (base.Context.ResolveType)
            {
                case ResolveExpressType.WhereSingle:
                case ResolveExpressType.WhereMultiple:
                case ResolveExpressType.SelectSingle:
                case ResolveExpressType.SelectMultiple:
                case ResolveExpressType.FieldSingle:
                case ResolveExpressType.FieldMultiple:
                 //   Check.ThrowNotSupportedException("NewArrayExpression");
                    break;
                case ResolveExpressType.ArraySingle:
                    foreach (var item in expression.Expressions)
                    {
                        base.Expression = item;
                        base.Start();
                    }
                    break;
                case ResolveExpressType.Join:
                    base.Context.ResolveType = ResolveExpressType.WhereMultiple;
                    int i = 0;
                    foreach (var item in expression.Expressions)
                    {
                        if (item is UnaryExpression)
                        {
                            base.Expression = item;
                            base.Start();
                            if (parameter.CommonTempData is JoinType)
                            {
                                if (i > 0)
                                {
                                    base.Context.Result.Append("," + parameter.CommonTempData.ToString() + ",");
                                }
                                else
                                {
                                    base.Context.Result.Append(parameter.CommonTempData.ToString() + ",");
                                }
                                ++i;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
