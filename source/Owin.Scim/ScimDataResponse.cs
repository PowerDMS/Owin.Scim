namespace Owin.Scim
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;

    using Model;

    using NContext.Common;

    public class ScimDataResponse<T> : ScimResponse<T>
    {
        private readonly T _Data;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScimDataResponse{T}"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public ScimDataResponse(T data)
        {
            var materializedData = MaterializeDataIfNeeded(data);
            _Data = (materializedData == null) ? default(T) : materializedData;
        }

        /// <summary>
        /// Gets the is left.
        /// </summary>
        /// <value>The is left.</_Value>
        public override Boolean IsLeft
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the left value. (Returns null)
        /// </summary>
        /// <returns>Error.</returns>
        public override IEnumerable<ScimError> GetLeft()
        {
            return null;
        }

        /// <summary>
        /// Gets the right value, <see cref="Data"/>.
        /// </summary>
        /// <returns>T.</returns>
        public override T GetRight()
        {
            return _Data;
        }

        private static T MaterializeDataIfNeeded(T data)
        {
            if (typeof(T).GetTypeInfo().IsValueType || data == null)
            {
                return data;
            }

            var dataType = data.GetType();
            var dataTypeInfo = dataType.GetTypeInfo();
            if (!(data is IEnumerable) ||
                !dataTypeInfo.IsGenericType ||
                IsDictionary(dataType))
            {
                return data;
            }

            if (!IsQueryable(dataType) && !dataTypeInfo.IsNestedPrivate)
            {
                return data;
            }

            // Get the last generic argument.
            // .NET has several internal iterable types in LINQ that have multiple generic
            // arguments.  The last is reserved for the actual type used for projection.
            // ex. WhereSelectArrayIterator, WhereSelectEnumerableIterator, WhereSelectListIterator
            var genericType = dataType.GenericTypeArguments.Last();
            if (dataType.GetGenericTypeDefinition() == typeof(Collection<>))
            {
                var collectionType = typeof(Collection<>).MakeGenericType(genericType);
                return (T)collectionType.CreateInstance(data);
            }

            var listType = typeof(List<>).MakeGenericType(genericType);
            return (T)listType.CreateInstance(data);
        }

        private static Boolean IsDictionary(Type type)
        {
            if (type == null) return false;

            var typeInfo = type.GetTypeInfo();

            return (typeInfo.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>)) ||
                   typeInfo.ImplementedInterfaces
                       .Any(interfaceType => interfaceType.GetTypeInfo().IsGenericType &&
                                             interfaceType.GetGenericTypeDefinition() == typeof(IDictionary<,>));
        }

        private static Boolean IsQueryable(Type type)
        {
            if (type == null) return false;

            var typeInfo = type.GetTypeInfo();

            return
                (typeInfo.IsGenericType && type.GetGenericTypeDefinition() == typeof(IQueryable<>)) ||
                typeInfo.ImplementedInterfaces
                    .Any(interfaceType => interfaceType.GetTypeInfo().IsGenericType &&
                                          interfaceType.GetGenericTypeDefinition() == typeof(IQueryable<>));
        }
    }
}