﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    partial class Expression
    {
        public static MethodCallExpression MethodCall(Expression insatnce, string methodName, Type declaringType, BindingFlags bindingFlags, Type[] genericArguments, Type[] parameterTypes, IEnumerable<Expression> arguments)
        {
            return new MethodCallExpression(insatnce, methodName, declaringType, bindingFlags, genericArguments, parameterTypes, arguments);
        }
    }
}
