using DotNetTN.Connector.SQL.Entities;
using DotNetTN.Connector.SQL.Interface;
using DotNetTN.Connector.SQL.SqlBuilderProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotNetTN.Connector.SQL.Common
{
    public class InstanceFactory
    {
        private static Assembly assembly = Assembly.Load(UtilConstants.AssemblyName);
        private static Dictionary<string, Type> typeCache = new Dictionary<string, Type>();

        public static QueryBuilder GetQueryBuilder(Config currentConnectionConfig)
        {
            if (currentConnectionConfig.DbType == DbType.SqlServer)
            {
                //    return new SqlServerQueryBuilder();
            }
            else
            {
                QueryBuilder result = CreateInstance<QueryBuilder>(GetClassName(currentConnectionConfig.DbType.ToString(), "QueryBuilder"));
                return result;
            }
            return null;
        }

        public static Interface.IQueryable<T> GetQueryable<T>(Config currentConnectionConfig)
        {
            if (currentConnectionConfig.DbType == DbType.SqlServer)
            {
                //  return new SqlServerQueryable<T>();
            }
            else
            {
                string className = "Queryable";
                className = GetClassName(currentConnectionConfig.DbType.ToString(), className);
                Interface.IQueryable<T> result = CreateInstance<T, Interface.IQueryable<T>>(className);
                return result;
            }

            return null;
        }

        public static Interface.IQueryable<T, T2> GetQueryable<T, T2>(Config currentConnectionConfig)
        {
            if (currentConnectionConfig.DbType == DbType.SqlServer)
            {
                //                return new sql<T, T2>();
            }
            else
            {
                string className = "Queryable";
                className = GetClassName(currentConnectionConfig.DbType.ToString(), className);
                Interface.IQueryable<T, T2> result = CreateInstance<T, T2, Interface.IQueryable<T, T2>>(className);
                return result;
            }
            return null;
        }

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

        private static Restult CreateInstance<T, Restult>(string className)
        {
            return CreateInstance<Restult>(className, typeof(T));
        }

        private static Restult CreateInstance<Restult>(string className, params Type[] types)
        {
            var cacheKey = className + string.Join(",", types.Select(it => it.FullName));
            Type type;
            if (typeCache.ContainsKey(cacheKey))
            {
                type = typeCache[cacheKey];
            }
            else
            {
                lock (typeCache)
                {
                    type = Type.GetType(className + "`" + types.Length, true).MakeGenericType(types);
                    //  Check.ArgumentNullException(type, string.Format(ErrorMessage.ObjNotExist, className));
                    if (!typeCache.ContainsKey(cacheKey))
                    {
                        typeCache.Add(cacheKey, type);
                    }
                }
            }
            var result = (Restult)Activator.CreateInstance(type, true);
            return result;
        }

        private static Restult CreateInstance<T, T2, Restult>(string className)
        {
            return CreateInstance<Restult>(className, typeof(T), typeof(T2));
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

// DotNetTN.Connector.MySql.DbBind.MySqlDbBind