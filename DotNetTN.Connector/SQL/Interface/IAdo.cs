using DotNetTN.Connector.SQL.Common;
using DotNetTN.Connector.SQL.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTN.Connector.SQL.Interface
{
    public partial interface IAdo
    {
        string SqlParameterKeyWord { get; }
        IDbConnection Connection { get; set; }
        IDbTransaction Transaction { get; set; }
        IDataParameter[] ToIDbDataParameter(params Parameter[] pars);
        Parameter[] GetParameters(object obj, PropertyInfo[] propertyInfo = null);
        SqlClient Context { get; set; }
        void ExecuteBefore(string sql, Parameter[] pars);
        void ExecuteAfter(string sql, Parameter[] pars);

        IDataParameterCollection DataReaderParameters { get; set; }
        CommandType CommandType { get; set; }
        bool IsEnableLogEvent { get; set; }
        Action<string, string> LogEventStarting { get; set; }
        Action<string, string> LogEventCompleted { get; set; }
        Func<string, Parameter[], KeyValuePair<string, Parameter[]>> ProcessingEventStartingSQL { get; set; }
        bool IsClearParameters { get; set; }
        int CommandTimeOut { get; set; }
       // IDbBind DbBind { get; }
        void SetCommandToAdapter(IDataAdapter adapter, IDbCommand command);
        IDataAdapter GetAdapter();
        IDbCommand GetCommand(string sql, Parameter[] parameters);
        DataTable GetDataTable(string sql, object parameters);
        DataTable GetDataTable(string sql, params Parameter[] parameters);
        DataTable GetDataTable(string sql, List<Parameter> parameters);
        DataSet GetDataSetAll(string sql, object parameters);
        DataSet GetDataSetAll(string sql, params Parameter[] parameters);
        DataSet GetDataSetAll(string sql, List<Parameter> parameters);
        IDataReader GetDataReader(string sql, object parameters);
        IDataReader GetDataReader(string sql, params Parameter[] parameters);
        IDataReader GetDataReader(string sql, List<Parameter> parameters);
        object GetScalar(string sql, object parameters);
        object GetScalar(string sql, params Parameter[] parameters);
        object GetScalar(string sql, List<Parameter> parameters);
        int ExecuteCommand(string sql, object parameters);
        int ExecuteCommand(string sql, params Parameter[] parameters);
        int ExecuteCommand(string sql, List<Parameter> parameters);
        string GetString(string sql, object parameters);
        string GetString(string sql, params Parameter[] parameters);
        string GetString(string sql, List<Parameter> parameters);
        int GetInt(string sql, object pars);
        int GetInt(string sql, params Parameter[] parameters);
        int GetInt(string sql, List<Parameter> parameters);
        Double GetDouble(string sql, object parameters);
        Double GetDouble(string sql, params Parameter[] parameters);
        Double GetDouble(string sql, List<Parameter> parameters);
        decimal GetDecimal(string sql, object parameters);
        decimal GetDecimal(string sql, params Parameter[] parameters);
        decimal GetDecimal(string sql, List<Parameter> parameters);
        DateTime GetDateTime(string sql, object parameters);
        DateTime GetDateTime(string sql, params Parameter[] parameters);
        DateTime GetDateTime(string sql, List<Parameter> parameters);
        List<T> SqlQuery<T>(string sql, object whereObj = null);
        List<T> SqlQuery<T>(string sql, params Parameter[] parameters);
        List<T> SqlQuery<T>(string sql, List<Parameter> parameters);
        T SqlQuerySingle<T>(string sql, object whereObj = null);
        T SqlQuerySingle<T>(string sql, params Parameter[] parameters);
        T SqlQuerySingle<T>(string sql, List<Parameter> parameters);
        dynamic SqlQueryDynamic(string sql, object whereObj = null);
        dynamic SqlQueryDynamic(string sql, params Parameter[] parameters);
        dynamic SqlQueryDynamic(string sql, List<Parameter> parameters);
        void Dispose();
        void Close();
        void Open();
        void CheckConnection();

        void BeginTran();
        void BeginTran(IsolationLevel iso);
        void BeginTran(string transactionName);
        void BeginTran(IsolationLevel iso, string transactionName);
        void RollbackTran();
        void CommitTran();

        DbResult<bool> UseTran(Action action);
        DbResult<T> UseTran<T>(Func<T> action);
        void UseStoredProcedure(Action action);
        T UseStoredProcedure<T>(Func<T> action);
        IAdo UseStoredProcedure();
    }
}
