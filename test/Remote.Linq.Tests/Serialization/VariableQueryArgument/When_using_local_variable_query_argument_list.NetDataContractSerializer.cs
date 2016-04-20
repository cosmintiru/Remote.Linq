﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#if NET

namespace Remote.Linq.Tests.Serialization.VariableQueryArgument
{
    partial class When_using_local_variable_query_argument_list
    {
        private class NetDataContractSerializer : When_using_local_variable_query_argument_list
        {
            public NetDataContractSerializer()
                : base(NetDataContractSerializationHelper.Serialize)
            {
            }
        }
    }
}

#endif