// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Owin.Scim.Patching.Operations
{
    using System;

    public class Operation<TModel> : Operation where TModel : class
    {
        public Operation()
        {

        }

        public Operation(string op, string path, string from, object value)
            : base(op, path, from)
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

        public Operation(string op, string path, string from)
            : base(op, path, from)
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

        public void Apply(TModel objectToApplyTo, IObjectAdapter adapter)
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
                    adapter.Add(this, objectToApplyTo);
                    break;
                case OperationType.Remove:
                    adapter.Remove(this, objectToApplyTo);
                    break;
                case OperationType.Replace:
                    adapter.Replace(this, objectToApplyTo);
                    break;
            }
        }
    }
}