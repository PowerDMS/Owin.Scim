namespace Owin.Scim.Extensions
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    using Antlr;

    using Antlr4.Runtime;

    using NContext.Common;

    using Querying;

    public static class PathFilterExpressionExtensions
    {
        public static Func<T, bool> ToPredicate<T>(this PathFilterExpression filterExpression)
        {
            if (filterExpression == null)
                return user => true;
            
            if (string.IsNullOrWhiteSpace(filterExpression.Filter))
                throw new Exception("Invalid scim filter"); // TODO: (DG) exception handling

            // parse our filter into an expression tree
            var declaryingType = filterExpression.Path == null
                ? typeof(T)
                : typeof(T)
                    .GetProperty(filterExpression.Path, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
                    .PropertyType;

            // if enumerable, get generic argument
            if (declaryingType.IsNonStringEnumerable())
                declaryingType = declaryingType.GetGenericArguments()[0];

            var lexer = new ScimFilterLexer(new AntlrInputStream(filterExpression.Filter));
            var parser = new ScimFilterParser(new CommonTokenStream(lexer));

            // create a visitor for the type of enumerable generic argument
            var filterVisitorType = typeof(ScimFilterVisitor<>).MakeGenericType(declaryingType);
            var filterVisitor = (IScimFilterVisitor)filterVisitorType.CreateInstance();
            var filterDelegateExpression = filterVisitor.VisitExpression(parser.parse());

            if (filterExpression.Path == null)
                return filterDelegateExpression.Compile().AsFunc<T, bool>();

            //Func<Product, object>
            var resourceArg = Expression.Parameter(typeof(T));
            var resourceComplexAttr = Expression.Property(resourceArg, filterExpression.Path);
            var invokeFilterExpression = Expression.Invoke(filterDelegateExpression, resourceComplexAttr);
            var predicate = Expression.Lambda<Func<T, bool>>(invokeFilterExpression, resourceArg);

            return predicate.Compile().AsFunc<T, bool>();
        }
    }
}