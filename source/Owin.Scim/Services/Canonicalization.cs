namespace Owin.Scim.Services
{
    using System;
    using System.Collections;
    using System.Linq.Expressions;
    using System.Reflection;

    using Model;

    public class Canonicalization
    {
        public static void Lowercase<T, TProperty>(T attribute, Expression<Func<T, TProperty>> expression)
            where T : MultiValuedAttribute
            where TProperty : class, IComparable, ICloneable, IConvertible, IEnumerable
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

        public static void EnforceMutabilityRules(MultiValuedAttribute attribute)
        {
            attribute.Display = null; // Immutable, readOnly, returns the canonical value
        }
    }
}