﻿using DotNetTN.Connector.SQL.Common;
using DotNetTN.Connector.SQL.Entities;
using DotNetTN.Connector.SQL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace DotNetTN.Connector.SQL.AdoProvider
{
    public abstract partial class AdoProvider : AdoAccessory, IAdo
    {
        #region Constructor

        public AdoProvider()
        {
            this.IsEnableLogEvent = false;
            this.CommandType = CommandType.Text;
            this.IsClearParameters = true;
            this.CommandTimeOut = 30000;
        }

        #endregion Constructor

        #region Properties

        protected List<IDataParameter> OutputParameters { get; set; }
        public virtual string SqlParameterKeyWord { get { return "@"; } }
        public IDbTransaction Transaction { get; set; }
        public virtual SqlClient Context { get; set; }
        internal CommandType OldCommandType { get; set; }
        internal bool OldClearParameters { get; set; }
        public IDataParameterCollection DataReaderParameters { get; set; }

        public virtual IDbBind DbBind
        {
            get
            {
                if (base._DbBind == null)
                {
                    IDbBind bind = InstanceFactory.GetDbBind(this.Context.CurrentConfig);
                    base._DbBind = bind;
                    bind.Context = this.Context;
                }
                return base._DbBind;
            }
        }

        public virtual int CommandTimeOut { get; set; }
        public virtual CommandType CommandType { get; set; }
        public virtual bool IsEnableLogEvent { get; set; }
        public virtual bool IsClearParameters { get; set; }
        public virtual Action<string, string> LogEventStarting { get; set; }
        public virtual Action<string, string> LogEventCompleted { get; set; }
        public virtual Func<string, Parameter[], KeyValuePair<string, Parameter[]>> ProcessingEventStartingSQL { get; set; }

        #endregion Properties

        #region Connection

        public virtual void Open()
        {
            CheckConnection();
        }

        public virtual void Close()
        {
            if (this.Transaction != null)
            {
                this.Transaction = null;
            }
            if (this.Connection != null && this.Connection.State == ConnectionState.Open)
            {
                this.Connection.Close();
            }
        }

        public virtual void Dispose()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Commit();
                this.Transaction = null;
            }
            if (this.Connection != null && this.Connection.State != ConnectionState.Open)
            {
                this.Connection.Close();
            }
            if (this.Connection != null)
            {
                this.Connection.Dispose();
            }
            this.Connection = null;
        }

        public virtual void CheckConnection()
        {
            if (this.Connection.State != ConnectionState.Open)
            {
                try
                {
                    this.Connection.Open();
                }
                catch (Exception ex)
                {
                    //  Check.Exception(true, ErrorMessage.ConnnectionOpen, ex.Message);
                }
            }
        }

        #endregion Connection

        #region Transaction

        public virtual void BeginTran()
        {
            CheckConnection();
            this.Transaction = this.Connection.BeginTransaction();
        }

        public virtual void BeginTran(IsolationLevel iso)
        {
            CheckConnection();
            this.Transaction = this.Connection.BeginTransaction(iso);
        }

        public virtual void RollbackTran()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Rollback();
                this.Transaction = null;
                if (this.Context.CurrentConfig.IsAutoCloseConnection) this.Close();
            }
        }

        public virtual void CommitTran()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Commit();
                this.Transaction = null;
                if (this.Context.CurrentConfig.IsAutoCloseConnection) this.Close();
            }
        }

        #endregion Transaction

        #region abstract

        public abstract IDataParameter[] ToIDbDataParameter(params Parameter[] pars);

        public abstract void SetCommandToAdapter(IDataAdapter adapter, IDbCommand command);

        public abstract IDataAdapter GetAdapter();

        public abstract IDbCommand GetCommand(string sql, Parameter[] pars);

        public abstract IDbConnection Connection { get; set; }

        public abstract void BeginTran(string transactionName);//Only SqlServer

        public abstract void BeginTran(IsolationLevel iso, string transactionName);//Only SqlServer

        #endregion abstract

        #region Use

        public DbResult<bool> UseTran(Action action)
        {
            var result = new DbResult<bool>();
            try
            {
                this.BeginTran();
                if (action != null)
                    action();
                this.CommitTran();
                result.Data = result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ErrorException = ex;
                result.ErrorMessage = ex.Message;
                result.IsSuccess = false;
                this.RollbackTran();
            }
            return result;
        }

        public DbResult<T> UseTran<T>(Func<T> action)
        {
            var result = new DbResult<T>();
            try
            {
                this.BeginTran();
                if (action != null)
                    result.Data = action();
                this.CommitTran();
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ErrorException = ex;
                result.ErrorMessage = ex.Message;
                result.IsSuccess = false;
                this.RollbackTran();
            }
            return result;
        }

        public void UseStoredProcedure(Action action)
        {
            var oldCommandType = this.CommandType;
            this.CommandType = CommandType.StoredProcedure;
            this.IsClearParameters = false;
            if (action != null)
            {
                action();
            }
            this.CommandType = oldCommandType;
            this.IsClearParameters = true;
        }

        public T UseStoredProcedure<T>(Func<T> action)
        {
            T result = default(T);
            var oldCommandType = this.CommandType;
            this.CommandType = CommandType.StoredProcedure;
            this.IsClearParameters = false;
            if (action != null)
            {
                result = action();
            }
            this.CommandType = oldCommandType;
            this.IsClearParameters = true;
            return result;
        }

        public IAdo UseStoredProcedure()
        {
            this.OldCommandType = this.CommandType;
            this.OldClearParameters = this.IsClearParameters;
            this.CommandType = CommandType.StoredProcedure;
            this.IsClearParameters = false;
            return this;
        }

        #endregion Use

        #region Core

        public virtual int ExecuteCommand(string sql, params Parameter[] parameters)
        {
            try
            {
                if (this.ProcessingEventStartingSQL != null)
                    ExecuteProcessingSQL(ref sql, parameters);
                ExecuteBefore(sql, parameters);
                IDbCommand sqlCommand = GetCommand(sql, parameters);
                int count = sqlCommand.ExecuteNonQuery();
                if (this.IsClearParameters)
                    sqlCommand.Parameters.Clear();
                ExecuteAfter(sql, parameters);
                return count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (this.IsClose()) this.Close();
            }
        }

        public virtual IDataReader GetDataReader(string sql, params Parameter[] parameters)
        {
            var isSp = this.CommandType == CommandType.StoredProcedure;
            if (this.ProcessingEventStartingSQL != null)
                ExecuteProcessingSQL(ref sql, parameters);
            ExecuteBefore(sql, parameters);
            IDbCommand sqlCommand = GetCommand(sql, parameters);
            IDataReader sqlDataReader = sqlCommand.ExecuteReader(this.IsClose() ? CommandBehavior.CloseConnection : CommandBehavior.Default);
            if (isSp)
                DataReaderParameters = sqlCommand.Parameters;
            if (this.IsClearParameters)
                sqlCommand.Parameters.Clear();
            ExecuteAfter(sql, parameters);
            return sqlDataReader;
        }

        public virtual DataSet GetDataSetAll(string sql, params Parameter[] parameters)
        {
            try
            {
                if (this.ProcessingEventStartingSQL != null)
                    ExecuteProcessingSQL(ref sql, parameters);
                ExecuteBefore(sql, parameters);
                IDataAdapter dataAdapter = this.GetAdapter();
                IDbCommand sqlCommand = GetCommand(sql, parameters);
                this.SetCommandToAdapter(dataAdapter, sqlCommand);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);
                if (this.IsClearParameters)
                    sqlCommand.Parameters.Clear();
                ExecuteAfter(sql, parameters);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (this.IsClose()) this.Close();
            }
        }

        public virtual object GetScalar(string sql, params Parameter[] parameters)
        {
            try
            {
                if (this.ProcessingEventStartingSQL != null)
                    ExecuteProcessingSQL(ref sql, parameters);
                ExecuteBefore(sql, parameters);
                IDbCommand sqlCommand = GetCommand(sql, parameters);
                object scalar = sqlCommand.ExecuteScalar();
                scalar = (scalar == null ? 0 : scalar);
                if (this.IsClearParameters)
                    sqlCommand.Parameters.Clear();
                ExecuteAfter(sql, parameters);
                return scalar;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (this.IsClose()) this.Close();
            }
        }

        #endregion Core

        #region Methods

        public virtual string GetString(string sql, object parameters)
        {
            return GetString(sql, this.GetParameters(parameters));
        }

        public virtual string GetString(string sql, params Parameter[] parameters)
        {
            return Convert.ToString(GetScalar(sql, parameters));
        }

        public virtual string GetString(string sql, List<Parameter> parameters)
        {
            if (parameters == null)
            {
                return GetString(sql);
            }
            else
            {
                return GetString(sql, parameters.ToArray());
            }
        }

        public virtual int GetInt(string sql, object parameters)
        {
            return GetInt(sql, this.GetParameters(parameters));
        }

        public virtual int GetInt(string sql, params Parameter[] parameters)
        {
            return GetScalar(sql, parameters).ObjToInt();
        }

        public virtual int GetInt(string sql, List<Parameter> parameters)
        {
            if (parameters == null)
            {
                return GetInt(sql);
            }
            else
            {
                return GetInt(sql, parameters.ToArray());
            }
        }

        public virtual Double GetDouble(string sql, object parameters)
        {
            return GetDouble(sql, this.GetParameters(parameters));
        }

        public virtual Double GetDouble(string sql, params Parameter[] parameters)
        {
            return GetScalar(sql, parameters).ObjToMoney();
        }

        public virtual Double GetDouble(string sql, List<Parameter> parameters)
        {
            if (parameters == null)
            {
                return GetDouble(sql);
            }
            else
            {
                return GetDouble(sql, parameters.ToArray());
            }
        }

        public virtual decimal GetDecimal(string sql, object parameters)
        {
            return GetDecimal(sql, this.GetParameters(parameters));
        }

        public virtual decimal GetDecimal(string sql, params Parameter[] parameters)
        {
            return GetScalar(sql, parameters).ObjToDecimal();
        }

        public virtual decimal GetDecimal(string sql, List<Parameter> parameters)
        {
            if (parameters == null)
            {
                return GetDecimal(sql);
            }
            else
            {
                return GetDecimal(sql, parameters.ToArray());
            }
        }

        public virtual DateTime GetDateTime(string sql, object parameters)
        {
            return GetDateTime(sql, this.GetParameters(parameters));
        }

        public virtual DateTime GetDateTime(string sql, params Parameter[] parameters)
        {
            return GetScalar(sql, parameters).ObjToDate();
        }

        public virtual DateTime GetDateTime(string sql, List<Parameter> parameters)
        {
            if (parameters == null)
            {
                return GetDateTime(sql);
            }
            else
            {
                return GetDateTime(sql, parameters.ToArray());
            }
        }

        public virtual List<T> SqlQuery<T>(string sql, object parameters = null)
        {
            var Parameters = this.GetParameters(parameters);
            return SqlQuery<T>(sql, Parameters);
        }

        public virtual List<T> SqlQuery<T>(string sql, params Parameter[] parameters)
        {
            var builder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConfig);
            builder.SqlQueryBuilder.sql.Append(sql);
            if (parameters != null && parameters.Any())
                builder.SqlQueryBuilder.Parameters.AddRange(parameters);
            List<T> result = null;
            using (var dataReader = this.GetDataReader(builder.SqlQueryBuilder.ToSqlString(), builder.SqlQueryBuilder.Parameters.ToArray()))
            {
                result = this.DbBind.DataReaderToList<T>(typeof(T), dataReader, builder.SqlQueryBuilder.Fields);
                builder.SqlQueryBuilder.Clear();
            }
            if (this.Context.Ado.DataReaderParameters != null)
            {
                foreach (IDataParameter item in this.Context.Ado.DataReaderParameters)
                {
                    var parameter = parameters.FirstOrDefault(it => item.ParameterName.Substring(1) == it.ParameterName.Substring(1));
                    if (parameter != null)
                    {
                        parameter.Value = item.Value;
                    }
                }
                this.Context.Ado.DataReaderParameters = null;
            }
            return result;
        }

        public virtual List<T> SqlQuery<T>(string sql, List<Parameter> parameters)
        {
            if (parameters != null)
            {
                return SqlQuery<T>(sql, parameters.ToArray());
            }
            else
            {
                return SqlQuery<T>(sql);
            }
        }

        public virtual T SqlQuerySingle<T>(string sql, object parameters = null)
        {
            var result = SqlQuery<T>(sql, parameters);
            return result == null ? default(T) : result.FirstOrDefault();
        }

        public virtual T SqlQuerySingle<T>(string sql, params Parameter[] parameters)
        {
            var result = SqlQuery<T>(sql, parameters);
            return result == null ? default(T) : result.FirstOrDefault();
        }

        public virtual T SqlQuerySingle<T>(string sql, List<Parameter> parameters)
        {
            var result = SqlQuery<T>(sql, parameters);
            return result == null ? default(T) : result.FirstOrDefault();
        }

        public virtual dynamic SqlQueryDynamic(string sql, object parameters = null)
        {
            var dt = this.GetDataTable(sql, parameters);
            return dt == null ? null : this.Context.Utilities.DataTableToDynamic(dt);
        }

        public virtual dynamic SqlQueryDynamic(string sql, params Parameter[] parameters)
        {
            var dt = this.GetDataTable(sql, parameters);
            return dt == null ? null : this.Context.Utilities.DataTableToDynamic(dt);
        }

        public dynamic SqlQueryDynamic(string sql, List<Parameter> parameters)
        {
            var dt = this.GetDataTable(sql, parameters);
            return dt == null ? null : this.Context.Utilities.DataTableToDynamic(dt);
        }

        public virtual DataTable GetDataTable(string sql, params Parameter[] parameters)
        {
            var ds = GetDataSetAll(sql, parameters);
            if (ds.Tables.Count != 0 && ds.Tables.Count > 0) return ds.Tables[0];
            return new DataTable();
        }

        public virtual DataTable GetDataTable(string sql, object parameters)
        {
            return GetDataTable(sql, this.GetParameters(parameters));
        }

        public virtual DataTable GetDataTable(string sql, List<Parameter> parameters)
        {
            if (parameters == null)
            {
                return GetDataTable(sql);
            }
            else
            {
                return GetDataTable(sql, parameters.ToArray());
            }
        }

        public virtual DataSet GetDataSetAll(string sql, object parameters)
        {
            return GetDataSetAll(sql, this.GetParameters(parameters));
        }

        public virtual DataSet GetDataSetAll(string sql, List<Parameter> parameters)
        {
            if (parameters == null)
            {
                return GetDataSetAll(sql);
            }
            else
            {
                return GetDataSetAll(sql, parameters.ToArray());
            }
        }

        public virtual IDataReader GetDataReader(string sql, object parameters)
        {
            return GetDataReader(sql, this.GetParameters(parameters));
        }

        public virtual IDataReader GetDataReader(string sql, List<Parameter> parameters)
        {
            if (parameters == null)
            {
                return GetDataReader(sql);
            }
            else
            {
                return GetDataReader(sql, parameters.ToArray());
            }
        }

        public virtual object GetScalar(string sql, object parameters)
        {
            return GetScalar(sql, this.GetParameters(parameters));
        }

        public virtual object GetScalar(string sql, List<Parameter> parameters)
        {
            if (parameters == null)
            {
                return GetScalar(sql);
            }
            else
            {
                return GetScalar(sql, parameters.ToArray());
            }
        }

        public virtual int ExecuteCommand(string sql, object parameters)
        {
            return ExecuteCommand(sql, GetParameters(parameters));
        }

        public virtual int ExecuteCommand(string sql, List<Parameter> parameters)
        {
            if (parameters == null)
            {
                return ExecuteCommand(sql);
            }
            else
            {
                return ExecuteCommand(sql, parameters.ToArray());
            }
        }

        #endregion Methods

        #region Helper

        private void ExecuteProcessingSQL(ref string sql, Parameter[] parameters)
        {
            var result = this.ProcessingEventStartingSQL(sql, parameters);
            sql = result.Key;
            parameters = result.Value;
        }

        public virtual void ExecuteBefore(string sql, Parameter[] parameters)
        {
            if (this.IsEnableLogEvent)
            {
                Action<string, string> action = LogEventStarting;
                if (action != null)
                {
                    if (parameters == null || parameters.Length == 0)
                    {
                        action(sql, null);
                    }
                    else
                    {
                        action(sql, this.Context.Utilities.SerializeObject(parameters.Select(it => new { key = it.ParameterName, value = it.Value.ObjToString() })));
                    }
                }
            }
        }

        public virtual void ExecuteAfter(string sql, Parameter[] parameters)
        {
            var hasParameter = parameters.IsValuable();
            if (hasParameter)
            {
                foreach (var outputParameter in parameters.Where(it => it.Direction == ParameterDirection.Output))
                {
                    var gobalOutputParamter = this.OutputParameters.Single(it => it.ParameterName == outputParameter.ParameterName);
                    outputParameter.Value = gobalOutputParamter.Value;
                    this.OutputParameters.Remove(gobalOutputParamter);
                }
            }
            if (this.IsEnableLogEvent)
            {
                Action<string, string> action = LogEventCompleted;
                if (action != null)
                {
                    if (parameters == null || parameters.Length == 0)
                    {
                        action(sql, null);
                    }
                    else
                    {
                        action(sql, this.Context.Utilities.SerializeObject(parameters.Select(it => new { key = it.ParameterName, value = it.Value.ObjToString() })));
                    }
                }
            }
            if (this.OldCommandType != 0)
            {
                this.CommandType = this.OldCommandType;
                this.IsClearParameters = this.OldClearParameters;
                this.OldCommandType = 0;
                this.OldClearParameters = false;
            }
        }

        public virtual Parameter[] GetParameters(object parameters, PropertyInfo[] propertyInfo = null)
        {
            if (parameters == null) return null;
            return base.GetParameters(parameters, propertyInfo, this.SqlParameterKeyWord);
        }

        private bool IsClose()
        {
            return this.Context.CurrentConfig.IsAutoCloseConnection && this.Transaction == null;
        }

        #endregion Helper
    }
}