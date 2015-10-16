namespace Owin.Scim.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FluentValidation;

    public static class RuleBuilderExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> Immutable<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder,
            Func<TProperty> toCompare,
            IEqualityComparer comparer = null)
        {
            if (comparer == null)
            {
                comparer = EqualityComparer<TProperty>.Default;
            }

            return ruleBuilder.MustAsync(async (val) => await Task.FromResult(comparer.Equals(val, toCompare())));
        }

        public static IRuleBuilderOptions<T, TProperty> Immutable<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder,
            Func<T, TProperty> toCompare,
            IEqualityComparer comparer = null)
        {
            if (comparer == null)
            {
                comparer = EqualityComparer<TProperty>.Default;
            }

            return ruleBuilder.MustAsync(async (entity, val) => await Task.FromResult(comparer.Equals(val, toCompare(entity))));
        }
    }
}