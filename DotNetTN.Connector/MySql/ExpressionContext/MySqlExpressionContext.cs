using DotNetTN.Connector.SQL;
using DotNetTN.Connector.SQL.Common;
using DotNetTN.Connector.SQL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetTN.Connector.SQL.Entities;
using System.Linq.Expressions;

namespace DotNetTN.Connector.MySql.ExpressionContext
{
    public class MySqlExpressionContext : DotNetTN.Connector.SQL.ExpressionsToSql.ExpressionContext, ILambdaExpressions
    {
        private IDbMethods _DbMehtods { get; set; }
        public IDbMethods DbMehtods
        {
            get
            {
                if (_DbMehtods == null)
                {
                    _DbMehtods = new MySqlMethod();
                }
                return _DbMehtods;
            }
            set
            {
                _DbMehtods = value;
            }
        }
        public SqlClient Context { get; set; }
        public MySqlExpressionContext()
        {
            this.DbMehtods = new MySqlMethod();
        }
        public  string SqlTranslationLeft { get { return "`"; } }
        public  string SqlTranslationRight { get { return "`"; } }

        public MappingColumnList MappingColumns { get; set; }
        public MappingTableList MappingTables { get; set; }

        
        public void Clear()
        {
           
        }
        public void Resolve(Expression expression, ResolveExpressType resolveType)
        {
           
        }
    }
    public class MySqlMethod : DefaultDbMethod, IDbMethods
    {
        public override string DateValue(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" {0}({1}) ", parameter2.MemberValue, parameter.MemberName);
        }

        public override string Contains(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} like concat('%',{1},'%')) ", parameter.MemberName, parameter2.MemberName);
        }

        public override string StartsWith(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} like concat({1},'%')) ", parameter.MemberName, parameter2.MemberName);
        }

        public override string EndsWith(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} like concat('%',{1}))", parameter.MemberName, parameter2.MemberName);
        }

        public override string DateIsSameDay(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" (TIMESTAMPDIFF(day,{0},{1})=0) ", parameter.MemberName, parameter2.MemberName); ;
        }

        public override string DateIsSameByType(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            return string.Format(" (TIMESTAMPDIFF({2},{0},{1})=0) ", parameter.MemberName, parameter2.MemberName, parameter3.MemberValue);
        }

        public override string DateAddByType(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            return string.Format(" (DATE_ADD({1} , INTERVAL {2} {0})) ", parameter3.MemberValue, parameter.MemberName, parameter2.MemberName);
        }

        public override string DateAddDay(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" (DATE_ADD({1} INTERVAL {0} day)) ", parameter.MemberName, parameter2.MemberName);
        }

        public override string ToInt32(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS SIGNED)", parameter.MemberName);
        }

        public override string ToInt64(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS SIGNED)", parameter.MemberName);
        }

        public override string ToString(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS CHAR)", parameter.MemberName);
        }

        public override string ToGuid(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS CHAR)", parameter.MemberName);
        }

        public override string ToDouble(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS DECIMAL(18,4))", parameter.MemberName);
        }

        public override string ToBool(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS SIGNED)", parameter.MemberName);
        }

        public override string ToDecimal(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS DECIMAL(18,4))", parameter.MemberName);
        }

        public override string Length(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" LENGTH({0})", parameter.MemberName);
        }
        public override string MergeString(params string[] strings)
        {
            return " concat(" + string.Join(",", strings).Replace("+", "") + ") ";
        }
    }
}