﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal sealed partial class RemoteQueryable<T> : RemoteQueryable, IRemoteQueryable<T>
    {
        internal RemoteQueryable(IRemoteQueryProvider provider)
            : base(typeof(T), provider)
        {
        }

        internal RemoteQueryable(IRemoteQueryProvider provider, Expression expression)
            : base(typeof(T), provider, expression)
        {
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return (_provider.Execute<IEnumerable<T>>(_expression)).GetEnumerator();
        }
    }
}
