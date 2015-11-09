// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Owin.Scim.Patching.Operations
{
    using System;

    using Newtonsoft.Json;

    public class OperationBase
    {
        [JsonIgnore]
        public OperationType OperationType
        {
            get
            {
                return (OperationType)Enum.Parse(typeof(OperationType), Operation, true);
            }
        }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("op")]
        public string Operation { get; set; }

        public OperationBase()
        {
        }

        public OperationBase(string operation, string path)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            this.Operation = operation;
            this.Path = path;
        }
    }
}