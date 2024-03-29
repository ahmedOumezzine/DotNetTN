﻿using DotNetTN.Connector.SQL.Common;
using DotNetTN.Connector.SQL.Entities;
using DotNetTN.Connector.SQL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace DotNetTN.Connector.SQL.SqlBuilderProvider
{
    public partial class IDataReaderEntityBuilder<T>
    {
        #region Properies

        private List<string> ReaderKeys { get; set; }

        #endregion Properies

        #region Fields

        private SqlClient Context = null;
        private IDataReaderEntityBuilder<T> DynamicBuilder;
        private IDataRecord DataRecord;
        private static readonly MethodInfo isDBNullMethod = typeof(IDataRecord).GetMethod("IsDBNull", new Type[] { typeof(int) });
        private static readonly MethodInfo getValueMethod = typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(int) });
        private static readonly MethodInfo getBoolean = typeof(IDataRecord).GetMethod("GetBoolean", new Type[] { typeof(int) });
        private static readonly MethodInfo getByte = typeof(IDataRecord).GetMethod("GetByte", new Type[] { typeof(int) });
        private static readonly MethodInfo getDateTime = typeof(IDataRecord).GetMethod("GetDateTime", new Type[] { typeof(int) });
        private static readonly MethodInfo getDecimal = typeof(IDataRecord).GetMethod("GetDecimal", new Type[] { typeof(int) });
        private static readonly MethodInfo getDouble = typeof(IDataRecord).GetMethod("GetDouble", new Type[] { typeof(int) });
        private static readonly MethodInfo getFloat = typeof(IDataRecord).GetMethod("GetFloat", new Type[] { typeof(int) });
        private static readonly MethodInfo getGuid = typeof(IDataRecord).GetMethod("GetGuid", new Type[] { typeof(int) });
        private static readonly MethodInfo getInt16 = typeof(IDataRecord).GetMethod("GetInt16", new Type[] { typeof(int) });
        private static readonly MethodInfo getInt32 = typeof(IDataRecord).GetMethod("GetInt32", new Type[] { typeof(int) });
        private static readonly MethodInfo getInt64 = typeof(IDataRecord).GetMethod("GetInt64", new Type[] { typeof(int) });
        private static readonly MethodInfo getString = typeof(IDataRecord).GetMethod("GetString", new Type[] { typeof(int) });
        private static readonly MethodInfo getStringGuid = typeof(IDataRecordExtensions).GetMethod("GetStringGuid");
        private static readonly MethodInfo getConvertStringGuid = typeof(IDataRecordExtensions).GetMethod("GetConvertStringGuid");
        private static readonly MethodInfo getEnum = typeof(IDataRecordExtensions).GetMethod("GetEnum");
        private static readonly MethodInfo getConvertString = typeof(IDataRecordExtensions).GetMethod("GetConvertString");
        private static readonly MethodInfo getConvertFloat = typeof(IDataRecordExtensions).GetMethod("GetConvertFloat");
        private static readonly MethodInfo getConvertBoolean = typeof(IDataRecordExtensions).GetMethod("GetConvertBoolean");
        private static readonly MethodInfo getConvertByte = typeof(IDataRecordExtensions).GetMethod("GetConvertByte");
        private static readonly MethodInfo getConvertChar = typeof(IDataRecordExtensions).GetMethod("GetConvertChar");
        private static readonly MethodInfo getConvertDateTime = typeof(IDataRecordExtensions).GetMethod("GetConvertDateTime");
        private static readonly MethodInfo getConvertDecimal = typeof(IDataRecordExtensions).GetMethod("GetConvertDecimal");
        private static readonly MethodInfo getConvertDouble = typeof(IDataRecordExtensions).GetMethod("GetConvertDouble");
        private static readonly MethodInfo getConvertGuid = typeof(IDataRecordExtensions).GetMethod("GetConvertGuid");
        private static readonly MethodInfo getConvertInt16 = typeof(IDataRecordExtensions).GetMethod("GetConvertInt16");
        private static readonly MethodInfo getConvertInt32 = typeof(IDataRecordExtensions).GetMethod("GetConvertInt32");
        private static readonly MethodInfo getConvertInt64 = typeof(IDataRecordExtensions).GetMethod("GetConvetInt64");
        private static readonly MethodInfo getConvertEnum_Null = typeof(IDataRecordExtensions).GetMethod("GetConvertEnum_Null");
        private static readonly MethodInfo getOtherNull = typeof(IDataRecordExtensions).GetMethod("GetOtherNull");
        private static readonly MethodInfo getOther = typeof(IDataRecordExtensions).GetMethod("GetOther");
        private static readonly MethodInfo getSqliteTypeNull = typeof(IDataRecordExtensions).GetMethod("GetSqliteTypeNull");
        private static readonly MethodInfo getSqliteType = typeof(IDataRecordExtensions).GetMethod("GetSqliteType");
        private static readonly MethodInfo getEntity = typeof(IDataRecordExtensions).GetMethod("GetEntity", new Type[] { typeof(SqlClient) });

        private delegate T Load(IDataRecord dataRecord);

        private Load handler;

        #endregion Fields

        #region Constructor

        private IDataReaderEntityBuilder()
        {
        }

        public IDataReaderEntityBuilder(SqlClient context, IDataRecord dataRecord)
        {
            this.Context = context;
            this.DataRecord = dataRecord;
            this.DynamicBuilder = new IDataReaderEntityBuilder<T>();
            this.ReaderKeys = new List<string>();
        }

        #endregion Constructor

        #region Public methods

        public T Build(IDataRecord dataRecord)
        {
            return handler(dataRecord);
        }

        public IDataReaderEntityBuilder<T> CreateBuilder(Type type)
        {
            for (int i = 0; i < this.DataRecord.FieldCount; i++)
            {
                this.ReaderKeys.Add(this.DataRecord.GetName(i));
            }
            DynamicMethod method = new DynamicMethod("SqlSugarEntity", type,
            new Type[] { typeof(IDataRecord) }, type, true);
            ILGenerator generator = method.GetILGenerator();
            LocalBuilder result = generator.DeclareLocal(type);
            generator.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);
            var mappingColumns = Context.MappingColumns.Where(it => it.EntityName.Equals(type.Name, StringComparison.CurrentCultureIgnoreCase)).ToList();
            var properties = type.GetProperties();
            foreach (var propertyInfo in properties)
            {
                string fileName = propertyInfo.Name;
                if (mappingColumns != null)
                {
                    var mappInfo = mappingColumns.SingleOrDefault(it => it.EntityName == type.Name && it.PropertyName.Equals(propertyInfo.Name));
                    if (mappInfo != null)
                    {
                        if (!ReaderKeys.Contains(mappInfo.DbColumnName))
                        {
                            fileName = ReaderKeys.Single(it => it.Equals(mappInfo.DbColumnName, StringComparison.CurrentCultureIgnoreCase));
                        }
                        else
                        {
                            fileName = mappInfo.DbColumnName;
                        }
                    }
                }

                if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
                {
                    if (propertyInfo.PropertyType.IsClass() && propertyInfo.PropertyType != UtilConstants.ByteArrayType)
                    {
                        BindClass(generator, result, propertyInfo);
                    }
                    else
                    {
                        if (this.ReaderKeys.Any(it => it.Equals(fileName, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            BindField(generator, result, propertyInfo, ReaderKeys.Single(it => it.Equals(fileName, StringComparison.CurrentCultureIgnoreCase)));
                        }
                    }
                }
            }
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);
            DynamicBuilder.handler = (Load)method.CreateDelegate(typeof(Load));
            return DynamicBuilder;
        }

        #endregion Public methods

        #region Private methods

        private void BindClass(ILGenerator generator, LocalBuilder result, PropertyInfo propertyInfo)
        {
        }

        private void BindField(ILGenerator generator, LocalBuilder result, PropertyInfo propertyInfo, string fileName)
        {
            int i = DataRecord.GetOrdinal(fileName);
            Label endIfLabel = generator.DefineLabel();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldc_I4, i);
            generator.Emit(OpCodes.Callvirt, isDBNullMethod);
            generator.Emit(OpCodes.Brtrue, endIfLabel);
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldc_I4, i);
            BindMethod(generator, propertyInfo, i);
            generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
            generator.MarkLabel(endIfLabel);
        }

        private void BindMethod(ILGenerator generator, PropertyInfo bindProperty, int ordinal)
        {
            IDbBind bind = Context.Ado.DbBind;
            bool isNullableType = false;
            MethodInfo method = null;
            Type bindPropertyType = Extensions.GetUnderType(bindProperty, ref isNullableType);
            string dbTypeName = DataRecord.GetDataTypeName(ordinal);
            if (Regex.IsMatch(dbTypeName, @"\(.+\)"))
            {
                dbTypeName = Regex.Replace(dbTypeName, @"\(.+\)", "");
            }
            string propertyName = bindProperty.Name;
            string validPropertyName = bind.GetPropertyTypeName(dbTypeName);
            validPropertyName = validPropertyName == "byte[]" ? "byteArray" : validPropertyName;
            CSharpDataType validPropertyType = (CSharpDataType)Enum.Parse(typeof(CSharpDataType), validPropertyName);

            #region Sqlite Logic

            if (this.Context.CurrentConfig.DbType == Entities.DbType.Sqlite)
            {
                if (bindPropertyType.IsEnum())
                {
                    method = isNullableType ? getConvertEnum_Null.MakeGenericMethod(bindPropertyType) : getEnum.MakeGenericMethod(bindPropertyType);
                }
                else if (bindPropertyType == UtilConstants.IntType)
                {
                    method = isNullableType ? getConvertInt32 : getInt32;
                }
                else if (bindPropertyType == UtilConstants.StringType)
                {
                    method = getString;
                }
                else if (bindPropertyType == UtilConstants.ByteArrayType)
                {
                    method = getValueMethod;
                    generator.Emit(OpCodes.Call, method);
                    generator.Emit(OpCodes.Unbox_Any, bindProperty.PropertyType);
                    return;
                }
                else
                {
                    method = isNullableType ? getSqliteTypeNull.MakeGenericMethod(bindPropertyType) : getSqliteType.MakeGenericMethod(bindPropertyType);
                }
                generator.Emit(OpCodes.Call, method);
                return;
            };

            #endregion Sqlite Logic

            #region Common Database Logic

            string bindProperyTypeName = bindPropertyType.Name.ToLower();
            bool isEnum = bindPropertyType.IsEnum();
            if (isEnum) { validPropertyType = CSharpDataType.@enum; }
            switch (validPropertyType)
            {
                case CSharpDataType.@int:
                    CheckType(bind.IntThrow, bindProperyTypeName, validPropertyName, propertyName);
                    if (bindProperyTypeName.IsContainsIn("int", "int32"))
                        method = isNullableType ? getConvertInt32 : getInt32;
                    if (bindProperyTypeName.IsContainsIn("int64"))
                        method = isNullableType ? getConvertInt64 : getInt64;
                    if (bindProperyTypeName.IsContainsIn("byte"))
                        method = isNullableType ? getConvertByte : getByte;
                    if (bindProperyTypeName.IsContainsIn("int16"))
                        method = isNullableType ? getConvertInt16 : getInt16;
                    break;

                case CSharpDataType.@bool:
                    if (bindProperyTypeName == "bool" || bindProperyTypeName == "boolean")
                        method = isNullableType ? getConvertBoolean : getBoolean;
                    break;

                case CSharpDataType.@string:
                    CheckType(bind.StringThrow, bindProperyTypeName, validPropertyName, propertyName);
                    method = getString;
                    if (bindProperyTypeName == "guid")
                    {
                        method = isNullableType ? getConvertStringGuid : getStringGuid;
                    }
                    break;

                case CSharpDataType.DateTime:
                    CheckType(bind.DateThrow, bindProperyTypeName, validPropertyName, propertyName);
                    if (bindProperyTypeName == "datetime")
                        method = isNullableType ? getConvertDateTime : getDateTime;
                    break;

                case CSharpDataType.@decimal:
                    CheckType(bind.DecimalThrow, bindProperyTypeName, validPropertyName, propertyName);
                    if (bindProperyTypeName == "decimal")
                        method = isNullableType ? getConvertDecimal : getDecimal;
                    break;

                case CSharpDataType.@float:
                case CSharpDataType.@double:
                    CheckType(bind.DoubleThrow, bindProperyTypeName, validPropertyName, propertyName);
                    if (bindProperyTypeName == "double")
                        method = isNullableType ? getConvertDouble : getDouble;
                    if (bindProperyTypeName == "single")
                        method = isNullableType ? getConvertFloat : getFloat;
                    break;

                case CSharpDataType.Guid:
                    CheckType(bind.GuidThrow, bindProperyTypeName, validPropertyName, propertyName);
                    if (bindProperyTypeName == "guid")
                        method = isNullableType ? getConvertGuid : getGuid;
                    break;

                case CSharpDataType.@byte:
                    if (bindProperyTypeName == "byte")
                        method = isNullableType ? getConvertByte : getByte;
                    break;

                case CSharpDataType.@enum:
                    method = isNullableType ? getConvertEnum_Null.MakeGenericMethod(bindPropertyType) : getEnum.MakeGenericMethod(bindPropertyType);
                    break;

                case CSharpDataType.@short:
                    CheckType(bind.ShortThrow, bindProperyTypeName, validPropertyName, propertyName);
                    if (bindProperyTypeName == "int16" || bindProperyTypeName == "short")
                        method = isNullableType ? getConvertInt16 : getInt16;
                    break;

                case CSharpDataType.@long:
                    if (bindProperyTypeName == "int64" || bindProperyTypeName == "long")
                        method = isNullableType ? getConvertInt64 : getInt64;
                    break;

                default:
                    method = getValueMethod;
                    break;
            }
            if (method == null && bindPropertyType == UtilConstants.StringType)
            {
                method = getConvertString;
            }
            if (method == null)
                method = isNullableType ? getOtherNull.MakeGenericMethod(bindPropertyType) : getOther.MakeGenericMethod(bindPropertyType);
            generator.Emit(OpCodes.Call, method);
            if (method == getValueMethod)
            {
                generator.Emit(OpCodes.Unbox_Any, bindProperty.PropertyType);
            }

            #endregion Common Database Logic
        }

        private void CheckType(List<string> invalidTypes, string bindProperyTypeName, string validPropertyType, string propertyName)
        {
            var isAny = invalidTypes.Contains(bindProperyTypeName);
            if (isAny)
            {
                // throw new UtilExceptions(string.Format("{0} can't  convert {1} to {2}", propertyName, validPropertyType, bindProperyTypeName));
            }
        }

        #endregion Private methods
    }
}