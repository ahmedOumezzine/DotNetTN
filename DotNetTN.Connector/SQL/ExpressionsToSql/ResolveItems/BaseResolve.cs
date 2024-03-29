﻿using DotNetTN.Connector.SQL.Common;
using DotNetTN.Connector.SQL.Entities;
using DotNetTN.Connector.SQL.ExpressionsToSql;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DotNetTN.Connector.SQL.SqlBuilderProvider
{
    public class BaseResolve
    {
        protected Expression Expression { get; set; }
        protected Expression ExactExpression { get; set; }
        public ExpressionContext Context { get; set; }
        public bool? IsLeft { get; set; }
        public int ContentIndex { get { return this.Context.Index; } }
        public int Index { get; set; }
        public ExpressionParameter BaseParameter { get; set; }

        private BaseResolve()
        {
        }

        public BaseResolve(ExpressionParameter parameter)
        {
            this.Expression = parameter.CurrentExpression;
            this.Context = parameter.Context;
            this.BaseParameter = parameter;
        }

        public BaseResolve Start()
        {
            Context.Index++;
            Expression expression = this.Expression;
            ExpressionParameter parameter = new ExpressionParameter()
            {
                Context = this.Context,
                CurrentExpression = expression,
                IsLeft = this.IsLeft,
                BaseExpression = this.ExactExpression,
                BaseParameter = this.BaseParameter,
                Index = Context.Index
            };
            if (expression is LambdaExpression)
            {
                return new LambdaExpressionResolve(parameter);
            }
            else if (expression is BinaryExpression)
            {
                return new BinaryExpressionResolve(parameter);
            }
            else if (expression is BlockExpression)
            {
                //   Check.ThrowNotSupportedException("BlockExpression");
            }
            else if (expression is ConditionalExpression)
            {
                return new ConditionalExpressionResolve(parameter);
            }
            else if (expression is MethodCallExpression)
            {
                return new MethodCallExpressionResolve(parameter);
            }
            else if (expression is ConstantExpression)
            {
                return new ConstantExpressionResolve(parameter);
            }
            else if (expression is MemberExpression)
            {
                return new MemberExpressionResolve(parameter);
            }
            else if (expression is MemberInitExpression)
            {
                return new MemberInitExpressionResolve(parameter);
            }
            else if (expression is NewExpression)
            {
                return new NewExpressionResolve(parameter);
            }
            else if (expression is NewArrayExpression)
            {
                return new NewArrayExpessionResolve(parameter);
            }
            else if (expression is ParameterExpression)
            {
                return new TypeParameterExpressionReolve(parameter);
            }
            else if (expression != null && expression.NodeType.IsIn(ExpressionType.NewArrayBounds))
            {
                //  Check.ThrowNotSupportedException("ExpressionType.NewArrayBounds");
            }
            return null;
        }

        protected void AppendMember(ExpressionParameter parameter, bool? isLeft, object appendValue)
        {
            Context.ParameterIndex++;
            if (isLeft == true)
            {
                appendValue += ExpressionConst.ExpressionReplace + parameter.BaseParameter.Index;
            }
            if (this.Context.Result.Contains(ExpressionConst.FormatSymbol))
            {
                this.Context.Result.Replace(ExpressionConst.FormatSymbol, appendValue.ObjToString());
            }
            else
            {
                this.Context.Result.Append(appendValue);
            }
        }

        protected void AppendValue(ExpressionParameter parameter, bool? isLeft, object value)
        {
            if (parameter.BaseExpression is BinaryExpression || parameter.BaseExpression == null)
            {
                var oppoSiteExpression = isLeft == true ? parameter.BaseParameter.RightExpression : parameter.BaseParameter.LeftExpression;
                if (parameter.CurrentExpression is MethodCallExpression)
                {
                    var appendValue = value;
                    if (this.Context.Result.Contains(ExpressionConst.FormatSymbol))
                    {
                        this.Context.Result.Replace(ExpressionConst.FormatSymbol, appendValue.ObjToString());
                    }
                    else
                    {
                        this.Context.Result.Append(appendValue);
                    }
                    this.AppendOpreator(parameter, isLeft);
                }
                else if (oppoSiteExpression is MemberExpression)
                {
                    string appendValue = Context.SqlParameterKeyWord
                        + ((MemberExpression)oppoSiteExpression).Member.Name
                        + Context.ParameterIndex;
                    if (value.ObjToString() != "NULL" && !parameter.ValueIsNull)
                    {
                        this.Context.Parameters.Add(new Parameter(appendValue, value));
                    }
                    else
                    {
                        appendValue = value.ObjToString();
                    }
                    Context.ParameterIndex++;
                    appendValue = string.Format(" {0} ", appendValue);
                    if (isLeft == true)
                    {
                        appendValue += ExpressionConst.ExpressionReplace + parameter.BaseParameter.Index;
                    }
                    if (this.Context.Result.Contains(ExpressionConst.FormatSymbol))
                    {
                        this.Context.Result.Replace(ExpressionConst.FormatSymbol, appendValue);
                    }
                    else
                    {
                        this.Context.Result.Append(appendValue);
                    }
                }
                else if ((oppoSiteExpression is UnaryExpression && (oppoSiteExpression as UnaryExpression).Operand is MemberExpression))
                {
                    string appendValue = Context.SqlParameterKeyWord
                      + ((MemberExpression)(oppoSiteExpression as UnaryExpression).Operand).Member.Name
                      + Context.ParameterIndex;
                    if (value.ObjToString() != "NULL" && !parameter.ValueIsNull)
                    {
                        this.Context.Parameters.Add(new Parameter(appendValue, value));
                    }
                    else
                    {
                        appendValue = value.ObjToString();
                    }
                    Context.ParameterIndex++;
                    appendValue = string.Format(" {0} ", appendValue);
                    if (isLeft == true)
                    {
                        appendValue += ExpressionConst.ExpressionReplace + parameter.BaseParameter.Index;
                    }
                    if (this.Context.Result.Contains(ExpressionConst.FormatSymbol))
                    {
                        this.Context.Result.Replace(ExpressionConst.FormatSymbol, appendValue);
                    }
                    else
                    {
                        this.Context.Result.Append(appendValue);
                    }
                }
                else
                {
                    var appendValue = this.Context.SqlParameterKeyWord + ExpressionConst.Const + Context.ParameterIndex;
                    Context.ParameterIndex++;
                    if (value != null && value.GetType().IsEnum())
                    {
                        value = Convert.ToInt64(value);
                    }
                    this.Context.Parameters.Add(new Parameter(appendValue, value));
                    appendValue = string.Format(" {0} ", appendValue);
                    if (isLeft == true)
                    {
                        appendValue += ExpressionConst.ExpressionReplace + parameter.BaseParameter.Index;
                    }
                    if (this.Context.Result.Contains(ExpressionConst.FormatSymbol))
                    {
                        this.Context.Result.Replace(ExpressionConst.FormatSymbol, appendValue);
                    }
                    else
                    {
                        this.Context.Result.Append(appendValue);
                    }
                }
            }
        }

        protected void AppendOpreator(ExpressionParameter parameter, bool? isLeft)
        {
            if (isLeft == true)
            {
                this.Context.Result.Append(" " + ExpressionConst.ExpressionReplace + parameter.BaseParameter.Index);
            }
        }

        protected string AppendParameter(object paramterValue)
        {
            string parameterName = this.Context.SqlParameterKeyWord + "constant" + this.Context.ParameterIndex;
            this.Context.ParameterIndex++; ;
            this.Context.Parameters.Add(new Parameter(parameterName, paramterValue));
            return parameterName;
        }

        protected void AppendNot(object Value)
        {
            var isAppend = !this.Context.Result.Contains(ExpressionConst.FormatSymbol);
            if (isAppend)
            {
                this.Context.Result.Append("NOT");
            }
            else
            {
                this.Context.Result.Replace(ExpressionConst.FormatSymbol, "NOT");
            }
        }

        protected MethodCallExpressionArgs GetMethodCallArgs(ExpressionParameter parameter, Expression item)
        {
            var newContext = this.Context.GetCopyContext();
            newContext.MappingColumns = this.Context.MappingColumns;
            newContext.MappingTables = this.Context.MappingTables;
            newContext.Resolve(item, this.Context.IsJoin ? ResolveExpressType.WhereMultiple : ResolveExpressType.WhereSingle);
            this.Context.Index = newContext.Index;
            this.Context.ParameterIndex = newContext.ParameterIndex;
            if (newContext.Parameters.IsValuable())
            {
                this.Context.Parameters.AddRange(newContext.Parameters);
            }
            var methodCallExpressionArgs = new MethodCallExpressionArgs()
            {
                IsMember = true,
                MemberName = newContext.Result.GetResultString()
            };
            return methodCallExpressionArgs;
        }

        protected string GetNewExpressionValue(Expression item)
        {
            var newContext = this.Context.GetCopyContext();
            newContext.Resolve(item, this.Context.IsJoin ? ResolveExpressType.WhereMultiple : ResolveExpressType.WhereSingle);
            this.Context.Index = newContext.Index;
            this.Context.ParameterIndex = newContext.ParameterIndex;
            if (newContext.Parameters.IsValuable())
            {
                this.Context.Parameters.AddRange(newContext.Parameters);
            }
            return newContext.Result.GetResultString();
        }

        protected void ResolveNewExpressions(ExpressionParameter parameter, Expression item, string asName)
        {
            if (item is ConstantExpression)
            {
                this.Expression = item;
                this.Start();
                string parameterName = this.Context.SqlParameterKeyWord + "constant" + this.Context.ParameterIndex;
                this.Context.ParameterIndex++;
                parameter.Context.Result.Append(this.Context.GetAsString(asName, parameterName));
                this.Context.Parameters.Add(new Parameter(parameterName, parameter.CommonTempData));
            }
            else if ((item is MemberExpression) && ((MemberExpression)item).Expression == null)
            {
                var paramterValue = ExpressionTool.GetPropertyValue(item as MemberExpression);
                string parameterName = this.Context.SqlParameterKeyWord + "constant" + this.Context.ParameterIndex;
                this.Context.ParameterIndex++;
                parameter.Context.Result.Append(this.Context.GetAsString(asName, parameterName));
                this.Context.Parameters.Add(new Parameter(parameterName, paramterValue));
            }
            else if ((item is MemberExpression) && ((MemberExpression)item).Expression.NodeType == ExpressionType.Constant)
            {
                this.Expression = item;
                this.Start();
                string parameterName = this.Context.SqlParameterKeyWord + "constant" + this.Context.ParameterIndex;
                this.Context.ParameterIndex++;
                parameter.Context.Result.Append(this.Context.GetAsString(asName, parameterName));
                this.Context.Parameters.Add(new Parameter(parameterName, parameter.CommonTempData));
            }
            else if (item is MemberExpression)
            {
                if (this.Context.Result.IsLockCurrentParameter == false)
                {
                    this.Context.Result.CurrentParameter = parameter;
                    this.Context.Result.IsLockCurrentParameter = true;
                    parameter.IsAppendTempDate();
                    this.Expression = item;
                    this.Start();
                    parameter.IsAppendResult();
                    this.Context.Result.Append(this.Context.GetAsString(asName, parameter.CommonTempData.ObjToString()));
                    this.Context.Result.CurrentParameter = null;
                }
            }
            else if (item is UnaryExpression && ((UnaryExpression)item).Operand is MemberExpression)
            {
                if (this.Context.Result.IsLockCurrentParameter == false)
                {
                    var expression = ((UnaryExpression)item).Operand as MemberExpression;
                    if (expression.Expression == null)
                    {
                        this.Context.Result.CurrentParameter = parameter;
                        this.Context.Result.IsLockCurrentParameter = true;
                        parameter.IsAppendTempDate();
                        this.Expression = item;
                        this.Start();
                        parameter.IsAppendResult();
                        this.Context.Result.Append(this.Context.GetAsString(asName, parameter.CommonTempData.ObjToString()));
                        this.Context.Result.CurrentParameter = null;
                    }
                    else if (expression.Expression is ConstantExpression)
                    {
                        string parameterName = this.Context.SqlParameterKeyWord + "constant" + this.Context.ParameterIndex;
                        this.Context.ParameterIndex++;
                        parameter.Context.Result.Append(this.Context.GetAsString(asName, parameterName));
                        this.Context.Parameters.Add(new Parameter(parameterName, ExpressionTool.GetMemberValue(expression.Member, expression)));
                    }
                    else
                    {
                        this.Context.Result.CurrentParameter = parameter;
                        this.Context.Result.IsLockCurrentParameter = true;
                        parameter.IsAppendTempDate();
                        this.Expression = item;
                        this.Start();
                        parameter.IsAppendResult();
                        this.Context.Result.Append(this.Context.GetAsString(asName, parameter.CommonTempData.ObjToString()));
                        this.Context.Result.CurrentParameter = null;
                    }
                }
            }
            else if (item is UnaryExpression && ((UnaryExpression)item).Operand is ConstantExpression)
            {
                if (this.Context.Result.IsLockCurrentParameter == false)
                {
                    this.Expression = ((UnaryExpression)item).Operand;
                    this.Start();
                    string parameterName = this.Context.SqlParameterKeyWord + "constant" + this.Context.ParameterIndex;
                    this.Context.ParameterIndex++;
                    parameter.Context.Result.Append(this.Context.GetAsString(asName, parameterName));
                    this.Context.Parameters.Add(new Parameter(parameterName, parameter.CommonTempData));
                }
            }
            else if (item is BinaryExpression)
            {
                if (this.Context.Result.IsLockCurrentParameter == false)
                {
                    var newContext = this.Context.GetCopyContext();
                    var resolveExpressType = this.Context.IsSingle ? ResolveExpressType.WhereSingle : ResolveExpressType.WhereMultiple;
                    newContext.Resolve(item, resolveExpressType);
                    this.Context.Index = newContext.Index;
                    this.Context.ParameterIndex = newContext.ParameterIndex;
                    if (newContext.Parameters.IsValuable())
                    {
                        this.Context.Parameters.AddRange(newContext.Parameters);
                    }
                    this.Context.Result.Append(this.Context.GetAsString(asName, newContext.Result.GetString()));
                    this.Context.Result.CurrentParameter = null;
                }
            }
            else if (item.Type.IsClass())
            {
                this.Expression = item;
                this.Start();
                var shortName = parameter.CommonTempData;
                var listProperties = item.Type.GetProperties().Cast<PropertyInfo>().ToList();
                foreach (var property in listProperties)
                {
                    if (property.PropertyType.IsClass())
                    {
                    }
                    else
                    {
                        asName = this.Context.GetTranslationText(item.Type.Name + "." + property.Name);
                        var columnName = property.Name;
                        if (Context.IsJoin)
                        {
                            this.Context.Result.Append(Context.GetAsString(asName, columnName, shortName.ObjToString()));
                        }
                        else
                        {
                            this.Context.Result.Append(Context.GetAsString(asName, columnName));
                        }
                    }
                }
            }
            else if (item is MethodCallExpression || item is UnaryExpression)
            {
                this.Expression = item;
                this.Start();
                parameter.Context.Result.Append(this.Context.GetAsString(asName, parameter.CommonTempData.ObjToString()));
            }
            else
            {
                // Check.ThrowNotSupportedException(item.GetType().Name);
            }
        }
    }
}