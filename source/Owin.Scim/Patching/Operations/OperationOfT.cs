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

        public Operation(OperationType operationType, string path, object value)
            : base(operationType, path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            this.Value = value;
        }

        public Operation(OperationType operationType, string path)
            : base(operationType, path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

        }

        public IEnumerable<PatchOperationResult> Apply(TModel objectToApplyTo, IObjectAdapter adapter)
        {
            if (objectToApplyTo == null)
            {
                throw new ArgumentNullException("objectToApplyTo");
            }

            if (adapter == null)
            {
                throw new ArgumentNullException("adapter");
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
                    throw new InvalidOperationException(string.Format("Patch OperationType type '{0}' is invalid.", OperationType));
            }
        }
    }
}