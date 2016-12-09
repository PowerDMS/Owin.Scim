namespace Owin.Scim.Querying
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Antlr;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;

    using Extensions;

    using Model;

    using NContext.Common;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;
    using Owin.Scim.Configuration;

    public class ScimFilterVisitor<TResource> : ScimFilterBaseVisitor<LambdaExpression>, IScimFilterVisitor
    {
        private static readonly ConcurrentDictionary<Type, IDictionary<string, PropertyInfo>> _PropertyCache = 
            new ConcurrentDictionary<Type, IDictionary<string, PropertyInfo>>();

        private static readonly Lazy<IDictionary<string, MethodInfo>> _MethodCache =
            new Lazy<IDictionary<string, MethodInfo>>(CreateMethodCache);

        private static readonly JsonSerializer _JsonSerializer;

        protected ScimServerConfiguration ServerConfiguration { get; private set; }

        public ScimFilterVisitor(ScimServerConfiguration scimServerConfiguration)
        {
            ServerConfiguration = scimServerConfiguration;
        }

        static ScimFilterVisitor()
        {
            _JsonSerializer = new JsonSerializer
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };
        }

        protected static IDictionary<string, MethodInfo> MethodCache
        {
            get { return _MethodCache.Value; }
        }

        protected static ConcurrentDictionary<Type, IDictionary<string, PropertyInfo>> PropertyCache
        {
            get { return _PropertyCache; }
        }

        public LambdaExpression VisitExpression(IParseTree tree)
        {
            return Visit(tree);
        }

        public override LambdaExpression VisitAndExp(ScimFilterParser.AndExpContext context)
        {
            var left = Visit(context.filter(0));
            var right = Visit(context.filter(1));

            var argument = Expression.Parameter(typeof(TResource));
            var resultBody = Expression.And(Expression.Invoke(left, argument), Expression.Invoke(right, argument));

            return Expression.Lambda<Func<TResource, bool>>(resultBody, argument);
        }

        public override LambdaExpression VisitBraceExp(ScimFilterParser.BraceExpContext context)
        {
            var predicate = Visit(context.filter());
            if (context.NOT() != null)
            {
                var argument = Expression.Parameter(typeof(TResource));
                var resultBody = Expression.Not(Expression.Invoke(predicate, argument));

                return Expression.Lambda<Func<TResource, bool>>(resultBody, argument);
            }

            return predicate;
        }

        public override LambdaExpression VisitValPathExp(ScimFilterParser.ValPathExpContext context)
        {
            // brackets MAY change the field type (TResource) thus, the expression within the brackets
            // should be visited in context of the new field's type

            var argument = Expression.Parameter(typeof(TResource));
            var attrPathExpression = Visit(context.attrPath());

            if (attrPathExpression.ReturnType != typeof (TResource))
            {
                Type childFilterType = attrPathExpression.ReturnType;
                bool isEnumerable = childFilterType.IsNonStringEnumerable();

                if (isEnumerable)
                {
                    childFilterType = childFilterType.GetGenericArguments()[0]; // set childFilterType to enumerable type argument
                }

                var childVisitorType = typeof (ScimFilterVisitor<>).MakeGenericType(childFilterType);
                var childVisitor = (IScimFilterVisitor) childVisitorType.CreateInstance(ServerConfiguration);
                var childLambda = childVisitor.VisitExpression(context.valPathFilter()); // Visit the nested filter expression.
                var childLambdaArgument = Expression.TryCatch(
                    Expression.Block(Expression.Invoke(attrPathExpression, argument)),
                    Expression.Catch(typeof(Exception),
                        Expression.Constant(attrPathExpression.ReturnType.GetDefaultValue(), attrPathExpression.ReturnType))
                    );

                if (isEnumerable)
                {
                    // if we have an enumerable, then we need to see if any of its elements satisfy the childLambda
                    // to accomplish this, let's just make use of .NET's Any<TSource>(enumerable, predicate)

                    var anyMethod = MethodCache["any"].MakeGenericMethod(childFilterType);
                    var anyPredicate = Expression.TryCatch(
                        Expression.Block(
                            Expression.Call(
                                anyMethod,
                                new List<Expression>
                                {
                                    childLambdaArgument,
                                    childLambda
                                })),
                        Expression.Catch(typeof (ArgumentNullException), Expression.Constant(false)));

                    return Expression.Lambda(anyPredicate, argument);
                }

                return Expression.Lambda(
                    Expression.Invoke(
                        childLambda, 
                        Expression.Invoke(attrPathExpression, argument)), 
                    argument);
            }

            // TODO: (DG) This is probably incorrect if the property is nested and the same type as its parent.
            // We'll most likely still need a childLambda.
            return Visit(context.valPathFilter());
        }

//        public override LambdaExpression VisitNotExp(ScimFilterParser.NotExpContext context)
//        {
//            var predicate = Visit(context.expression());

//            var parameter = Expression.Parameter(typeof(TResource));
//            var resultBody = Expression.Not(Expression.Invoke(predicate, parameter));

//            return Expression.Lambda<Func<TResource, bool>>(resultBody, parameter);
//        }

        public override LambdaExpression VisitOperatorExp(ScimFilterParser.OperatorExpContext context)
        {
            var argument = Expression.Parameter(typeof(TResource));
            var attrPathExpression = Visit(context.attrPath());
            var operatorToken = context.COMPAREOPERATOR().GetText().ToLower();
            var valueToken = context.VALUE().GetText().Trim('"');

            var isEnumerable = attrPathExpression.ReturnType.IsNonStringEnumerable();

            var left = Expression.TryCatch(
                Expression.Block(Expression.Invoke(attrPathExpression, argument)),
                Expression.Catch(
                    typeof(NullReferenceException),
                    Expression.Constant(attrPathExpression.ReturnType.GetDefaultValue(), attrPathExpression.ReturnType))
                );

            if (isEnumerable &&
                attrPathExpression.ReturnType.IsGenericType &&
                typeof(MultiValuedAttribute).IsAssignableFrom(attrPathExpression.ReturnType.GetGenericArguments()[0]))
            {
                // we're filtering an enumerable of multivaluedattribute without a sub-attribute
                // therefore, we default to evaluating the .Value member

                var multiValuedAttributeType = attrPathExpression.ReturnType.GetGenericArguments()[0];
                var multiValuedAttribute = Expression.Parameter(multiValuedAttributeType);
                var valueAttribute = multiValuedAttributeType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
                var valueExpression = Expression.TryCatch(
                    Expression.Block(Expression.Property(multiValuedAttribute, valueAttribute)),
                    Expression.Catch(
                        typeof (NullReferenceException),
                        Expression.Constant(valueAttribute.PropertyType.GetDefaultValue(), valueAttribute.PropertyType))
                    );

                var valueLambda = Expression.Lambda(
                    CreateBinaryExpression(valueExpression, operatorToken, valueToken),
                    multiValuedAttribute);

                var anyMethod = MethodCache["any"].MakeGenericMethod(multiValuedAttributeType);
                var anyPredicate = Expression.TryCatch(
                        Expression.Block(
                            Expression.Call(
                                anyMethod,
                                new List<Expression>
                                {
                                    left,
                                    valueLambda
                                })),
                        Expression.Catch(typeof(ArgumentNullException), Expression.Constant(false)));

                 return Expression.Lambda(anyPredicate, argument);
            }

            return Expression.Lambda<Func<TResource, bool>>(
                CreateBinaryExpression(left, operatorToken, valueToken),
                argument);
        }

        public override LambdaExpression VisitOrExp(ScimFilterParser.OrExpContext context)
        {
            var left = Visit(context.filter(0));
            var right = Visit(context.filter(1));

            var parameter = Expression.Parameter(typeof(TResource));
            var resultBody = Expression.Or(Expression.Invoke(left, parameter), Expression.Invoke(right, parameter));

            return Expression.Lambda<Func<TResource, bool>>(resultBody, parameter);
        }

        public override LambdaExpression VisitPresentExp(ScimFilterParser.PresentExpContext context)
        {
            var argument = Expression.Parameter(typeof(TResource));
            var attrPathExpression = Visit(context.attrPath());

            return Expression.Lambda<Func<TResource,bool>>(
                Expression.Call(
                    MethodCache["pr"].MakeGenericMethod(attrPathExpression.ReturnType),
                    Expression.Invoke(attrPathExpression, argument)),
                argument);
        }

        public override LambdaExpression VisitValPathAndExp([NotNull] ScimFilterParser.ValPathAndExpContext context)
        {
            var left = Visit(context.valPathFilter(0));
            var right = Visit(context.valPathFilter(1));

            var argument = Expression.Parameter(typeof(TResource));
            var resultBody = Expression.And(Expression.Invoke(left, argument), Expression.Invoke(right, argument));

            return Expression.Lambda<Func<TResource, bool>>(resultBody, argument);
        }

        public override LambdaExpression VisitValPathBraceExp(ScimFilterParser.ValPathBraceExpContext context)
        {
            var predicate = Visit(context.valPathFilter());
            if (context.NOT() != null)
            {
                var argument = Expression.Parameter(typeof(TResource));
                var resultBody = Expression.Not(Expression.Invoke(predicate, argument));

                return Expression.Lambda<Func<TResource, bool>>(resultBody, argument);
            }

            return predicate;
        }

        public override LambdaExpression VisitValPathOperatorExp(ScimFilterParser.ValPathOperatorExpContext context)
        {
            var argument = Expression.Parameter(typeof(TResource));
            var attrPathExpression = Visit(context.attrPath());
            var operatorToken = context.COMPAREOPERATOR().GetText().ToLower();
            var valueToken = context.VALUE().GetText().Trim('"');

            var isEnumerable = attrPathExpression.ReturnType.IsNonStringEnumerable();

            var left = Expression.TryCatch(
                Expression.Block(Expression.Invoke(attrPathExpression, argument)),
                Expression.Catch(
                    typeof(NullReferenceException),
                    Expression.Constant(attrPathExpression.ReturnType.GetDefaultValue(), attrPathExpression.ReturnType)
                ));

            if (isEnumerable &&
                attrPathExpression.ReturnType.IsGenericType &&
                typeof(MultiValuedAttribute).IsAssignableFrom(attrPathExpression.ReturnType.GetGenericArguments()[0]))
            {
                // we're filtering an enumerable of multivaluedattribute without a sub-attribute
                // therefore, we default to evaluating the .Value member

                var multiValuedAttributeType = attrPathExpression.ReturnType.GetGenericArguments()[0];
                var multiValuedAttribute = Expression.Parameter(multiValuedAttributeType);
                var valueAttribute = multiValuedAttributeType.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
                var valueExpression = Expression.TryCatch(
                    Expression.Block(Expression.Property(multiValuedAttribute, valueAttribute)),
                    Expression.Catch(
                        typeof(NullReferenceException),
                        Expression.Constant(valueAttribute.PropertyType.GetDefaultValue(), valueAttribute.PropertyType))
                    );

                var valueLambda = Expression.Lambda(
                    CreateBinaryExpression(valueExpression, operatorToken, valueToken),
                    multiValuedAttribute);

                var anyMethod = MethodCache["any"].MakeGenericMethod(multiValuedAttributeType);
                var anyPredicate = Expression.TryCatch(
                        Expression.Block(
                            Expression.Call(
                                anyMethod,
                                new List<Expression>
                                {
                                    left,
                                    valueLambda
                                })),
                        Expression.Catch(typeof(ArgumentNullException), Expression.Constant(false)));

                return Expression.Lambda(anyPredicate, argument);
            }

            return Expression.Lambda<Func<TResource, bool>>(
                CreateBinaryExpression(left, operatorToken, valueToken),
                argument);
        }

        public override LambdaExpression VisitValPathOrExp(ScimFilterParser.ValPathOrExpContext context)
        {
            var left = Visit(context.valPathFilter(0));
            var right = Visit(context.valPathFilter(1));

            var argument = Expression.Parameter(typeof(TResource));
            var resultBody = Expression.Or(Expression.Invoke(left, argument), Expression.Invoke(right, argument));

            return Expression.Lambda<Func<TResource, bool>>(resultBody, argument);
        }

        public override LambdaExpression VisitValPathPresentExp(ScimFilterParser.ValPathPresentExpContext context)
        {
            var argument = Expression.Parameter(typeof(TResource));
            var attrPathExpression = Visit(context.attrPath());

            return Expression.Lambda<Func<TResource, bool>>(
                Expression.Call(
                    MethodCache["pr"].MakeGenericMethod(attrPathExpression.ReturnType), Expression.Invoke(attrPathExpression, argument)),
                argument);
        }

        public override LambdaExpression VisitAttrPath(ScimFilterParser.AttrPathContext context)
        {
            string schemaToken = GetSchema(context);

            if (!string.IsNullOrEmpty(schemaToken) &&  ServerConfiguration.ResourceExtensionExists(schemaToken))
            {
                return VisitResourceExtensionAttrPath(context);
            }

            if (!string.IsNullOrEmpty(schemaToken)) // fully qualified property
            {
                string schemaIdentifierForResourceType = ServerConfiguration.GetSchemaIdentifierForResourceType(typeof(TResource));
                // swallow correct namespace but validate

                if (!string.Equals(schemaToken, schemaIdentifierForResourceType, StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("unrecognized schema"); // TODO: (MR) make proper error
                }
            }

            string propNameToken = context.ATTRNAME(0).GetText();
            var argument = Expression.Parameter(typeof(TResource));
            PropertyInfo propertyInfo = GetPropertyInfoFromCache(typeof(TResource), propNameToken);

            return Expression.Lambda(Expression.Property(argument, propertyInfo), argument);
        }

        public virtual LambdaExpression VisitResourceExtensionAttrPath(ScimFilterParser.AttrPathContext context)
        {
            /* We want to achieve something like this below
             * Func<TResource, e.g. EnterpriseUserExtension.Manager.GetType()> lambda = r => r.Extensions
             *                                                                .Select(k => k.Value)
             *                                                                .OfType<EnterpriseUserExtension>()
             *                                                                .Select(e => e.Manager)
             *                                                                .FirstOrDefault();
             */

            var argument = Expression.Parameter(typeof(TResource));
            string propNameToken = context.ATTRNAME(0).GetText();
            string schemaToken = GetSchema(context);
            Type extensionType = ServerConfiguration.ResourceExtensionSchemas[schemaToken];

            PropertyInfo extensionsPropInfo = GetPropertyInfoFromCache(typeof(TResource), "Extensions");
            var extensionPropertyExpression = Expression.Property(argument, extensionsPropInfo);
            var keyValuePairArgument = Expression.Parameter(typeof(KeyValuePair<string, ResourceExtension>));

            var selectExtensionExpression = Expression.Call(
                typeof(Enumerable),
                "Select",
                new[]
                {
                    typeof(KeyValuePair<string, ResourceExtension>),
                    typeof(ResourceExtension)
                },
                extensionPropertyExpression,
                Expression.Lambda<Func<KeyValuePair<string, ResourceExtension>, ResourceExtension>>(
                    Expression.Property(keyValuePairArgument,"Value"), keyValuePairArgument));

            var selectTypedExtensionExpression = Expression.Call(
                typeof(Enumerable),
                "OfType",
                new[]
                {
                    extensionType
                },
                selectExtensionExpression);

            PropertyInfo propertyInfo = GetPropertyInfoFromCache(extensionType, propNameToken);
            var extensionArgument = Expression.Parameter(extensionType);

            var selectTypedExtensionPropertyExpression = Expression.Call(
                typeof(Enumerable),
                "Select",
                new[]
                {
                    extensionType, propertyInfo.PropertyType
                },
                selectTypedExtensionExpression,
                MakeLambdaExpression(extensionType, propertyInfo.PropertyType,
                                     Expression.Property(extensionArgument, propertyInfo),
                                     extensionArgument));

            var firstExpression = Expression.Call(
                typeof(Enumerable),
                "FirstOrDefault",
                new[]
                {
                    propertyInfo.PropertyType
                },
                selectTypedExtensionPropertyExpression);

            return MakeLambdaExpression(typeof(TResource), propertyInfo.PropertyType, firstExpression, argument);
        }

        protected Expression CreateBinaryExpression(Expression left, string operatorToken, string valueToken)
        {
            // Equal
            Type propertyType = left.Type;

            if (operatorToken.Equals("eq"))
            {
                int intValue;
                if (propertyType == typeof(int) && int.TryParse(valueToken, out intValue))
                {
                    return Expression.Equal(left, Expression.Constant(intValue));
                }

                bool boolValue;
                if (propertyType == typeof(bool) && bool.TryParse(valueToken, out boolValue))
                {
                    return Expression.Equal(left, Expression.Constant(boolValue));
                }
                
                if (propertyType == typeof(DateTime))
                {
                    return Expression.Equal(left, Expression.Constant(ParseDateTime(valueToken)));
                }

                if (propertyType != typeof(string))
                {
                    return Expression.Equal(left, Expression.Constant(valueToken));
                }

                return Expression.Call(
                    MethodCache["eq"],
                    new List<Expression>
                    {
                        left,
                        Expression.Constant(valueToken),
                        Expression.Constant(StringComparison.OrdinalIgnoreCase)
                    });
            }

            // Not Equal
            if (operatorToken.Equals("ne"))
            {
                int intValue;
                if (propertyType == typeof(int) && int.TryParse(valueToken, out intValue))
                {
                    return Expression.NotEqual(left, Expression.Constant(intValue));
                }

                bool boolValue;
                if (propertyType == typeof(bool) && bool.TryParse(valueToken, out boolValue))
                {
                    return Expression.NotEqual(left, Expression.Constant(boolValue));
                }
                
                if (propertyType == typeof(DateTime))
                {
                    return Expression.NotEqual(left, Expression.Constant(ParseDateTime(valueToken)));
                }

                if (propertyType != typeof(string))
                {
                    return Expression.NotEqual(left, Expression.Constant(valueToken));
                }

                return Expression.IsFalse(
                    Expression.Call(
                        MethodCache["eq"],
                        new List<Expression>
                        {
                            left,
                            Expression.Constant(valueToken),
                            Expression.Constant(StringComparison.OrdinalIgnoreCase)
                        }));
            }

            // Contains
            if (operatorToken.Equals("co"))
            {
                if (propertyType != typeof(string))
                {
                    throw new InvalidOperationException("co only works on strings");
                }

                return Expression.Call(
                    MethodCache["co"],
                    new List<Expression>
                    {
                        left,
                        Expression.Constant(valueToken)
                    });
            }

            // Starts With
            if (operatorToken.Equals("sw"))
            {
                if (propertyType != typeof (string))
                {
                    throw new InvalidOperationException("sw only works with strings");
                }

                return Expression.Call(
                    MethodCache["sw"],
                    new List<Expression>
                    {
                        left,
                        Expression.Constant(valueToken)
                    });
            }

            // Ends With
            if (operatorToken.Equals("ew"))
            {
                if (propertyType != typeof (string))
                {
                    throw new InvalidOperationException("ew only works with strings");
                }

                return Expression.Call(
                    MethodCache["ew"],
                        new List<Expression>
                        {
                            left,
                            Expression.Constant(valueToken)
                        });
            }

            // Greater Than
            if (operatorToken.Equals("gt"))
            {
                int intValue;
                if (propertyType == typeof(int) && int.TryParse(valueToken, out intValue))
                {
                    return Expression.GreaterThan(left, Expression.Constant(intValue));
                }

                bool boolValue;
                if (propertyType == typeof(bool) && bool.TryParse(valueToken, out boolValue))
                {
                    return Expression.GreaterThan(left, Expression.Constant(boolValue));
                }
                
                if (propertyType == typeof(DateTime))
                {
                    return Expression.GreaterThan(left, Expression.Constant(ParseDateTime(valueToken)));
                }
                
                if (propertyType == typeof(string))
                {
                    var method = MethodCache["compareto"];
                    var result = Expression.Call(left, method, Expression.Constant(valueToken));
                    var zero = Expression.Constant(0);

                    return Expression.MakeBinary(ExpressionType.GreaterThan, result, zero);
                }

                return Expression.MakeBinary(ExpressionType.GreaterThan, left, Expression.Constant(valueToken));
            }

            // Greater Than or Equal
            if (operatorToken.Equals("ge"))
            {
                int intValue;
                if (propertyType == typeof(int) && int.TryParse(valueToken, out intValue))
                {
                    return Expression.GreaterThanOrEqual(left, Expression.Constant(intValue));
                }

                bool boolValue;
                if (propertyType == typeof(bool) && bool.TryParse(valueToken, out boolValue))
                {
                    return Expression.GreaterThanOrEqual(left, Expression.Constant(boolValue));
                }
                
                if (propertyType == typeof(DateTime))
                {
                    return Expression.GreaterThanOrEqual(left, Expression.Constant(ParseDateTime(valueToken)));
                }
                
                if (propertyType == typeof(string))
                {
                    var method = MethodCache["compareto"];
                    var result = Expression.Call(left, method, Expression.Constant(valueToken));
                    var zero = Expression.Constant(0);

                    return Expression.MakeBinary(ExpressionType.GreaterThanOrEqual, result, zero);
                }

                return Expression.MakeBinary(ExpressionType.GreaterThanOrEqual, left, Expression.Constant(valueToken));
            }

            // Less Than
            if (operatorToken.Equals("lt"))
            {
                int intValue;
                if (propertyType == typeof(int) && int.TryParse(valueToken, out intValue))
                {
                    return Expression.LessThan(left, Expression.Constant(intValue));
                }

                bool boolValue;
                if (propertyType == typeof(bool) && bool.TryParse(valueToken, out boolValue))
                {
                    return Expression.LessThan(left, Expression.Constant(boolValue));
                }
                
                if (propertyType == typeof(DateTime))
                {
                    return Expression.LessThan(left, Expression.Constant(ParseDateTime(valueToken)));
                }

                if (propertyType == typeof(string))
                {
                    var method = MethodCache["compareto"];
                    var result = Expression.Call(left, method, Expression.Constant(valueToken));
                    var zero = Expression.Constant(0);

                    return Expression.MakeBinary(ExpressionType.LessThan, result, zero);
                }

                return Expression.MakeBinary(ExpressionType.LessThan, left, Expression.Constant(valueToken));
            }

            // Less Than or Equal
            if (operatorToken.Equals("le"))
            {
                int intValue;
                if (propertyType == typeof(int) && int.TryParse(valueToken, out intValue))
                {
                    return Expression.LessThanOrEqual(left, Expression.Constant(intValue));
                }

                bool boolValue;
                if (propertyType == typeof(bool) && bool.TryParse(valueToken, out boolValue))
                {
                    return Expression.LessThanOrEqual(left, Expression.Constant(boolValue));
                }
                
                if (propertyType == typeof(DateTime))
                {
                    return Expression.LessThanOrEqual(left, Expression.Constant(ParseDateTime(valueToken)));
                }

                if (propertyType == typeof(string))
                {
                    var method = MethodCache["compareto"];
                    var result = Expression.Call(left, method, Expression.Constant(valueToken));
                    var zero = Expression.Constant(0);

                    return Expression.MakeBinary(ExpressionType.LessThanOrEqual, result, zero);
                }

                return Expression.MakeBinary(ExpressionType.LessThanOrEqual, left, Expression.Constant(valueToken));
            }

            throw new Exception("Invalid filter operator for a binary expression.");
        }

        protected static DateTime ParseDateTime(string valueToken)
        {
            return JToken.Parse("\"" + valueToken + "\"").ToObject<DateTime>(_JsonSerializer);
        }

        protected static PropertyInfo GetPropertyInfoFromCache(Type type, string propertyName)
        {
            IDictionary<string, PropertyInfo> typeProperties = PropertyCache.GetOrAdd(
                type,
                t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                      .ToDictionary(pi => pi.Name, pi => pi, StringComparer.OrdinalIgnoreCase));

            if (!typeProperties.ContainsKey(propertyName))
            {
                throw new Exception("ERROR"); // TODO: (DG) make proper error
            }

            return typeProperties[propertyName];
        }

        protected LambdaExpression MakeLambdaExpression(Type argType, Type returnType, Expression body, ParameterExpression argument)
        {
            Type @delegate = typeof(Func<,>).MakeGenericType(argType, returnType);

            return (LambdaExpression)typeof(Expression)
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .First(m => m.Name == "Lambda" &&
                                m.IsGenericMethod &&
                                m.GetParameters().Select(p => p.ParameterType)
                                    .SequenceEqual(new[]
                                    {
                                        typeof(Expression), typeof(ParameterExpression[])
                                    }))
                    .MakeGenericMethod(@delegate)
                    .Invoke(null, new object[] { body, new[] {argument} });
        }

        protected static string GetSchema(ScimFilterParser.AttrPathContext context)
        {
            if (context.SCHEMA() != null)
            {
                return context.SCHEMA().GetText().TrimEnd(':');
            }

            return null;
        }

        private static IDictionary<string, MethodInfo> CreateMethodCache()
        {
            var methodCache = new Dictionary<string, MethodInfo>();

            methodCache.Add("eq",
                            typeof(string).GetMethod(
                                "Equals",
                                BindingFlags.Public | BindingFlags.Static,
                                null,
                                new[] { typeof(string), typeof(string), typeof(StringComparison) },
                                new ParameterModifier[0]));
            methodCache.Add("compareto",
                            typeof(string).GetMethod("CompareTo", new[] { typeof(string) }));
            methodCache.Add("any",
                            typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                              .Single(mi => mi.Name.Equals("Any") && mi.GetParameters().Length == 2));
            methodCache.Add("sw",
                            typeof(FilterHelpers).GetMethod("StartsWith", BindingFlags.Public | BindingFlags.Static,
                                                            null,
                                                            new[] { typeof(string), typeof(string) },
                                                            new ParameterModifier[0]));
            methodCache.Add("ew",
                            typeof(FilterHelpers).GetMethod("EndsWith", BindingFlags.Public | BindingFlags.Static,
                                                            null,
                                                            new[] { typeof(string), typeof(string) },
                                                            new ParameterModifier[0]));
            methodCache.Add("co",
                            typeof(FilterHelpers).GetMethod("Contains", BindingFlags.Public | BindingFlags.Static,
                                                            null,
                                                            new[] { typeof(string), typeof(string) },
                                                            new ParameterModifier[0]));
            methodCache.Add("pr",
                            typeof (FilterHelpers).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                  .Single(mi => mi.Name.Equals("IsPresent")));

            return methodCache;
        }
    }
}