﻿using System.Collections.Generic;

namespace DotNetTN.Connector.SQL.Common
{
    public class MethodCallExpressionModel
    {
        public List<MethodCallExpressionArgs> Args { get; set; }
    }

    public class MethodCallExpressionArgs
    {
        public bool IsMember { get; set; }
        public object MemberName { get; set; }
        public object MemberValue { get; set; }
    }
}