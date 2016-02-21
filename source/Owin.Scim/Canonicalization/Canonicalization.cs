namespace Owin.Scim.Canonicalization
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;

    using Extensions;

    using Model;
    
    public static class Canonicalization
    {
        public static void Lowercase<T>(T attribute, Expression<Func<T, string>> expression)
        {
            var mE = expression.Body as MemberExpression;
            if (mE == null) throw new InvalidOperationException("Expression body must be a MemberExpression to an attribute's string property.");

            var pI = mE.Member as PropertyInfo;
            if (pI == null) throw new InvalidOperationException("Expression body member must be an attribute's string property.");

            var value = pI.GetValue(attribute) as string;

            if (string.IsNullOrWhiteSpace(value)) return;

            pI.SetValue(attribute, value.ToLower());
        }

        public static void EnforceSinglePrimaryAttribute(MultiValuedAttribute attribute, ref object state)
        {
            bool hasPrimary = false;
            if (state != null)
                hasPrimary = (bool)state;

            if (!hasPrimary && attribute.Primary)
            {
                state = true;
            }

            if (hasPrimary && attribute.Primary)
            {
                attribute.Primary = false;
            }
        }

        public static TProperty Canonicalize<TProperty>(
            this TProperty property,
            StatefulCanonicalizationFunc<TProperty> canonicalizationFunc)
        {
            return default(TProperty);// TODO : (DG) remove or impl this
        }

        public static void Canonicalize<T, TProperty>(
            this T source,
            Expression<Func<T, TProperty>> property,
            Func<TProperty, TProperty> canonicalizationFunc)
        {
            source.Canonicalize(property, property, canonicalizationFunc);
        }

        public static void Canonicalize<T, TProperty, TOtherProperty>(
            this T source,
            Expression<Func<T, TProperty>> sourceProperty,
            Expression<Func<T, TOtherProperty>> targetProperty,
            Func<TProperty, TOtherProperty> canonicalizationFunc)
            where TProperty : TOtherProperty
        {
            if (source == null) return;
            if (sourceProperty == null) throw new ArgumentNullException("sourceProperty");
            if (targetProperty == null) throw new ArgumentNullException("targetProperty");
            if (canonicalizationFunc == null) throw new ArgumentNullException("canonicalizationFunc");

            var descriptors = GetDescriptors(sourceProperty, targetProperty);
            var sourceValue = descriptors.Item1.GetValue(source);

            // If sourceValue is null or default for type, just set target to default
            if (sourceValue == descriptors.Item1.PropertyType.GetDefaultValue())
            {
                descriptors.Item2.SetValue(source, sourceValue);
                return;
            }

            var canonicalizedValue = canonicalizationFunc((TProperty)sourceValue);
            descriptors.Item2.SetValue(source, canonicalizedValue);
        }

        private static Tuple<PropertyDescriptor, PropertyDescriptor> GetDescriptors<T, TProperty, TOtherProperty>(
            Expression<Func<T, TProperty>> sourceProperty,
            Expression<Func<T, TOtherProperty>> targetProperty)
        {
            var memberExpression = sourceProperty.Body as MemberExpression;
            if (memberExpression == null)
                throw new InvalidOperationException("sourceProperty must be of type MemberExpression.");

            var sourceDescriptor = TypeDescriptor.GetProperties(typeof(T)).Find(memberExpression.Member.Name, true);

            memberExpression = targetProperty.Body as MemberExpression;
            if (memberExpression == null)
                throw new InvalidOperationException("targetProperty must be of type MemberExpression.");

            var targetDescriptor = TypeDescriptor.GetProperties(typeof(T)).Find(memberExpression.Member.Name, true);

            return Tuple.Create(sourceDescriptor, targetDescriptor);
        }
    }
}