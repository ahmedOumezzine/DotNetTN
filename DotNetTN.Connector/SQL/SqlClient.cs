﻿using DotNetTN.Connector.SQL.Common;
using DotNetTN.Connector.SQL.Entities;
using DotNetTN.Connector.SQL.Interface;
using System;
using System.Linq.Expressions;

namespace DotNetTN.Connector.SQL
{
    public class SqlClient : Accessory
    {
        public Config Config { get; set; }

        public SqlClient(Config config)
        {
            if (config == null)
                throw new Exception("config is null");

            base.CurrentConfig = config;
            base.Context = this;
            switch (config.DbType)
            {
                case DbType.MySql:
                    GlobalProvider.TryMySqlData();
                    break;

                case DbType.SqlServer:
                    // en developed
                    break;

                case DbType.Sqlite:
                    // en developed
                    break;

                case DbType.Oracle:
                // en developed
                default:
                    throw new Exception("ConnectionConfig.DbType is null");
            }
        }

        protected IRewritableMethods _RewritableMethods;

        public virtual IAdo Ado
        {
            get
            {
                if (_Ado == null)
                {
                    var reval = InstanceFactory.GetAdo(base.CurrentConfig);
                    //Check.ConnectionConfig(base.CurrentConnectionConfig);
                    _Ado = reval;
                    reval.Context = this;
                    return reval;
                }
                return _Ado;
            }
        }

        #region Util Methods

        [Obsolete("Use SqlClient.Utilities")]
        public virtual IRewritableMethods RewritableMethods
        {
            get { return this.Utilities; }
            set { this.Utilities = value; }
        }

        public virtual IRewritableMethods Utilities
        {
            get
            {
                if (this._RewritableMethods == null)
                    this._RewritableMethods = new RewritableMethods();
                return _RewritableMethods;
            }
            set { this._RewritableMethods = value; }
        }

        #endregion Util Methods

        #region Entity Methods

        [Obsolete("Use .EntityMaintenance")]
        public virtual EntityMaintenance EntityProvider
        {
            get { return this.EntityMaintenance; }
            set { this.EntityMaintenance = value; }
        }

        public virtual EntityMaintenance EntityMaintenance
        {
            get
            {
                if (base._EntityProvider == null)
                {
                    base._EntityProvider = new EntityMaintenance();
                    base._EntityProvider.Context = this;
                }
                return _EntityProvider;
            }
            set { base._EntityProvider = value; }
        }

        #endregion Entity Methods

        public virtual IInsertable<T> Insertable<T>(T insertObj) where T : class, new()
        {
            return base.CreateInsertable(insertObj);
        }

        public virtual IInsertable<T> Updateable<T>(T UpdateObjs) where T : class, new()
        {
            return base.CreateUpdateable(UpdateObjs);
        }

        public virtual IInsertable<T> Deleteable<T>() where T : class, new()
        {
            return base.CreateDeleteable<T>();
        }

        public virtual IQueryable<T> Queryable<T>() where T : class, new()
        {
            var result = base.CreateQueryable<T>();
            return result;
        }

        public virtual IQueryable<T, T2> Queryable<T, T2>(JoinType JoinType, Expression<Func<T, T2, bool>> whereExpression) where T : class, new()
        {
            var types = new Type[] { typeof(T2) };
            var queryable = InstanceFactory.GetQueryable<T, T2>(base.CurrentConfig);
            base.CreateQueryJoin(whereExpression, JoinType, types, queryable);
            return queryable;
        }
    }
}