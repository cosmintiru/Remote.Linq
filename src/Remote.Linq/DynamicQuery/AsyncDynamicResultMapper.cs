﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Remote.Linq.Dynamic;
    using Remote.Linq.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

    internal sealed class AsyncDynamicResultMapper : IAsyncQueryResultMapper<IEnumerable<DynamicObject>>
    {
        private readonly IDynamicObjectMapper _mapper;

        public AsyncDynamicResultMapper(IDynamicObjectMapper mapper)
        {
            _mapper = mapper;
        }

        public Task<TResult> MapResultAsync<TResult>(IEnumerable<DynamicObject> source)
        {
            return Task.Factory.StartNew(() => DynamicResultMapper.MapToType<TResult>(source, _mapper));
        }
    }
}
