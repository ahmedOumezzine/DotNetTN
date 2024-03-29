﻿using System;
using System.Collections.Generic;
using System.Data;

namespace DotNetTN.Connector.SQL.Common
{
    public class Parameter
    {
        public Parameter(string name, object value)
        {
            this.Value = value;
            this.ParameterName = name;
            if (value != null)
            {
                SettingDataType(value.GetType());
            }
        }

        public Parameter(string name, object value, Type type)
        {
            this.Value = value;
            this.ParameterName = name;
            SettingDataType(type);
        }

        private void SettingDataType(Type type)
        {
            if (type == UtilConstants.ByteArrayType)
            {
                this.DbType = System.Data.DbType.Binary;
            }
            else if (type == UtilConstants.GuidType)
            {
                this.DbType = System.Data.DbType.Guid;
            }
            else if (type == UtilConstants.IntType)
            {
                this.DbType = System.Data.DbType.Int32;
            }
            else if (type == UtilConstants.ShortType)
            {
                this.DbType = System.Data.DbType.Int16;
            }
            else if (type == UtilConstants.LongType)
            {
                this.DbType = System.Data.DbType.Int64;
            }
            else if (type == UtilConstants.DateType)
            {
                this.DbType = System.Data.DbType.DateTime;
            }
            else if (type == UtilConstants.DobType)
            {
                this.DbType = System.Data.DbType.Double;
            }
            else if (type == UtilConstants.DecType)
            {
                this.DbType = System.Data.DbType.Decimal;
            }
            else if (type == UtilConstants.ByteType)
            {
                this.DbType = System.Data.DbType.Byte;
            }
            else if (type == UtilConstants.FloatType)
            {
                this.DbType = System.Data.DbType.Single;
            }
            else if (type == UtilConstants.BoolType)
            {
                this.DbType = System.Data.DbType.Boolean;
            }
            else if (type == UtilConstants.StringType)
            {
                this.DbType = System.Data.DbType.String;
            }
        }

        public ParameterDirection Direction
        {
            get; set;
        }

        public System.Data.DbType DbType
        {
            get; set;
        }

        public bool IsNullable
        {
            get; set;
        }

        public string ParameterName
        {
            get; set;
        }

        public int _Size;

        public int Size
        {
            get
            {
                if (_Size == 0 && Value != null)
                {
                    var isByteArray = Value.GetType() == UtilConstants.ByteArrayType;
                    if (isByteArray)
                        _Size = -1;
                    else
                    {
                        var length = Value.ToString().Length;
                        _Size = length < 4000 ? 4000 : -1;
                    }
                }
                if (_Size == 0)
                    _Size = 4000;
                return _Size;
            }
            set
            {
                _Size = value;
            }
        }

        public string SourceColumn
        {
            get; set;
        }

        public bool SourceColumnNullMapping
        {
            get; set;
        }

        public string UdtTypeName
        {
            get;
            set;
        }

        public object Value
        {
            get; set;
        }

        public Dictionary<string, object> TempDate
        {
            get; set;
        }
    }
}