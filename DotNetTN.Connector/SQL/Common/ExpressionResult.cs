﻿using DotNetTN.Connector.SQL.Entities;
using DotNetTN.Connector.SQL.ExpressionsToSql;
using System.Collections.Generic;
using System.Text;

namespace DotNetTN.Connector.SQL.Common
{
    public class ExpressionResult
    {
        public bool IsLockCurrentParameter { get; set; }
        public bool IsUpper { get; set; }
        private ExpressionParameter _CurrentParameter;

        public ExpressionParameter CurrentParameter
        {
            get
            {
                return this._CurrentParameter;
            }
            set
            {
                //  Check.Exception(value != null && IsLockCurrentParameter, "CurrentParameter is locked.");
                this._CurrentParameter = value;
                this.IsLockCurrentParameter = false;
            }
        }

        #region constructor

        private ExpressionResult()
        {
        }

        public ExpressionResult(ResolveExpressType resolveExpressType)
        {
            this._ResolveExpressType = resolveExpressType;
        }

        #endregion constructor

        #region Fields

        private ResolveExpressType _ResolveExpressType;
        private StringBuilder _Result;

        #endregion Fields

        #region properties

        private StringBuilder Result
        {
            get
            {
                if (_Result == null) _Result = new StringBuilder();
                return _Result;
            }

            set
            {
                _Result = value;
            }
        }

        #endregion properties

        public string GetString()
        {
            if (_Result == null) return null;
            if (IsUpper)
                return _Result.ToString().ToUpper().TrimEnd(',');
            else
                return _Result.ToString().TrimEnd(',');
        }

        #region functions

        public string[] GetResultArray()
        {
            if (this._Result == null) return null;
            if (IsUpper)
                return this.Result.ToString().ToUpper().TrimEnd(',').Split(',');
            else
                return this.Result.ToString().TrimEnd(',').Split(',');
        }

        public string GetResultString()
        {
            if (this._Result == null) return null;
            if (this._ResolveExpressType.IsIn(ResolveExpressType.SelectMultiple, ResolveExpressType.SelectSingle))
            {
                return this.Result.ToString().TrimEnd(',');
            }
            if (IsUpper)
                return this.Result.ToString().ToUpper();
            else
                return this.Result.ToString();
        }

        public void TrimEnd()
        {
            if (this._Result == null) return;
            this.Result = this.Result.Remove(this.Result.Length - 1, 1);
        }

        public bool Contains(string value)
        {
            if (this.Result.Equals(value)) return true;
            return (this.Result.ToString().Contains(value));
        }

        internal void Insert(int index, string value)
        {
            if (this.Result == null) this.Result.Append(value);
            this.Result.Insert(index, value);
        }

        public void Append(object parameter)
        {
            if (this.CurrentParameter.IsValuable() && this.CurrentParameter.AppendType.IsIn(ExpressionResultAppendType.AppendTempDate))
            {
                this.CurrentParameter.CommonTempData = parameter;
                return;
            }
            switch (this._ResolveExpressType)
            {
                case ResolveExpressType.ArraySingle:
                case ResolveExpressType.ArrayMultiple:
                case ResolveExpressType.SelectSingle:
                case ResolveExpressType.SelectMultiple:
                case ResolveExpressType.Update:
                    parameter = parameter + ",";
                    break;

                case ResolveExpressType.WhereSingle:
                    break;

                case ResolveExpressType.WhereMultiple:
                    break;

                case ResolveExpressType.FieldSingle:
                    break;

                case ResolveExpressType.FieldMultiple:
                    break;

                default:
                    break;
            }
            this.Result.Append(parameter);
        }

        public void AppendFormat(string parameter, params object[] orgs)
        {
            if (this.CurrentParameter.IsValuable() && this.CurrentParameter.AppendType.IsIn(ExpressionResultAppendType.AppendTempDate))
            {
                this.CurrentParameter.CommonTempData = new KeyValuePair<string, object[]>(parameter, orgs);
                return;
            }
            switch (this._ResolveExpressType)
            {
                case ResolveExpressType.SelectSingle:
                case ResolveExpressType.SelectMultiple:
                    parameter = parameter + ",";
                    break;

                case ResolveExpressType.WhereSingle:
                    break;

                case ResolveExpressType.WhereMultiple:
                    break;

                case ResolveExpressType.FieldSingle:
                    break;

                case ResolveExpressType.FieldMultiple:
                    break;

                default:
                    break;
            }
            this.Result.AppendFormat(parameter, orgs);
        }

        public void Replace(string parameter, string newValue)
        {
            this.Result.Replace(parameter, newValue);
        }

        #endregion functions
    }
}