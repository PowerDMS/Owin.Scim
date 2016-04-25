namespace Owin.Scim.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FluentValidation;
    using FluentValidation.Validators;

    public static class RuleBuilderExtensions
    {
        public static void NestedRules<T, TCollectionElement>(
            this IRuleBuilder<T, IEnumerable<TCollectionElement>> ruleBuilder,
            Action<InlineValidator<TCollectionElement>> setup)
        {
            var inlineValidator = new InlineValidator<TCollectionElement>();
            setup(inlineValidator);
            var adaptor = new ChildCollectionValidatorAdaptor(inlineValidator);
            ruleBuilder.SetValidator(adaptor);
        }

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
        
        /// <summary>
        /// Associates a validator provider with the current property rule. 
        /// </summary>
        /// <param name="ruleBuilder"></param>
        /// <param name="validatorProvider">The validator provider to use</param>
        public static IRuleBuilderOptions<T, TProperty> SetValidatorNonGeneric<T, TProperty, TValidator>(
            this IRuleBuilderInitial<T, TProperty> ruleBuilder,
            Func<T, TValidator> validatorProvider)
            where TValidator : IValidator
        {
            if (validatorProvider == null)
                throw new ArgumentNullException("validatorProvider", "Cannot pass a null validatorProvider to SetValidator");

            ruleBuilder.SetValidator(new ChildValidatorAdaptor(t => validatorProvider((T)t) as IValidator, typeof(TProperty)));

            return (IRuleBuilderOptions<T, TProperty>)ruleBuilder;
        }
    }
}