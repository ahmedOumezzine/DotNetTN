using DotNetTN.Connector.SQL.Common;
using DotNetTN.Connector.SQL.Interface;
using DotNetTN.Connector.SQL.SqlBuilderProvider;

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

        protected IQueryable<T> CreateQueryable<T>() where T : class, new()
        {
            IQueryable<T> result = InstanceFactory.GetQueryable<T>(this.CurrentConfig);
            return CreateQueryable(result);
        }

        protected IQueryable<T> CreateQueryable<T>(IQueryable<T> result) where T : class, new()
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

        protected EntityMaintenance _EntityProvider;
    }
}