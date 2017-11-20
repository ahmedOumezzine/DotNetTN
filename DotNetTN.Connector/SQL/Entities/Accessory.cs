using DotNetTN.Common.Expression;
using DotNetTN.Connector.SQL.Common;
using DotNetTN.Connector.SQL.Interface;
using DotNetTN.Connector.SQL.Mapping;
using DotNetTN.Connector.SQL.SqlBuilderProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DotNetTN.Connector.SQL.Entities
{
    public partial class Accessory
    {
        public SqlClient Context { get; set; }
        public Config CurrentConfig { get; set; }

        public MappingTableList MappingTables = new MappingTableList();
        public MappingColumnList MappingColumns = new MappingColumnList();
        public bool IsSystemTablesConfig { get { return this.CurrentConfig.InitKeyType == InitKeyType.SystemTable; } }

        protected IAdo _Ado;

        protected InsertableProvider<T> CreateInsertable<T>(T insertObjs) where T : class, new()
        {
            var reval = new InsertableProvider<T>();

            var sqlBuilder = InstanceFactory.GetSqlbuilder(this.CurrentConfig);
            reval.Context = this.Context;
            reval.EntityInfo = this.Context.EntityMaintenance.GetEntityInfo<T>();
            reval.SqlBuilder = sqlBuilder;
            reval.InsertObjs = insertObjs;
            sqlBuilder.InsertBuilder = reval.InsertBuilder = InstanceFactory.GetInsertBuilder(this.CurrentConfig);
            sqlBuilder.InsertBuilder.Builder = sqlBuilder;
            sqlBuilder.Context = reval.SqlBuilder.InsertBuilder.Context = this.Context;
            reval.Init();
            return reval;
        }

        protected UpdateableProvider<T> CreateUpdateable<T>(T UpdateObjs) where T : class, new()
        {
            var reval = new UpdateableProvider<T>();
            var sqlBuilder = InstanceFactory.GetSqlbuilder(this.CurrentConfig);
            reval.Context = this.Context;
            reval.EntityInfo = this.Context.EntityMaintenance.GetEntityInfo<T>();
            reval.SqlBuilder = sqlBuilder;
            reval.UpdateObjs = UpdateObjs;
            sqlBuilder.UpdateBuilder = reval.UpdateBuilder = InstanceFactory.GetUpdateBuilder(this.CurrentConfig);
            sqlBuilder.UpdateBuilder.Builder = sqlBuilder;
            sqlBuilder.UpdateBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(this.CurrentConfig);

            sqlBuilder.Context = reval.SqlBuilder.UpdateBuilder.Context = this.Context;
            reval.Init();
            return reval;
        }

        protected Interface.IQueryable<T> CreateQueryable<T>() where T : class, new()
        {
            Interface.IQueryable<T> result = InstanceFactory.GetQueryable<T>(this.CurrentConfig);
            return CreateQueryable(result);
        }

        protected Interface.IQueryable<T> CreateQueryable<T>(Interface.IQueryable<T> result) where T : class, new()
        {
            var sqlBuilder = InstanceFactory.GetSqlbuilder(CurrentConfig);
            result.Context = this.Context;
            result.SqlBuilder = sqlBuilder;
            result.SqlBuilder.QueryBuilder = InstanceFactory.GetQueryBuilder(CurrentConfig);
            result.SqlBuilder.QueryBuilder.Builder = sqlBuilder;
            result.SqlBuilder.Context = result.SqlBuilder.QueryBuilder.Context = this.Context;
            var EntityInfo = this.Context.EntityMaintenance.GetEntityInfo<T>();
            result.SqlBuilder.QueryBuilder.EntityType = typeof(T);
            result.SqlBuilder.QueryBuilder.EntityName = EntityInfo.DbTableName;
            result.SqlBuilder.QueryBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(CurrentConfig);
            return result;
        }

        protected DeleteableProvider<T> CreateDeleteable<T>() where T : class, new()
        {
            var reval = new DeleteableProvider<T>();
            var sqlBuilder = InstanceFactory.GetSqlbuilder(this.CurrentConfig);
            reval.Context = this.Context;
            reval.SqlBuilder = sqlBuilder;
            sqlBuilder.DeleteBuilder = reval.DeleteBuilder = InstanceFactory.GetDeleteBuilder(this.CurrentConfig);
            sqlBuilder.DeleteBuilder.Builder = sqlBuilder;
            sqlBuilder.Context = reval.SqlBuilder.DeleteBuilder.Context = this.Context;
            return reval;
        }

        protected void CreateQueryJoin<T, T2>(Expression<Func<T, T2, bool>> whereExpression, JoinType JoinType, Type[] types, Interface.IQueryable<T> queryable) where T : class, new()
        {
            this.CreateQueryable<T>(queryable);
            string TableA = string.Empty; string KeyA = string.Empty;
            string TableB = string.Empty; string KeyB = string.Empty;
            List<Parameter> paramters = new List<Parameter>();
            //
            BinaryExpression binaryExpression = whereExpression.Body as BinaryExpression;
            TableA = ((MemberExpression)binaryExpression.Left).Member.DeclaringType.CustomAttributes.ElementAt(0).ConstructorArguments.ElementAt(0).Value.ToString();
            KeyA = ((MemberExpression)binaryExpression.Left).Member.CustomAttributes.ElementAt(0).NamedArguments.FirstOrDefault(c => c.MemberName.Equals("ColumnName")).TypedValue.Value.ToString();

            Expression right = binaryExpression.Right;//right part of the "==" of your predicate
            UnaryExpression data = binaryExpression.Right as UnaryExpression;
            var exp = data.Operand;
            TableB = ((MemberExpression)exp).Member.DeclaringType.CustomAttributes.ElementAt(0).ConstructorArguments.ElementAt(0).Value.ToString();
            KeyB = ((MemberExpression)exp).Member.CustomAttributes.ElementAt(0).NamedArguments.FirstOrDefault(c => c.MemberName.Equals("ColumnName")).TypedValue.Value.ToString();

            StringBuilder sqlrequest = new StringBuilder();

            if (JoinType == JoinType.Inner)
                sqlrequest.AppendFormat("SELECT * FROM {0} INNER JOIN {1} ON {0}.{2} = {1}.{3}", TableA, TableB, KeyA, KeyB);

            if (JoinType == JoinType.Left)
                sqlrequest.AppendFormat("SELECT * FROM {0} LEFT JOIN {1} ON {0}.{2} = {1}.{3}", TableA, TableB, KeyA, KeyB);

            if (JoinType == JoinType.leftWithoutIntersection)
                sqlrequest.AppendFormat("SELECT * FROM {0} LEFT JOIN {1} ON {0}.{2} = {1}.{3} WHERE {1}.{3} IS NULL", TableA, TableB, KeyA, KeyB);

            if (JoinType == JoinType.Right)
                sqlrequest.AppendFormat("SELECT * FROM {0} RIGHT JOIN {1} ON {0}.{2} = {1}.{3}", TableA, TableB, KeyA, KeyB);

            if (JoinType == JoinType.RightWithoutIntersection)
                sqlrequest.AppendFormat("SELECT * FROM {0} RIGHT JOIN {1} ON {0}.{2} = {1}.{3} WHERE {1}.{3} IS NULL", TableA, TableB, KeyA, KeyB);

            if (JoinType == JoinType.FULL)
                sqlrequest.AppendFormat("SELECT * FROM {0} FULL JOIN {1} ON {0}.{2} = {1}.{3}", TableA, TableB, KeyA, KeyB);
            queryable.QueryBuilder.sql = sqlrequest;
        }

        #region Private methods

        protected List<JoinQueryInfo> GetJoinInfos(ISqlBuilder sqlBuilder, Expression joinExpression, ref List<Parameter> parameters, ref string shortName, params Type[] entityTypeArray)
        {
            List<JoinQueryInfo> result = new List<JoinQueryInfo>();
            var lambdaParameters = ((LambdaExpression)joinExpression).Parameters.ToList();
            ILambdaExpressions expressionContext = sqlBuilder.QueryBuilder.LambdaExpressions;
            expressionContext.MappingColumns = this.Context.MappingColumns;
            expressionContext.MappingTables = this.Context.MappingTables;
            expressionContext.Resolve(joinExpression, ResolveExpressType.Join);
            int i = 0;
            var joinArray = expressionContext.Result.GetResultArray();
            parameters = expressionContext.Parameters;
            foreach (var entityType in entityTypeArray)
            {
                var isFirst = i == 0; ++i;
                JoinQueryInfo joinInfo = new JoinQueryInfo();
                var hasMappingTable = expressionContext.MappingTables.IsValuable();
                MappingTable mappingInfo = null;
                if (hasMappingTable)
                {
                    mappingInfo = expressionContext.MappingTables.FirstOrDefault(it => it.EntityName.Equals(entityType.Name, StringComparison.CurrentCultureIgnoreCase));
                    joinInfo.TableName = mappingInfo != null ? mappingInfo.DbTableName : entityType.Name;
                }
                else
                {
                    joinInfo.TableName = entityType.Name;
                }
                if (isFirst)
                {
                    var firstItem = lambdaParameters.First();
                    lambdaParameters.Remove(firstItem);
                    shortName = firstItem.Name;
                }
                var joinString = joinArray[i * 2 - 2];
                joinInfo.ShortName = lambdaParameters[i - 1].Name;
                joinInfo.JoinType = (JoinType)Enum.Parse(typeof(JoinType), joinString);
                joinInfo.JoinWhere = joinArray[i * 2 - 1];
                joinInfo.JoinIndex = i;
                result.Add((joinInfo));
            }
            expressionContext.Clear();
            return result;
        }

        protected Dictionary<string, string> GetEasyJoinInfo(Expression joinExpression, ref string shortName, ISqlBuilder builder, params Type[] entityTypeArray)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            var lambdaParameters = ((LambdaExpression)joinExpression).Parameters.ToList();
            shortName = lambdaParameters.First().Name;
            var index = 1;
            foreach (var item in entityTypeArray)
            {
                result.Add(UtilConstants.Space + lambdaParameters[index].Name, item.Name);
                ++index;
            }
            return result;
        }

        #endregion Private methods

        protected EntityMaintenance _EntityProvider;
    }
}