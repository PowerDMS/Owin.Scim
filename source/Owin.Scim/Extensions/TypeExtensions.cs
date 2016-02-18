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

            return type != typeof (string) &&
                   ((type.IsGenericType && type.GetGenericTypeDefinition() == typeof (IEnumerable<>)) ||
                    type.GetInterfaces()
                        .Any(
                            interfaceType =>
                                (interfaceType.IsGenericType &&
                                 interfaceType.GetGenericTypeDefinition() == typeof (IEnumerable<>)) ||
                                (interfaceType == typeof (IEnumerable))));
        }

        public static Type GetEnumerableType(this Type type)
        {
            if (type == null) throw new ArgumentNullException();

            var genericEnumerableInterface =
                type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof (IEnumerable<>)
                    ? type
                    : type.GetInterfaces()
                        .SingleOrDefault(
                            interfaceType =>
                                interfaceType.IsGenericType &&
                                interfaceType.GetGenericTypeDefinition() == typeof (IEnumerable<>));

            return genericEnumerableInterface == null
                ? null
                : genericEnumerableInterface;
        }

        public static object GetDefaultValue(this Type type)
        {
            return type.IsValueType
                ? Activator.CreateInstance(type)
                : null;
        }

        public static bool IsTerminalObject(this Type type)
        {
            if (type == null) return true;

            return type.IsPrimitive || 
                   type.IsEnum || 
                   type.IsPointer || 
                   type == typeof (string) || 
                   type == typeof (DateTime) || 
                   type == typeof (decimal) || 
                   type == typeof(Uri) || 
                   type == typeof (Guid) || 
                   type == typeof (DateTimeOffset) || 
                   type == typeof (TimeSpan);
        }
    }
}