using DotNetTN.Connector.SQL.Common;
using DotNetTN.Connector.SQL.Interface;
using DotNetTN.Connector.SQL.SqlBuilderProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        protected EntityMaintenance _EntityProvider;


    }
}
