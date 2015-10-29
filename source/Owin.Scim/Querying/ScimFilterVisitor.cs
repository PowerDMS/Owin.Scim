namespace Owin.Scim.Querying
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Antlr;

    public class ScimFilterVisitor<TResource> : ScimFilterBaseVisitor<Expression<Func<TResource, bool>>>
    {
        public override Expression<Func<TResource, bool>> VisitAndExp(ScimFilterParser.AndExpContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            return CombineWithAnd(left, right);
        }

        public override Expression<Func<TResource, bool>> VisitBraceExp(ScimFilterParser.BraceExpContext context)
        {
            return Visit(context.expression());
        }

        public override Expression<Func<TResource, bool>> VisitBracketExp(ScimFilterParser.BracketExpContext context)
        {
            return base.VisitBracketExp(context);
        }

        public override Expression<Func<TResource, bool>> VisitNotExp(ScimFilterParser.NotExpContext context)
        {
            var predicate = Visit(context.expression());

            return CombineWithNot(predicate);
        }

        public override Expression<Func<TResource, bool>> VisitOperatorExp(ScimFilterParser.OperatorExpContext context)
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

        public override Expression<Func<TResource, bool>> VisitOrExp(ScimFilterParser.OrExpContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            return CombineWithOr(left, right);
        }

        public override Expression<Func<TResource, bool>> VisitPresentExp(ScimFilterParser.PresentExpContext context)
        {
            var propertyNameToken = context.FIELD().GetText();

            var property = typeof(TResource)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .SingleOrDefault(pi => pi.Name.Equals(propertyNameToken, StringComparison.OrdinalIgnoreCase));

            if (property == null) throw new Exception("eeeerrrooorrr"); // TODO: (DG) proper error handling
            if (property.GetGetMethod() == null) throw new Exception("error");

            //            var propertyValue = property.GetValue()

            //            var argument = Expression.Parameter(typeof(TResource));
            //            var predicate = Expression.Lambda<Func<TResource, bool>>(
            //                Expression.Call(),
            //                argument);

            return null;
        }

        private Expression<Func<TResource, bool>> CombineWithOr(Expression<Func<TResource, bool>> left, Expression<Func<TResource, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(TResource), "x");
            var resultBody = Expression.Or(Expression.Invoke(left, parameter), Expression.Invoke(right, parameter));
            return Expression.Lambda<Func<TResource, bool>>(resultBody, parameter);
        }

        private Expression<Func<TResource, bool>> CombineWithAnd(Expression<Func<TResource, bool>> left, Expression<Func<TResource, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(TResource), "x");
            var resultBody = Expression.And(Expression.Invoke(left, parameter), Expression.Invoke(right, parameter));
            return Expression.Lambda<Func<TResource, bool>>(resultBody, parameter);
        }

        private Expression<Func<TResource, bool>> CombineWithNot(Expression<Func<TResource, bool>> predicate)
        {
            var parameter = Expression.Parameter(typeof(TResource), "x");
            var resultBody = Expression.Not(Expression.Invoke(predicate, parameter));
            return Expression.Lambda<Func<TResource, bool>>(resultBody, parameter);
        }
    }
}