// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Owin.Scim.Patching.Operations
{
    using System;
    using System.Collections.Generic;

    public class Operation<TModel> : Operation where TModel : class
    {
        public Operation()
        {

        }

        public Operation(string op, string path, object value)
            : base(op, path)
        {
            if (op == null)
            {
                throw new ArgumentNullException(nameof(op));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            this.value = value;
        }

        public Operation(string op, string path)
            : base(op, path)
        {
            if (op == null)
            {
                throw new ArgumentNullException(nameof(op));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

        }

        public IEnumerable<PatchOperation> Apply(TModel objectToApplyTo, IObjectAdapter adapter)
        {
            if (objectToApplyTo == null)
            {
                throw new ArgumentNullException(nameof(objectToApplyTo));
            }

            if (adapter == null)
            {
                throw new ArgumentNullException(nameof(adapter));
            }

            switch (OperationType)
            {
                case OperationType.Add:
                    return adapter.Add(this, objectToApplyTo);
                case OperationType.Remove:
                    return adapter.Remove(this, objectToApplyTo);
                case OperationType.Replace:
                    return adapter.Replace(this, objectToApplyTo);
                default:
                    throw new InvalidOperationException(string.Format("Patch operation type '{0}' is invalid.", OperationType));
            }
        }
    }
}