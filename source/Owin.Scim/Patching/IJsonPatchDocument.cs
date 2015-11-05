// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Owin.Scim.Patching
{
    using System.Collections.Generic;

    using Newtonsoft.Json.Serialization;

    using Operations;

    public interface IJsonPatchDocument
    {
        IContractResolver ContractResolver { get; set; }

        IEnumerable<Operation> GetOperations();
    }
}