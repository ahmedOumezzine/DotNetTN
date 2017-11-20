using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DotNetTN.Common.Expression
{
    public class MethodKey
    {
        public static string GetKey<T>(Expression<Func<T>> method, params string[] paramMembers)
        {
            var keys = new Dictionary<string, string>();
            string scope = null;
            string prefix = null;
            ParameterInfo[] formalParams = null;
            object[] actual = null;

            var methodCall = method.Body as MethodCallExpression;
            if (methodCall != null)
            {
                scope = methodCall.Method.DeclaringType.FullName;
                prefix = methodCall.Method.Name;

                IEnumerable<System.Linq.Expressions.Expression> actualParams = methodCall.Arguments;
                actual = actualParams.Select(GetValueOfParameter<T>).ToArray();
                formalParams = methodCall.Method.GetParameters();
            }
            else
            {
                // TODO: Check if the supplied expression is something that makes sense to evaluate as a method, e.g. MemberExpression (method.Body as MemberExpression)

                var objectMember = System.Linq.Expressions.Expression.Convert(method.Body, typeof(object));
                var getterLambda = System.Linq.Expressions.Expression.Lambda<Func<object>>(objectMember);
                var getter = getterLambda.Compile();
                var m = getter();

                var m2 = ((System.Delegate)m);

                var delegateDeclaringType = m2.Method.DeclaringType;
                var actualMethodDeclaringType = delegateDeclaringType.DeclaringType;
                scope = actualMethodDeclaringType.FullName;
                var ar = m2.Target;
                formalParams = m2.Method.GetParameters();
                //var m = (System.MulticastDelegate)((Expression.Lambda<Func<object>>(Expression.Convert(method.Body, typeof(object)))).Compile()())

                //throw new ArgumentException("Caller is not a method", "method");
            }

            // null list of paramMembers should disregard all parameters when creating key.
            if (paramMembers != null)
            {
                for (var i = 0; i < formalParams.Length; i++)
                {
                    var par = formalParams[i];
                    // empty list of paramMembers should be treated as using all parameters
                    if (paramMembers.Length == 0 || paramMembers.Contains(par.Name))
                    {
                        var value = actual[i];
                        keys.Add(par.Name, value.ToString());
                    }
                }

                if (paramMembers.Length != 0 && keys.Count != paramMembers.Length)
                {
                    var notFound = paramMembers.Where(x => !keys.ContainsKey(x));
                    var notFoundString = string.Join(", ", notFound);

                    throw new ArgumentException("Unable to find the following parameters in supplied method: " + notFoundString, "paramMembers");
                }
            }

            return scope + "¤" + prefix + "¤";//+ Flatten(keys)
        }

        private static object GetValueOfParameter<T>(System.Linq.Expressions.Expression parameter)
        {
            LambdaExpression lambda = System.Linq.Expressions.Expression.Lambda(parameter);
            var compiledExpression = lambda.Compile();
            var value = compiledExpression.DynamicInvoke();
            return value;
        }
    }
}