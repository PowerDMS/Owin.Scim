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

            return ruleBuilder.MustAsync(
                async (val, token) => 
                await Task.FromResult(comparer.Equals(val, toCompare())));
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

            return ruleBuilder.MustAsync(
                async (entity, val, token) => 
                await Task.FromResult(comparer.Equals(val, toCompare(entity))));
        }

        public static IRuleBuilderOptions<T, TProperty> ImmutableAsync<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder,
            Func<Task<TProperty>> toCompare,
            IEqualityComparer comparer = null)
        {
            if (comparer == null)
            {
                comparer = EqualityComparer<TProperty>.Default;
            }

            return ruleBuilder.MustAsync(
                async (val, token) =>
                {
                    var compareValue = await toCompare();
                    return await Task.FromResult(comparer.Equals(val, compareValue));
                });
        }

        public static IRuleBuilderOptions<T, TProperty> ImmutableAsync<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder,
            Func<T, Task<TProperty>> toCompare,
            IEqualityComparer comparer = null)
        {
            if (comparer == null)
            {
                comparer = EqualityComparer<TProperty>.Default;
            }

            return ruleBuilder.MustAsync(
                async (entity, val, token) =>
                {
                    var compareValue = await toCompare(entity);
                    return await Task.FromResult(comparer.Equals(val, compareValue));
                });
        }
    }
}