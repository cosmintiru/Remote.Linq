﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.TypeSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using BindingFlags = System.Reflection.BindingFlags;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class NewExpression : Expression
    {
        internal NewExpression(ConstructorInfo constructor, IEnumerable<Expression> arguments, IEnumerable<MemberInfo> members = null)
        {
            Constructor = constructor;
            Arguments = ReferenceEquals(null, arguments) || !arguments.Any() ? null : arguments.ToList().AsReadOnly();
            Members = ReferenceEquals(null, members) || !members.Any() ? null : members.ToList().AsReadOnly();
        }

        internal NewExpression(System.Reflection.ConstructorInfo constructor, IEnumerable<Expression> arguments = null, IEnumerable<System.Reflection.MemberInfo> members = null)
            : this(new ConstructorInfo(constructor), arguments, ReferenceEquals(null, members) ? null : members.Select(x => MemberInfo.Create(x)))
        {
        }

        internal NewExpression(string name, Type declaringType, BindingFlags bindingFlags, Type[] genericArguments, Type[] parameterTypes, IEnumerable<Expression> arguments = null, IEnumerable<System.Reflection.MemberInfo> members = null)
            : this(new ConstructorInfo(name, declaringType, bindingFlags, genericArguments, parameterTypes), arguments)
        {
        }

        public override ExpressionType NodeType { get { return ExpressionType.New; } }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public ConstructorInfo Constructor { get; private set; }

        [DataMember(EmitDefaultValue = true, IsRequired = false)]
        public ReadOnlyCollection<Expression> Arguments { get; private set; }

        [DataMember(EmitDefaultValue = true, IsRequired = false)]
        public ReadOnlyCollection<MemberInfo> Members { get; private set; }

        public override string ToString()
        {
            return string.Format("New {0}({1})", Constructor.DeclaringType, ReferenceEquals(null, Arguments) ? null : string.Join(", ", Arguments.Select(x => x.ToString()).ToArray()));
        }
    }
}
