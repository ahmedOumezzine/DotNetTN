﻿using DotNetTN.Connector.SQL.Common;

namespace DotNetTN.Connector.SQL.ExpressionsToSql
{
    public interface IDbMethods
    {
        string IIF(MethodCallExpressionModel model);

        string HasNumber(MethodCallExpressionModel model);

        string HasValue(MethodCallExpressionModel model);

        string IsNullOrEmpty(MethodCallExpressionModel model);

        string ToLower(MethodCallExpressionModel model);

        string ToUpper(MethodCallExpressionModel model);

        string Trim(MethodCallExpressionModel model);

        string Contains(MethodCallExpressionModel model);

        string ContainsArray(MethodCallExpressionModel model);

        string Equals(MethodCallExpressionModel model);

        string DateIsSameDay(MethodCallExpressionModel model);

        string DateIsSameByType(MethodCallExpressionModel model);

        string DateAddByType(MethodCallExpressionModel model);

        string DateValue(MethodCallExpressionModel model);

        string DateAddDay(MethodCallExpressionModel model);

        string Between(MethodCallExpressionModel model);

        string StartsWith(MethodCallExpressionModel model);

        string EndsWith(MethodCallExpressionModel model);

        string ToInt32(MethodCallExpressionModel model);

        string ToInt64(MethodCallExpressionModel model);

        string ToString(MethodCallExpressionModel model);

        string ToGuid(MethodCallExpressionModel model);

        string ToDouble(MethodCallExpressionModel model);

        string ToBool(MethodCallExpressionModel model);

        string Substring(MethodCallExpressionModel model);

        string ToDate(MethodCallExpressionModel model);

        string ToTime(MethodCallExpressionModel model);

        string ToDecimal(MethodCallExpressionModel model);

        string Length(MethodCallExpressionModel model);

        string Replace(MethodCallExpressionModel model);

        string AggregateSum(MethodCallExpressionModel model);

        string AggregateAvg(MethodCallExpressionModel model);

        string AggregateMin(MethodCallExpressionModel model);

        string AggregateMax(MethodCallExpressionModel model);

        string AggregateCount(MethodCallExpressionModel model);

        string MappingColumn(MethodCallExpressionModel model);

        string GetSelfAndAutoFill(string shortName, bool isSingle);

        string True();

        string False();

        string GuidNew();

        string MergeString(params string[] strings);
    }
}