using DotNetTN.Connector.SQL.Common;
using DotNetTN.Connector.SQL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DotNetTN.Connector.SQL.AdoProvider
{
    public partial class AdoAccessory
    {

        protected IDbBind _DbBind;

        protected IDbConnection _DbConnection;

        protected virtual Parameter[] GetParameters(object parameters, PropertyInfo[] propertyInfo, string sqlParameterKeyWord)
        {
            List<Parameter> result = new List<Parameter>();
            if (parameters != null)
            {
                var entityType = parameters.GetType();
                var isDictionary = entityType.IsIn(UtilConstants.DicArraySO, UtilConstants.DicArraySS);
                if (isDictionary)
                    DictionaryToParameters(parameters, sqlParameterKeyWord, result, entityType);
                else
                    ProperyToParameter(parameters, propertyInfo, sqlParameterKeyWord, result, entityType);
            }
            return result.ToArray();
        }
        protected void ProperyToParameter(object parameters, PropertyInfo[] propertyInfo, string sqlParameterKeyWord, List<Parameter> listParams, Type entityType)
        {
            PropertyInfo[] properties = null;
            if (propertyInfo != null)
                properties = propertyInfo;
            else
                properties = entityType.GetProperties();

            foreach (PropertyInfo properyty in properties)
            {
                var value = properyty.GetValue(parameters, null);
                if (properyty.PropertyType.IsEnum())
                    value = Convert.ToInt64(value);
                if (value == null || value.Equals(DateTime.MinValue)) value = DBNull.Value;
                if (properyty.Name.ToLower().Contains("hierarchyid"))
                {
                    var parameter = new Parameter(sqlParameterKeyWord + properyty.Name, SqlDbType.Udt);
                    parameter.UdtTypeName = "HIERARCHYID";
                    parameter.Value = value;
                    listParams.Add(parameter);
                }
                else
                {
                    var parameter = new Parameter(sqlParameterKeyWord + properyty.Name, value);
                    listParams.Add(parameter);
                }
            }
        }
        protected void DictionaryToParameters(object parameters, string sqlParameterKeyWord, List<Parameter> listParams, Type entityType)
        {
            if (entityType == UtilConstants.DicArraySO)
            {
                var dictionaryParameters = (Dictionary<string, object>)parameters;
                var Parameters = dictionaryParameters.Select(it => new Parameter(sqlParameterKeyWord + it.Key, it.Value));
                listParams.AddRange(Parameters);
            }
            else
            {
                var dictionaryParameters = (Dictionary<string, string>)parameters;
                var Parameters = dictionaryParameters.Select(it => new Parameter(sqlParameterKeyWord + it.Key, it.Value));
                listParams.AddRange(Parameters); ;
            }
        }
    }
}