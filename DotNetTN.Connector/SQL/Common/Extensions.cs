using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTN.Connector.SQL.Common
{
    public static class Extensions
    {

        public static Type GetTypeInfo(this Type typeInfo)
        {
            return typeInfo;
        }
        internal static T IsNullReturnNew<T>(T returnObj) where T : new()
        {
            if (returnObj.IsNullOrEmpty())
            {
                returnObj = new T();
            }
            return returnObj;
        }
        public static bool IsIn<T>(this T thisValue, params T[] values)
        {
            return values.Contains(thisValue);
        }

        public static Type[] GetGenericArguments(this Type type)
        {
            var reval = type.GetTypeInfo().GetGenericArguments();
            return reval;
        }
        public static bool IsGenericType(this Type type)
        {
            var reval = type.GetTypeInfo().IsGenericType;
            return reval;
        }
        public static PropertyInfo[] GetProperties(this Type type)
        {
            var reval = type.GetTypeInfo().GetProperties();
            return reval;
        }
        public static PropertyInfo GetProperty(this Type type, string name)
        {
            var reval = type.GetTypeInfo().GetProperty(name);
            return reval;
        }

      

        public static FieldInfo GetField(this Type type, string name)
        {
            var reval = type.GetTypeInfo().GetField(name);
            return reval;
        }

        public static bool IsEnum(this Type type)
        {
            var reval = type.GetTypeInfo().IsEnum;
            return reval;
        }

        public static MethodInfo GetMethod(this Type type, string name)
        {
            var reval = type.GetTypeInfo().GetMethod(name);
            return reval;
        }
        public static MethodInfo GetMethod(this Type type, string name, Type[] types)
        {
            var reval = type.GetTypeInfo().GetMethod(name, types);
            return reval;
        }
        public static ConstructorInfo GetConstructor(this Type type, Type[] types)
        {
            var reval = type.GetTypeInfo().GetConstructor(types);
            return reval;
        }

        public static bool IsValueType(this Type type)
        {
            return type.GetTypeInfo().IsValueType;
        }

        public static bool IsEntity(this Type type)
        {
            return type.GetTypeInfo().IsClass;
        }

        public static Type ReflectedType(this MethodInfo method)
        {
            return method.ReflectedType;
        }

        public static string ToJoinSqlInVals<T>(this T[] array)
        {
            if (array == null || array.Length == 0)
            {
                return ToSqlValue(string.Empty);
            }
            else
            {
                return string.Join(",", array.Where(c => c != null).Select(it => (it + "").ToSqlValue()));
            }
        }

        public static string ToSqlValue(this string value)
        {
            return string.Format("'{0}'", value.ToSqlFilter());
        }

        /// <summary>
        ///Sql Filter
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSqlFilter(this string value)
        {
            if (!value.IsNullOrEmpty())
            {
                value = value.Replace("'", "''");
            }
            return value;
        }

        public static bool IsNullOrEmpty(this object thisValue)
        {
            if (thisValue == null || thisValue == DBNull.Value) return true;
            return thisValue.ToString() == "";
        }

        public static bool IsValuable(this object thisValue)
        {
            if (thisValue == null || thisValue == DBNull.Value) return false;
            return thisValue.ToString() != "";
        }

        public static bool IsContainsIn(this string thisValue, params string[] inValues)
        {
            return inValues.Any(it => thisValue.Contains(it));
        }


        internal static string GetPackTable(string sql, string shortName)
        {
            return string.Format(" ({0}) {1} ", sql, shortName);
        }
        internal static void RepairReplicationParameters(ref string appendSql, Parameter[] parameters, int addIndex)
        {
            if (appendSql.IsValuable() && parameters.IsValuable())
            {
                foreach (var parameter in parameters.OrderByDescending(it => it.ParameterName.Length))
                {
                    //Compatible with.NET CORE parameters case
                    var name = parameter.ParameterName;
                    string newName = name + addIndex;
                    appendSql = appendSql.Replace(name, newName);
                    parameter.ParameterName = newName;
                }
            }
        }

        internal static Type GetUnderType(Type oldType)
        {
            Type type = Nullable.GetUnderlyingType(oldType);
            return type == null ? oldType : type;
        }
        internal static Type GetUnderType(PropertyInfo propertyInfo)
        {
            Type unType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
            unType = unType ?? propertyInfo.PropertyType;
            return unType;
        }
    }
}
