using DotNetTN.Connector.SQL.Entities;
using DotNetTN.Connector.SQL.Interface;
using DotNetTN.Connector.SQL.SqlBuilderProvider;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DotNetTN.Connector.SQL.Common
{
    public class InstanceFactory
    {
        private static Assembly assembly = Assembly.Load(UtilConstants.AssemblyName);
        private static Dictionary<string, Type> typeCache = new Dictionary<string, Type>();

        public static ILambdaExpressions GetLambdaExpressions(Config currentConnectionConfig)
        {
            if (currentConnectionConfig.DbType == DbType.SqlServer)
            {
                //  return new SqlServerExpressionContext();
            }
            else
            {
                ILambdaExpressions result = CreateInstance<ILambdaExpressions>(GetClassName(currentConnectionConfig.DbType.ToString(), "ExpressionContext"));
                return result;
            }
            return null;
        }

        public static UpdateBuilder GetUpdateBuilder(Config currentConnectionConfig)
        {
            UpdateBuilder result = CreateInstance<UpdateBuilder>(GetClassName(currentConnectionConfig.DbType.ToString(), "UpdateBuilder"));
            return result;
        }

        public static DeleteBuilder GetDeleteBuilder(Config currentConnectionConfig)
        {
            DeleteBuilder result = CreateInstance<DeleteBuilder>(GetClassName(currentConnectionConfig.DbType.ToString(), "DeleteBuilder"));
            return result;
        }

        public static InsertBuilder GetInsertBuilder(Config currentConnectionConfig)
        {
            InsertBuilder result = CreateInstance<InsertBuilder>(GetClassName(currentConnectionConfig.DbType.ToString(), "InsertBuilder"));
            return result;
        }

        public static ISqlBuilder GetSqlbuilder(Config currentConnectionConfig)
        {
            ISqlBuilder result = CreateInstance<ISqlBuilder>(GetClassName(currentConnectionConfig.DbType.ToString(), "Builder"));
            return result;
        }

        public static IDbBind GetDbBind(Config currentConnectionConfig)
        {
            IDbBind result = CreateInstance<IDbBind>(GetClassName(currentConnectionConfig.DbType.ToString(), "DbBind"));
            return result;
        }

        public static IAdo GetAdo(Config currentConnectionConfig)
        {
            IAdo result = CreateInstance<IAdo>(GetClassName(currentConnectionConfig.DbType.ToString(), "Provider"));
            return result;
        }

        private static string GetClassName(string type, string name)
        {
            return UtilConstants.AssemblyName + "." + type + "." + name + "." + type + name;
        }

        public static T CreateInstance<T>(string className)
        {
            Type type;
            if (typeCache.ContainsKey(className))
            {
                type = typeCache[className];
            }
            else
            {
                lock (typeCache)
                {
                    type = assembly.GetType(className);
                    //  Check.ArgumentNullException(type, string.Format(ErrorMessage.ObjNotExist, className));
                    if (!typeCache.ContainsKey(className))
                    {
                        typeCache.Add(className, type);
                    }
                }
            }
            var result = (T)Activator.CreateInstance(type, true);
            return result;
        }
    }
}