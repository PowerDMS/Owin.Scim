namespace Owin.Scim.Querying
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Antlr;

    using Antlr4.Runtime.Tree;

    using Extensions;

    using NContext.Common;

    public interface IScimFilterVisitor
    {
        LambdaExpression VisitExpression(IParseTree tree);
    }

    public class ScimFilterVisitor<TResource> : ScimFilterBaseVisitor<LambdaExpression>, IScimFilterVisitor
    {
        public LambdaExpression VisitExpression(IParseTree tree)
        {
            return Visit(tree);
        }

        public override LambdaExpression VisitAndExp(ScimFilterParser.AndExpContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            return CombineWithAnd(left, right);
        }

        public override LambdaExpression VisitBraceExp(ScimFilterParser.BraceExpContext context)
        {
            return Visit(context.expression());
        }

        public override LambdaExpression VisitBracketExp(ScimFilterParser.BracketExpContext context)
        {
            // brackets change the TResource (field) and the expression within should be visited in context of the new field's type

            var propertyNameToken = context.FIELD().GetText();
            var property = typeof(TResource)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField)
                .SingleOrDefault(pi => pi.Name.Equals(propertyNameToken, StringComparison.OrdinalIgnoreCase));

            if (property == null) throw new Exception("ERROR"); // TODO: (DG) make proper error

            if (property.PropertyType != typeof (TResource))
            {
                Type childFilterType = property.PropertyType;
                if (childFilterType.IsNonStringEnumerable())
                {
                    childFilterType = childFilterType.GetGenericArguments()[0];
                }

                var childVisitorType = typeof (ScimFilterVisitor<>).MakeGenericType(childFilterType);
                var childVisitor = (IScimFilterVisitor) childVisitorType.CreateInstance();
                
                return childVisitor.VisitExpression(context.expression());
            }

            return Visit(context.expression());
        }

        public override LambdaExpression VisitNotExp(ScimFilterParser.NotExpContext context)
        {
            var predicate = Visit(context.expression());

            return CombineWithNot(predicate);
        }

        public override LambdaExpression VisitOperatorExp(ScimFilterParser.OperatorExpContext context)
        {
            var propertyNameToken = context.FIELD().GetText();
            var operatorToken = context.OPERATOR().GetText().ToLower();
            var valueToken = context.VALUE().GetText().Trim('"');

            var property = typeof(TResource)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField)
                .SingleOrDefault(pi => pi.Name.Equals(propertyNameToken, StringComparison.OrdinalIgnoreCase));

            if (property == null) throw new Exception("ERROR"); // TODO: (DG) make proper error


            var argument = Expression.Parameter(typeof(TResource));
            var predicate = Expression.Lambda<Func<TResource, bool>>(
                GetBinaryExpression(operatorToken, argument, property, valueToken),
                argument);

            return predicate;
        }

        private static Expression GetBinaryExpression(string operatorToken, ParameterExpression argument, PropertyInfo property, string valueToken)
        {
            var left = Expression.Property(argument, property);

            // Equal
            if (operatorToken.Equals("eq"))
            {
                int intValue;
                if (property.PropertyType == typeof(int) && int.TryParse(valueToken, out intValue))
                {
                    return Expression.Equal(left, Expression.Constant(intValue));
                }
                
                bool boolValue;
                if (property.PropertyType == typeof(bool) && bool.TryParse(valueToken, out boolValue))
                {
                    return Expression.Equal(left, Expression.Constant(boolValue));
                }

                DateTime dateTimeValue;
                if (property.PropertyType == typeof(DateTime) && DateTime.TryParse(valueToken, out dateTimeValue))
                {
                    return Expression.Equal(left, Expression.Constant(dateTimeValue));
                }

                return Expression.Call(
                        typeof(string).GetMethod("Equals", BindingFlags.Public | BindingFlags.Static, null,
                            new[] { typeof(string), typeof(string), typeof(StringComparison) },
                            new ParameterModifier[0]),
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
                if (valueToken.StartsWith("\""))
                {
                    return Expression.IsFalse(
                        Expression.Call(
                            typeof (string).GetMethod("Equals", BindingFlags.Public | BindingFlags.Static, null,
                                new[] { typeof (string), typeof (string), typeof (StringComparison) },
                                new ParameterModifier[0]),
                            new List<Expression>
                            {
                                left,
                                Expression.Constant(valueToken.Trim('"')),
                                Expression.Constant(StringComparison.OrdinalIgnoreCase)
                            }));
                }

                int intValue;
                if (property.PropertyType == typeof(int) && int.TryParse(valueToken, out intValue))
                {
                    return Expression.NotEqual(left, Expression.Constant(intValue));
                }

                bool boolValue;
                if (property.PropertyType == typeof(bool) && bool.TryParse(valueToken, out boolValue))
                {
                    return Expression.NotEqual(left, Expression.Constant(boolValue));
                }

                DateTime dateTimeValue;
                if (property.PropertyType == typeof(DateTime) && DateTime.TryParse(valueToken, out dateTimeValue))
                {
                    return Expression.NotEqual(left, Expression.Constant(dateTimeValue));
                }

                return Expression.NotEqual(left, Expression.Constant(valueToken));
            }

            // Contains
            if (operatorToken.Equals("co"))
            {
            }

            // Starts With
            if (operatorToken.Equals("sw"))
            {
            }

            // Ends With
            if (operatorToken.Equals("ew"))
            {
            }

            // Greater Than
            if (operatorToken.Equals("gt"))
            {
            }

            // Greater Than or Equal
            if (operatorToken.Equals("ge"))
            {
            }

            // Less Than
            if (operatorToken.Equals("lt"))
            {
            }

            // Less Than or Equal
            if (operatorToken.Equals("le"))
            {
            }

            throw new Exception("Invalid filter operator for a binary expression.");
        }

        public override LambdaExpression VisitOrExp(ScimFilterParser.OrExpContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            return CombineWithOr(left, right);
        }

        public override LambdaExpression VisitPresentExp(ScimFilterParser.PresentExpContext context)
        {
            var propertyNameToken = context.FIELD().GetText();

            var property = typeof(TResource)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .SingleOrDefault(pi => pi.Name.Equals(propertyNameToken, StringComparison.OrdinalIgnoreCase));

            if (property == null) throw new Exception("eeeerrrooorrr"); // TODO: (DG) proper error handling
            if (property.GetGetMethod() == null) throw new Exception("error");
            
            var argument = Expression.Parameter(typeof(TResource));
            var predicate = Expression.Lambda<Func<TResource, bool>>(
                Expression.Call(
                    GetType().GetMethod("IsPresent", BindingFlags.NonPublic | BindingFlags.Static, 
                            null,
                            new[] { typeof(TResource), typeof(PropertyInfo) },
                            new ParameterModifier[0]),
                        new List<Expression>
                        {
                            argument,
                            Expression.Constant(property)
                        }),
                argument);

            return predicate;
        }

        private LambdaExpression CombineWithOr(LambdaExpression left, LambdaExpression right)
        {
            var parameter = Expression.Parameter(typeof(TResource), "x");
            var resultBody = Expression.Or(Expression.Invoke(left, parameter), Expression.Invoke(right, parameter));
            return Expression.Lambda<Func<TResource, bool>>(resultBody, parameter);
        }

        private LambdaExpression CombineWithAnd(LambdaExpression left, LambdaExpression right)
        {
            var parameter = Expression.Parameter(typeof(TResource), "x");
            var resultBody = Expression.And(Expression.Invoke(left, parameter), Expression.Invoke(right, parameter));
            return Expression.Lambda<Func<TResource, bool>>(resultBody, parameter);
        }

        private LambdaExpression CombineWithNot(LambdaExpression predicate)
        {
            var parameter = Expression.Parameter(typeof(TResource), "x");
            var resultBody = Expression.Not(Expression.Invoke(predicate, parameter));
            return Expression.Lambda<Func<TResource, bool>>(resultBody, parameter);
        }

        internal static bool IsPresent(TResource resource, PropertyInfo property)
        {
            if (resource == null || property == null) return false;

            var value = property.GetValue(resource);
            if (value == null) return false;

            var valueType = value.GetType();
            if (valueType == typeof (String))
            {
                return !string.IsNullOrWhiteSpace(value as string);
            }

            if (valueType.IsNonStringEnumerable())
            {
                var enumerable = (IEnumerable) value;
                var enumerator = enumerable.GetEnumerator();
                return enumerator.MoveNext();
            }

            return true;
        }
    }
}