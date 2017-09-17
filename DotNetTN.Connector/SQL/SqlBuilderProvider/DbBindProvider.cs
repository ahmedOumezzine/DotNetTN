using DotNetTN.Connector.SQL.Common;
using DotNetTN.Connector.SQL.Entities;
using DotNetTN.Connector.SQL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTN.Connector.SQL.SqlBuilderProvider
{
    public partial class DbBindAccessory
    {
        protected List<T> GetEntityList<T>(SqlClient context, IDataReader dataReader, string fields)
        {
            Type type = typeof(T);
            var cacheManager = context.Utilities.GetCacheInstance<IDataReaderEntityBuilder<T>>();
            string key = "DataReaderToList." + fields + context.CurrentConfig.DbType + type.FullName;
            IDataReaderEntityBuilder<T> entytyList = null;
            if (cacheManager.ContainsKey(key))
            {
                entytyList = cacheManager[key];
            }
            else
            {
                entytyList = new IDataReaderEntityBuilder<T>(context, dataReader).CreateBuilder(type);
                cacheManager.Add(key, entytyList);
            }
            List<T> result = new List<T>();
            try
            {
                if (dataReader == null) return result;
                while (dataReader.Read())
                {
                    result.Add(entytyList.Build(dataReader));
                }
            }
            catch (Exception ex)
            {
                //    Check.Exception(true, ErrorMessage.EntityMappingError, ex.Message);
            }
            return result;
        }

        protected List<T> GetKeyValueList<T>(Type type, IDataReader dataReader)
        {
            List<T> reval = new List<T>();
            using (IDataReader re = dataReader)
            {
                while (re.Read())
                {
                    if (UtilConstants.DicOO == type)
                    {
                        var kv = new KeyValuePair<object, object>(dataReader.GetValue(0), re.GetValue(1));
                        reval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<object, object>)));
                    }
                    else if (UtilConstants.DicIS == type)
                    {
                        var kv = new KeyValuePair<int, string>(dataReader.GetValue(0).ObjToInt(), re.GetValue(1).ObjToString());
                        reval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<int, string>)));
                    }
                    else if (UtilConstants.Dicii == type)
                    {
                        var kv = new KeyValuePair<int, int>(dataReader.GetValue(0).ObjToInt(), re.GetValue(1).ObjToInt());
                        reval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<int, int>)));
                    }
                    else if (UtilConstants.DicSi == type)
                    {
                        var kv = new KeyValuePair<string, int>(dataReader.GetValue(0).ObjToString(), re.GetValue(1).ObjToInt());
                        reval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<string, int>)));
                    }
                    else if (UtilConstants.DicSo == type)
                    {
                        var kv = new KeyValuePair<string, object>(dataReader.GetValue(0).ObjToString(), re.GetValue(1));
                        reval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<string, object>)));
                    }
                    else if (UtilConstants.DicSS == type)
                    {
                        var kv = new KeyValuePair<string, string>(dataReader.GetValue(0).ObjToString(), dataReader.GetValue(1).ObjToString());
                        reval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<string, string>)));
                    }
                    else
                    {
                        //   Check.Exception(true, ErrorMessage.NotSupportedDictionary);
                    }
                }
            }
            return reval;
        }

        protected List<T> GetArrayList<T>(Type type, IDataReader dataReader)
        {
            List<T> reval = new List<T>();
            using (IDataReader re = dataReader)
            {
                int count = dataReader.FieldCount;
                var childType = type.GetElementType();
                while (re.Read())
                {
                    object[] array = new object[count];
                    for (int i = 0; i < count; i++)
                    {
                        array[i] = Convert.ChangeType(re.GetValue(i), childType);
                    }
                    if (childType == UtilConstants.StringType)
                        reval.Add((T)Convert.ChangeType(array.Select(it => it.ObjToString()).ToArray(), type));
                    else if (childType == UtilConstants.ObjType)
                        reval.Add((T)Convert.ChangeType(array.Select(it => it == DBNull.Value ? null : (object)it).ToArray(), type));
                    else if (childType == UtilConstants.BoolType)
                        reval.Add((T)Convert.ChangeType(array.Select(it => it.ObjToBool()).ToArray(), type));
                    else if (childType == UtilConstants.ByteType)
                        reval.Add((T)Convert.ChangeType(array.Select(it => it == DBNull.Value ? 0 : (byte)it).ToArray(), type));
                    else if (childType == UtilConstants.DecType)
                        reval.Add((T)Convert.ChangeType(array.Select(it => it.ObjToDecimal()).ToArray(), type));
                    else if (childType == UtilConstants.GuidType)
                        reval.Add((T)Convert.ChangeType(array.Select(it => it == DBNull.Value ? Guid.Empty : (Guid)it).ToArray(), type));
                    else if (childType == UtilConstants.DateType)
                        reval.Add((T)Convert.ChangeType(array.Select(it => it == DBNull.Value ? DateTime.MinValue : (DateTime)it).ToArray(), type));
                    else if (childType == UtilConstants.IntType)
                        reval.Add((T)Convert.ChangeType(array.Select(it => it.ObjToInt()).ToArray(), type));
                    else
                    {
                        //     Check.Exception(true, ErrorMessage.NotSupportedArray);
                    }
                }
            }
            return reval;
        }

        protected List<T> GetValueTypeList<T>(Type type, IDataReader dataReader)
        {
            List<T> reval = new List<T>();
            using (IDataReader re = dataReader)
            {
                while (re.Read())
                {
                    var value = re.GetValue(0);
                    if (value == DBNull.Value)
                    {
                        reval.Add(default(T));
                    }
                }
            }
            return reval;
        }
    }

    public abstract partial class DbBindProvider : DbBindAccessory, IDbBind
    {
        #region Properties

        public virtual SqlClient Context { get; set; }
        public abstract List<KeyValuePair<string, CSharpDataType>> MappingTypes { get; }

        #endregion Properties

        #region Public methods

        public virtual string GetDbTypeName(string csharpTypeName)
        {
            if (csharpTypeName == UtilConstants.ByteArrayType.Name)
            {
                return "varbinary";
            }
            if (csharpTypeName == "Int32")
                csharpTypeName = "int";
            if (csharpTypeName == "Int16")
                csharpTypeName = "short";
            if (csharpTypeName == "Int64")
                csharpTypeName = "long";
            if (csharpTypeName == "Boolean")
                csharpTypeName = "bool";
            var mappings = this.MappingTypes.Where(it => it.Value.ToString().Equals(csharpTypeName, StringComparison.CurrentCultureIgnoreCase));
            return mappings.IsValuable() ? mappings.First().Key : "varchar";
        }

        public string GetCsharpTypeName(string dbTypeName)
        {
            var mappings = this.MappingTypes.Where(it => it.Key == dbTypeName);
            return mappings.IsValuable() ? mappings.First().Key : "string";
        }

        public virtual string GetConvertString(string dbTypeName)
        {
            string reval = string.Empty;
            switch (dbTypeName.ToLower())
            {
                #region Int

                case "int":
                    reval = "Convert.ToInt32";
                    break;

                #endregion Int

                #region String

                case "nchar":
                case "char":
                case "ntext":
                case "nvarchar":
                case "varchar":
                case "text":
                    reval = "Convert.ToString";
                    break;

                #endregion String

                #region Long

                case "bigint":
                    reval = "Convert.ToInt64";
                    break;

                #endregion Long

                #region Bool

                case "bit":
                    reval = "Convert.ToBoolean";
                    break;

                #endregion Bool

                #region Datetime

                case "timestamp":
                case "smalldatetime":
                case "datetime":
                case "date":
                case "datetime2":
                    reval = "Convert.ToDateTime";
                    break;

                #endregion Datetime

                #region Decimal

                case "smallmoney":
                case "single":
                case "numeric":
                case "money":
                case "decimal":
                    reval = "Convert.ToDecimal";
                    break;

                #endregion Decimal

                #region Double

                case "float":
                    reval = "Convert.ToDouble";
                    break;

                #endregion Double

                #region Byte[]

                case "varbinary":
                case "binary":
                case "image":
                    reval = "byte[]";
                    break;

                #endregion Byte[]

                #region Float

                case "real":
                    reval = "Convert.ToSingle";
                    break;

                #endregion Float

                #region Short

                case "smallint":
                    reval = "Convert.ToInt16";
                    break;

                #endregion Short

                #region Byte

                case "tinyint":
                    reval = "Convert.ToByte";
                    break;

                #endregion Byte

                #region Guid

                case "uniqueidentifier":
                    reval = "Guid.Parse";
                    break;

                #endregion Guid

                #region Null

                default:
                    reval = null;
                    break;

                    #endregion Null
            }
            return reval;
        }

        public virtual string GetPropertyTypeName(string dbTypeName)
        {
            dbTypeName = dbTypeName.ToLower();
            var propertyTypes = MappingTypes.Where(it => it.Key == dbTypeName);
            if (dbTypeName == "int32")
            {
                return "int";
            }
            else if (dbTypeName == "int64")
            {
                return "long";
            }
            else if (propertyTypes == null)
            {
                return "other";
            }
            else if (dbTypeName == "xml")
            {
                return "string";
            }
            else if (propertyTypes == null || propertyTypes.Count() == 0)
            {
                //    Check.ThrowNotSupportedException(string.Format(" \"{0}\" Type NotSupported, DbBindProvider.GetPropertyTypeName error.", dbTypeName));
                return null;
            }
            else if (propertyTypes.First().Value == CSharpDataType.byteArray)
            {
                return "byte[]";
            }
            else
            {
                return propertyTypes.First().Value.ToString();
            }
        }

        public virtual List<T> DataReaderToList<T>(Type type, IDataReader dataReader, string fields)
        {
            using (dataReader)
            {
                if (type.Name.Contains("KeyValuePair"))
                {
                    return GetKeyValueList<T>(type, dataReader);
                }
                else if (type.IsValueType() || type == UtilConstants.StringType)
                {
                    return GetValueTypeList<T>(type, dataReader);
                }
                else if (type.IsArray)
                {
                    return GetArrayList<T>(type, dataReader);
                }
                else
                {
                    return GetEntityList<T>(Context, dataReader, fields);
                }
            }
        }

        #endregion Public methods

        protected List<T> GetKeyValueList<T>(Type type, IDataReader dataReader)
        {
            List<T> reval = new List<T>();
            using (IDataReader re = dataReader)
            {
                while (re.Read())
                {
                    if (UtilConstants.DicOO == type)
                    {
                        var kv = new KeyValuePair<object, object>(dataReader.GetValue(0), re.GetValue(1));
                        reval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<object, object>)));
                    }
                    else if (UtilConstants.DicIS == type)
                    {
                        var kv = new KeyValuePair<int, string>(dataReader.GetValue(0).ObjToInt(), re.GetValue(1).ObjToString());
                        reval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<int, string>)));
                    }
                    else if (UtilConstants.Dicii == type)
                    {
                        var kv = new KeyValuePair<int, int>(dataReader.GetValue(0).ObjToInt(), re.GetValue(1).ObjToInt());
                        reval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<int, int>)));
                    }
                    else if (UtilConstants.DicSi == type)
                    {
                        var kv = new KeyValuePair<string, int>(dataReader.GetValue(0).ObjToString(), re.GetValue(1).ObjToInt());
                        reval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<string, int>)));
                    }
                    else if (UtilConstants.DicSo == type)
                    {
                        var kv = new KeyValuePair<string, object>(dataReader.GetValue(0).ObjToString(), re.GetValue(1));
                        reval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<string, object>)));
                    }
                    else if (UtilConstants.DicSS == type)
                    {
                        var kv = new KeyValuePair<string, string>(dataReader.GetValue(0).ObjToString(), dataReader.GetValue(1).ObjToString());
                        reval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<string, string>)));
                    }
                    else
                    {
                        //         Check.Exception(true, ErrorMessage.NotSupportedDictionary);
                    }
                }
            }
            return reval;
        }

        #region Throw rule

        public virtual List<string> IntThrow
        {
            get
            {
                return new List<string>() { "datetime", "byte" };
            }
        }

        public virtual List<string> ShortThrow
        {
            get
            {
                return new List<string>() { "datetime", "guid" };
            }
        }

        public virtual List<string> DecimalThrow
        {
            get
            {
                return new List<string>() { "datetime", "byte", "guid" };
            }
        }

        public virtual List<string> DoubleThrow
        {
            get
            {
                return new List<string>() { "datetime", "byte", "guid" };
            }
        }

        public virtual List<string> DateThrow
        {
            get
            {
                return new List<string>() { "int32", "decimal", "double", "byte", "guid" };
            }
        }

        public virtual List<string> GuidThrow
        {
            get
            {
                return new List<string>() { "int32", "datetime", "decimal", "double", "byte" };
            }
        }

        public virtual List<string> StringThrow
        {
            get
            {
                return new List<string>() { "int32", "datetime", "decimal", "double", "byte", "guid" };
            }
        }

        #endregion Throw rule
    }
}