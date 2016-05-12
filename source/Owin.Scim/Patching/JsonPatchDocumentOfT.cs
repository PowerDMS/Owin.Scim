// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Owin.Scim.Patching
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Converters;

    using Helpers;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using Operations;

    // Implementation details: the purpose of this type of patch document is to ensure we can do _TokenType-checking
    // when producing a JsonPatchDocument.  However, we cannot send this "typed" over the wire, as that would require
    // including type data in the JsonPatchDocument serialized as JSON (to allow for correct deserialization) - that's
    // not according to RFC 6902, and would thus break cross-platform compatibility.
    [JsonConverter(typeof(TypedJsonPatchDocumentConverter))]
    public class JsonPatchDocument<TModel> : IJsonPatchDocument where TModel : class
    {
        public List<Operation<TModel>> Operations { get; private set; }

        [JsonIgnore]
        public IContractResolver ContractResolver { get; set; }

        public JsonPatchDocument()
        {
            Operations = new List<Operation<TModel>>();
            ContractResolver = new DefaultContractResolver();
        }

        // Create from list of operations
        public JsonPatchDocument(List<Operation<TModel>> operations, IContractResolver contractResolver)
        {
            if (operations == null)
            {
                throw new ArgumentNullException("operations");
            }

            if (contractResolver == null)
            {
                throw new ArgumentNullException("contractResolver");
            }

            Operations = operations;
            ContractResolver = contractResolver;
        }

        /// <summary>
        /// Add the specified value to the attribute.
        /// </summary>
        /// <typeparam name="TProp">value type</typeparam>
        /// <param name="path">target location</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        public JsonPatchDocument<TModel> Add<TProp>(Expression<Func<TModel, TProp>> path, TProp value)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            Operations.Add(new Operation<TModel>(
                OperationType.Add, 
                ExpressionHelpers.GetPath(path).ToLowerInvariant(),
                value: value));

            return this;
        }

        /// <summary>
        /// Add the specified value to the enumeable attribute.
        /// </summary>
        /// <typeparam name="TProp">value type</typeparam>
        /// <param name="path">target location</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        public JsonPatchDocument<TModel> Add<TProp>(Expression<Func<TModel, IEnumerable<TProp>>> path, TProp value)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            Operations.Add(new Operation<TModel>(
                OperationType.Add,
                ExpressionHelpers.GetPath(path).ToLowerInvariant(),
                value: value));

            return this;
        }

        /// <summary>
        /// Remove value at target location.
        /// </summary>
        /// <param name="path">target location</param>
        /// <returns></returns>
        public JsonPatchDocument<TModel> Remove<TProp>(Expression<Func<TModel, TProp>> path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            Operations.Add(new Operation<TModel>(OperationType.Remove, ExpressionHelpers.GetPath(path).ToLowerInvariant()));

            return this;
        }

        /// <summary>
        /// Removes the entire enumerable attribute.
        /// </summary>
        /// <typeparam name="TProp">value type</typeparam>
        /// <param name="path">target location</param>
        /// <returns></returns>
        public JsonPatchDocument<TModel> Remove<TProp>(Expression<Func<TModel, IEnumerable<TProp>>> path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            Operations.Add(new Operation<TModel>(
                OperationType.Remove,
                ExpressionHelpers.GetPath(path).ToLowerInvariant()));

            return this;
        }

        /// <summary>
        /// Replaces the value at the specified location.
        /// </summary>
        /// <param name="path">target location</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        public JsonPatchDocument<TModel> Replace<TProp>(Expression<Func<TModel, TProp>> path, TProp value)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            Operations.Add(new Operation<TModel>(
                OperationType.Replace,
                ExpressionHelpers.GetPath(path).ToLowerInvariant(),
                value: value));

            return this;
        }
        
        /// <summary>
        /// Replaces the entire enumerable at the specified location.
        /// </summary>
        /// <typeparam name="TProp">value type</typeparam>
        /// <param name="path">target location</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        public JsonPatchDocument<TModel> Replace<TProp>(Expression<Func<TModel, IEnumerable<TProp>>> path, TProp value)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            Operations.Add(new Operation<TModel>(
                OperationType.Replace,
                ExpressionHelpers.GetPath(path).ToLowerInvariant(),
                value: value));

            return this;
        }

        /// <summary>
        /// Apply this JsonPatchDocument  
        /// </summary>
        /// <param name="objectToApplyTo">Object to apply the JsonPatchDocument to</param>
        /// <param name="adapter">IObjectAdapter instance to use when applying</param>
        public PatchResult ApplyTo(TModel objectToApplyTo, IObjectAdapter adapter)
        {
            if (objectToApplyTo == null)
            {
                throw new ArgumentNullException("objectToApplyTo");
            }

            if (adapter == null)
            {
                throw new ArgumentNullException("adapter");
            }

            var patchResult = new PatchResult();
            foreach (var op in Operations)
            {
                patchResult.AddRange(op.Apply(objectToApplyTo, adapter));
            }

            return patchResult;
        }

        public IEnumerable<Operation> GetOperations()
        {
            var operations = new List<Operation>();
            if (Operations != null)
            {
                foreach (var op in Operations)
                {
                    var untypedOp = new Operation
                    {
                        OperationType = op.OperationType,
                        Value = op.Value,
                        Path = op.Path
                    };

                    operations.Add(untypedOp);
                }
            }

            return operations;
        }
    }
}