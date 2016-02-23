// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Owin.Scim.Patching.Operations
{
    using System;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class OperationBase
    {
        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("op", ItemConverterType = typeof(StringEnumConverter))]
        public OperationType OperationType { get; set; }

        public OperationBase()
        {
        }

        public OperationBase(OperationType operationType, string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            this.OperationType = operationType;
            this.Path = path;
        }
    }
}