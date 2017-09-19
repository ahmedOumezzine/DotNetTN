using DotNetTN.Connector.SQL.Common;
using DotNetTN.Connector.SQL.Entities;
using DotNetTN.Connector.SQL.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DotNetTN.Connector.SQL.SqlBuilderProvider
{
    public class QueryableAccessory
    {
        protected ILambdaExpressions _LambdaExpressions;
        protected bool _RestoreMapping = true;
        protected int _InQueryableIndex = 100;
    }

    public partial class QueryableProvider<T> : QueryableAccessory, Interface.IQueryable<T>
    {
        public SqlClient Context { get; set; }
        public IAdo Db { get { return Context.Ado; } }

        public IDbBind Bind { get { return this.Db.DbBind; } }
        public ISqlBuilder SqlBuilder { get; set; }

        public MappingTableList OldMappingTableList { get; set; }
        public MappingTableList QueryableMappingTableList { get; set; }
        public bool IsAs { get; set; }

        public QueryBuilder QueryBuilder
        {
            get
            {
                return this.SqlBuilder.QueryBuilder;
            }
            set
            {
                this.SqlBuilder.QueryBuilder = value;
            }
        }

        public EntityInfo EntityInfo
        {
            get
            {
                return this.Context.EntityMaintenance.GetEntityInfo<T>();
            }
        }

        public void Clear()
        {
            QueryBuilder.Clear();
        }

        public virtual Interface.IQueryable<T> AS<T2>(string tableName)
        {
            var entityName = typeof(T2).Name;
            return _As(tableName, entityName);
        }

        public Interface.IQueryable<T> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            return _As(tableName, entityName);
        }

        public virtual Interface.IQueryable<T> With(string withString)
        {
            QueryBuilder.TableWithString = withString;
            return this;
        }

        public virtual Interface.IQueryable<T> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }

        public virtual Interface.IQueryable<T> AddParameters(Parameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }

        public virtual Interface.IQueryable<T> AddParameters(List<Parameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }

        public virtual Interface.IQueryable<T> AddParameters(Parameter parameter)
        {
            if (parameter != null)
                QueryBuilder.Parameters.Add(parameter);
            return this;
        }

        public virtual Interface.IQueryable<T> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {
            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }

        public virtual Interface.IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            BinaryExpression binaryExpression = expression.Body as BinaryExpression;
            string Left = ((MemberExpression)binaryExpression.Left).Member.Name;
            Expression right = binaryExpression.Right;//right part of the "==" of your predicate
            var objectMember = Expression.Convert(right, typeof(object));//convert to object, as we don't know what's in

            var getterLambda = Expression.Lambda<Func<object>>(objectMember);

            var getter = getterLambda.Compile();



            var valueYouWant = getter();//here's the "x" or "y"
            var sql = Left + " = " + valueYouWant;
           // var sql = this.CreateWhereClause(expression);

            QueryBuilder.WhereInfos.Add(SqlBuilder.AppendWhereOrAnd(true, sql));

          //  this._Where(expression);

            return this;
        }

        private string CreateWhereClause(Expression<Func<T, bool>> predicate)
        {
            StringBuilder p = new StringBuilder(predicate.Body.ToString());
            var pName = predicate.Parameters.First();
            p.Replace(pName.Name + ".", "");
            p.Replace("==", "=");
            p.Replace("AndAlso", "and");
            p.Replace("OrElse", "or");
            p.Replace("\"", "\'");
            p.Replace("(", " ");
            p.Replace(")", "");
            return p.ToString();
        }

        public virtual Interface.IQueryable<T> Where(string whereString, object whereObj = null)
        {
            if (whereString.IsValuable())
                this.Where<T>(whereString, whereObj);
            return this;
        }

        public virtual Interface.IQueryable<T> Where<T2>(string whereString, object whereObj = null)
        {
            var whereValue = QueryBuilder.WhereInfos;
            whereValue.Add(SqlBuilder.AppendWhereOrAnd(whereValue.Count == 0, whereString + UtilConstants.Space));
            if (whereObj != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(whereObj));
            return this;
        }

        public virtual Interface.IQueryable<T> Having(Expression<Func<T, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public virtual Interface.IQueryable<T> Having(string whereString, object parameters = null)
        {
            QueryBuilder.HavingInfos = SqlBuilder.AppendHaving(whereString);
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }

        public virtual Interface.IQueryable<T> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (!isWhere) return this;
            _Where(expression);
            return this;
        }

        public virtual Interface.IQueryable<T> WhereIF(bool isWhere, string whereString, object whereObj = null)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }

        public virtual T InSingle(object pkValue)
        {
            var list = In(pkValue).ToList();
            if (list == null) return default(T);
            else return list.SingleOrDefault();
        }

        public virtual Interface.IQueryable<T> In<TParamter>(params TParamter[] pkValues)
        {
            if (pkValues == null || pkValues.Length == 0)
            {
                Where(SqlBuilder.SqlFalse);
                return this;
            }
            var pks = GetPrimaryKeys().Select(it => SqlBuilder.GetTranslationTableName(it)).ToList();
            //    Check.Exception(pks == null || pks.Count != 1, "Queryable.In(params object[] pkValues): Only one primary key");
            string filed = pks.FirstOrDefault();
            string shortName = QueryBuilder.TableShortName == null ? null : (QueryBuilder.TableShortName + ".");
            filed = shortName + filed;
            return In(filed, pkValues);
        }

        protected List<string> GetPrimaryKeys()
        {
            return this.EntityInfo.Columns.Where(it => it.IsPrimarykey).Select(it => it.DbColumnName).ToList();
        }

        public virtual Interface.IQueryable<T> In<FieldType>(string filed, params FieldType[] inValues)
        {
            if (inValues.Length == 1)
            {
                if (inValues.GetType().IsArray)
                {
                    var whereIndex = QueryBuilder.WhereIndex;
                    string parameterName = this.SqlBuilder.SqlParameterKeyWord + "InPara" + whereIndex;
                    this.AddParameters(new Parameter(parameterName, inValues[0]));
                    this.Where(string.Format(QueryBuilder.InTemplate, filed, parameterName));
                    QueryBuilder.WhereIndex++;
                }
                else
                {
                    var values = new List<object>();
                    foreach (var item in ((IEnumerable)inValues[0]))
                    {
                        if (item != null)
                        {
                            values.Add(item.ToString().ToSqlValue());
                        }
                    }
                    this.Where(string.Format(QueryBuilder.InTemplate, filed, string.Join(",", values)));
                }
            }
            else
            {
                var values = new List<object>();
                foreach (var item in inValues)
                {
                    if (item != null)
                    {
                        values.Add(item.ToString().ToSqlValue());
                    }
                }
                this.Where(string.Format(QueryBuilder.InTemplate, filed, string.Join(",", values)));
            }
            return this;
        }

        public virtual Interface.IQueryable<T> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            return In(fieldName, inValues);
        }

        public virtual Interface.IQueryable<T> In<TParamter>(List<TParamter> pkValues)
        {
            if (pkValues == null || pkValues.Count == 0)
            {
                Where(SqlBuilder.SqlFalse);
                return this;
            }
            return In(pkValues.ToArray());
        }

        protected Interface.IQueryable<T> _As(string tableName, string entityName)
        {
            IsAs = true;
            OldMappingTableList = this.Context.MappingTables;
            this.Context.MappingTables = this.Context.Utilities.TranslateCopy(this.Context.MappingTables);
            this.Context.MappingTables.Add(entityName, tableName);
            this.QueryableMappingTableList = this.Context.MappingTables;
            return this;
        }

        public virtual Interface.IQueryable<T> In<FieldType>(string InFieldName, List<FieldType> inValues)
        {
            if (inValues == null || inValues.Count == 0)
            {
                Where(SqlBuilder.SqlFalse);
                return this;
            }
            return In(InFieldName, inValues.ToArray());
        }

        public virtual Interface.IQueryable<T> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            if (inValues == null || inValues.Count == 0)
            {
                Where(SqlBuilder.SqlFalse);
                return this;
            }
            return In(expression, inValues.ToArray());
        }

        public virtual Interface.IQueryable<T> OrderBy(string orderFileds)
        {
            var orderByValue = QueryBuilder.OrderByValue;
            if (QueryBuilder.OrderByValue.IsNullOrEmpty())
            {
                QueryBuilder.OrderByValue = QueryBuilder.OrderByTemplate;
            }
            QueryBuilder.OrderByValue += string.IsNullOrEmpty(orderByValue) ? orderFileds : ("," + orderFileds);
            return this;
        }

        public virtual Interface.IQueryable<T> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            this._OrderBy(expression, type);
            return this;
        }

        public virtual Interface.IQueryable<T> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }

        public virtual Interface.IQueryable<T> GroupBy(string groupFileds)
        {
            var croupByValue = QueryBuilder.GroupByValue;
            if (QueryBuilder.GroupByValue.IsNullOrEmpty())
            {
                QueryBuilder.GroupByValue = QueryBuilder.GroupByTemplate;
            }
            QueryBuilder.GroupByValue += string.IsNullOrEmpty(croupByValue) ? groupFileds : ("," + groupFileds);
            return this;
        }

        public virtual Interface.IQueryable<T> PartitionBy(Expression<Func<T, object>> expression)
        {
            if (QueryBuilder.Take == null)
                QueryBuilder.Take = 1;
            _PartitionBy(expression);
            return this;
        }

        public virtual Interface.IQueryable<T> PartitionBy(string groupFileds)
        {
            var partitionByValue = QueryBuilder.PartitionByValue;
            if (QueryBuilder.PartitionByValue.IsNullOrEmpty())
            {
                QueryBuilder.PartitionByValue = QueryBuilder.PartitionByTemplate;
            }
            QueryBuilder.PartitionByValue += string.IsNullOrEmpty(partitionByValue) ? groupFileds : ("," + groupFileds);
            return this;
        }

        public virtual Interface.IQueryable<T> Skip(int num)
        {
            QueryBuilder.Skip = num;
            return this;
        }

        public virtual Interface.IQueryable<T> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }

        public virtual T Single()
        {
            if (QueryBuilder.OrderByValue.IsNullOrEmpty())
            {
                QueryBuilder.OrderByValue = QueryBuilder.DefaultOrderByTemplate;
            }
            var oldSkip = QueryBuilder.Skip;
            var oldTake = QueryBuilder.Take;
            var oldOrderBy = QueryBuilder.OrderByValue;
            QueryBuilder.Skip = null;
            QueryBuilder.Take = null;
            QueryBuilder.OrderByValue = null;
            var reval = this.ToList();
            QueryBuilder.Skip = oldSkip;
            QueryBuilder.Take = oldTake;
            QueryBuilder.OrderByValue = oldOrderBy;
            if (reval == null || reval.Count == 0)
            {
                return default(T);
            }
            else if (reval.Count == 2)
            {
                //    Check.Exception(true, ".Single()  result must not exceed one . You can use.First()");
                return default(T);
            }
            else
            {
                return reval.SingleOrDefault();
            }
        }

        public virtual T Single(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            var result = Single();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }

        public virtual T First()
        {
            if (QueryBuilder.OrderByValue.IsNullOrEmpty())
            {
                QueryBuilder.OrderByValue = QueryBuilder.DefaultOrderByTemplate;
            }
            QueryBuilder.Skip = 0;
            QueryBuilder.Take = 1;
            var reval = this.ToList();
            if (reval.IsValuable())
            {
                return reval.FirstOrDefault();
            }
            else
            {
                return default(T);
            }
        }

        public virtual T First(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            var result = First();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }

        public virtual bool Any(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            var result = Any();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }

        public virtual bool Any()
        {
            return this.Count() > 0;
        }

        public virtual Interface.IQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }

        public virtual Interface.IQueryable<TResult> Select<TResult>(string selectValue)
        {
            var reval = InstanceFactory.GetQueryable<TResult>(this.Context.CurrentConfig);
            reval.Context = this.Context;
            reval.SqlBuilder = this.SqlBuilder;
            QueryBuilder.SelectValue = selectValue;
            return reval;
        }

        public virtual Interface.IQueryable<T> Select(string selectValue)
        {
            QueryBuilder.SelectValue = selectValue;
            return this;
        }

        public virtual Interface.IQueryable<T> MergeTable()
        {
            //  Check.Exception(this.QueryBuilder.SelectValue.IsNullOrEmpty(), "MergeTable need to use Select(it=>new{}) Method .");
            // Check.Exception(this.QueryBuilder.Skip > 0 || this.QueryBuilder.Take > 0, "MergeTable  Queryable cannot Take Skip OrderBy PageToList  ");
            var sql = QueryBuilder.ToSqlString();
            var tableName = this.SqlBuilder.GetPackTable(sql, "MergeTable");
            return this.Context.Queryable<ExpandoObject>().AS(tableName).Select<T>("*");
        }

        public virtual int Count()
        {
            QueryBuilder.IsCount = true;
            var sql = string.Empty;
            sql = QueryBuilder.ToSqlString();
            sql = QueryBuilder.ToCountSql(sql);
            var reval = Context.Ado.GetInt(sql, QueryBuilder.Parameters.ToArray());
            QueryBuilder.IsCount = false;
            return reval;
        }

        public virtual int Count(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            var result = Count();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }

        public virtual TResult Max<TResult>(string maxField)
        {
            this.Select(string.Format(QueryBuilder.MaxTemplate, maxField));
            var reval = this._ToList<TResult>().SingleOrDefault();
            return reval;
        }

        public virtual TResult Max<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }

        public virtual TResult Min<TResult>(string minField)
        {
            this.Select(string.Format(QueryBuilder.MinTemplate, minField));
            var reval = this._ToList<TResult>().SingleOrDefault();
            return reval;
        }

        public virtual TResult Min<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }

        public virtual TResult Sum<TResult>(string sumField)
        {
            this.Select(string.Format(QueryBuilder.SumTemplate, sumField));
            var reval = this._ToList<TResult>().SingleOrDefault();
            return reval;
        }

        public virtual TResult Sum<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }

        public virtual TResult Avg<TResult>(string avgField)
        {
            this.Select(string.Format(QueryBuilder.AvgTemplate, avgField));
            var reval = this._ToList<TResult>().SingleOrDefault();
            return reval;
        }

        public virtual TResult Avg<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }

        public virtual string ToJson()
        {
            return this.Context.Utilities.SerializeObject(this.ToList());
        }

        public virtual string ToJsonPage(int pageIndex, int pageSize)
        {
            return this.Context.Utilities.SerializeObject(this.ToPageList(pageIndex, pageSize));
        }

        public virtual string ToJsonPage(int pageIndex, int pageSize, ref int totalNumber)
        {
            return this.Context.Utilities.SerializeObject(this.ToPageList(pageIndex, pageSize, ref totalNumber));
        }

        public virtual DataTable ToDataTable()
        {
            var sqlObj = this.ToSql();
            var result = this.Db.GetDataTable(sqlObj.Key, sqlObj.Value.ToArray());
            return result;
        }

        public virtual DataTable ToDataTablePage(int pageIndex, int pageSize)
        {
            if (pageIndex == 0)
                pageIndex = 1;
            if (QueryBuilder.PartitionByValue.IsValuable())
            {
                QueryBuilder.ExternalPageIndex = pageIndex;
                QueryBuilder.ExternalPageSize = pageSize;
            }
            else
            {
                QueryBuilder.Skip = (pageIndex - 1) * pageSize;
                QueryBuilder.Take = pageSize;
            }
            return ToDataTable();
        }

        public virtual DataTable ToDataTablePage(int pageIndex, int pageSize, ref int totalNumber)
        {
            _RestoreMapping = false;
            totalNumber = this.Count();
            var result = ToDataTablePage(pageIndex, pageSize);
            _RestoreMapping = true;
            return result;
        }

        public virtual List<T> ToList()
        {
            return _ToList<T>();
        }

        public virtual List<T> ToPageList(int pageIndex, int pageSize)
        {
            if (pageIndex == 0)
                pageIndex = 1;
            if (QueryBuilder.PartitionByValue.IsValuable())
            {
                QueryBuilder.ExternalPageIndex = pageIndex;
                QueryBuilder.ExternalPageSize = pageSize;
            }
            else
            {
                QueryBuilder.Skip = (pageIndex - 1) * pageSize;
                QueryBuilder.Take = pageSize;
            }
            return ToList();
        }

        public virtual List<T> ToPageList(int pageIndex, int pageSize, ref int totalNumber)
        {
            _RestoreMapping = false;
            List<T> result = null;
            totalNumber = this.Count();
            if (totalNumber == 0)
                result = new List<T>();
            else
                result = ToPageList(pageIndex, pageSize);
            _RestoreMapping = true;
            return result;
        }

        public virtual KeyValuePair<string, List<Parameter>> ToSql()
        {
            string sql = QueryBuilder.ToSqlString();
            return new KeyValuePair<string, List<Parameter>>(sql, QueryBuilder.Parameters);
        }

        #region Private Methods

        protected Interface.IQueryable<TResult> _Select<TResult>(Expression expression)
        {
            var reval = InstanceFactory.GetQueryable<TResult>(this.Context.CurrentConfig);
            reval.Context = this.Context;
            reval.SqlBuilder = this.SqlBuilder;
            reval.SqlBuilder.QueryBuilder.Parameters = QueryBuilder.Parameters;
            reval.SqlBuilder.QueryBuilder.SelectValue = expression;
            return reval;
        }

        protected void _Where(Expression expression)
        {
        }

        protected Interface.IQueryable<T> _OrderBy(Expression expression, OrderByType type = OrderByType.Asc)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            OrderBy(lamResult.GetResultString() + UtilConstants.Space + type.ToString().ToUpper());
            return this;
        }

        protected Interface.IQueryable<T> _GroupBy(Expression expression)
        {
            LambdaExpression lambda = expression as LambdaExpression;
            expression = lambda.Body;
            var isSingle = QueryBuilder.IsSingle();
            ExpressionResult lamResult = null;
            string result = null;
            if (expression is NewExpression)
            {
                lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.ArraySingle : ResolveExpressType.ArrayMultiple);
                result = string.Join(",", lamResult.GetResultArray().Select(it => it));
            }
            else
            {
                lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
                result = lamResult.GetResultString();
            }
            GroupBy(result);
            return this;
        }

        protected TResult _Min<TResult>(Expression expression)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            return Min<TResult>(lamResult.GetResultString());
        }

        protected TResult _Avg<TResult>(Expression expression)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            return Avg<TResult>(lamResult.GetResultString());
        }

        protected TResult _Max<TResult>(Expression expression)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            return Max<TResult>(lamResult.GetResultString());
        }

        protected TResult _Sum<TResult>(Expression expression)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            return Sum<TResult>(lamResult.GetResultString());
        }

        public Interface.IQueryable<T> _PartitionBy(Expression expression)
        {
            LambdaExpression lambda = expression as LambdaExpression;
            expression = lambda.Body;
            var isSingle = QueryBuilder.IsSingle();
            ExpressionResult lamResult = null;
            string result = null;
            if (expression is NewExpression)
            {
                lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.ArraySingle : ResolveExpressType.ArrayMultiple);
                result = string.Join(",", lamResult.GetResultArray());
            }
            else
            {
                lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
                result = lamResult.GetResultString();
            }
            PartitionBy(result);
            return this;
        }

        protected Interface.IQueryable<T> _Having(Expression expression)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.WhereSingle : ResolveExpressType.WhereMultiple);
            Having(lamResult.GetResultString());
            return this;
        }

        protected List<TResult> _ToList<TResult>()
        {
            List<TResult> result = null;
            var sqlObj = this.ToSql();
            var isComplexModel = QueryBuilder.IsComplexModel(sqlObj.Key);
            var entityType = typeof(TResult);
            var dt = this.Db.GetDataTable(sqlObj.Key, sqlObj.Value.ToArray());
            List<TResult> list = ConvertDataTable<TResult>(dt);

            return list;
        }

        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

        protected void _InQueryable(Expression<Func<T, object>> expression, KeyValuePair<string, List<Parameter>> sqlObj)
        {
            string sql = sqlObj.Key;
            if (sqlObj.Value.IsValuable())
            {
                this.SqlBuilder.RepairReplicationParameters(ref sql, sqlObj.Value.ToArray(), 100);
                this.QueryBuilder.Parameters.AddRange(sqlObj.Value);
            }
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            var whereSql = string.Format(this.QueryBuilder.InTemplate, fieldName, sql);
            this.QueryBuilder.WhereInfos.Add(SqlBuilder.AppendWhereOrAnd(this.QueryBuilder.WhereInfos.IsNullOrEmpty(), whereSql));
            base._InQueryableIndex += 100;
        }

        #endregion Private Methods
    }
}