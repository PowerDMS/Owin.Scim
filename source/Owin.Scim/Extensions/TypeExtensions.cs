namespace Owin.Scim.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public static class TypeExtensions
    {
        public static bool IsNonStringEnumerable(this Type type)
        {
            if (type == null) return false;

            return type != typeof(string) &&
            ((type.IsGenericType && type.GetGenericTypeDefinition() == typeof (IEnumerable<>)) ||
                type.GetInterfaces()
                    .Any(
                        interfaceType =>
                            (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof (IEnumerable<>)) ||
                            (interfaceType == typeof (IEnumerable))));
        }

        public static Type GetEnumerableType(this Type type)
        {
            if (type == null) throw new ArgumentNullException();

            var genericEnumerableInterface =
                type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                    ? type
                    : type.GetInterfaces()
                        .SingleOrDefault(
                            interfaceType =>
                                interfaceType.IsGenericType &&
                                interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            return genericEnumerableInterface == null
                ? null
                : genericEnumerableInterface;
        }
    }
}