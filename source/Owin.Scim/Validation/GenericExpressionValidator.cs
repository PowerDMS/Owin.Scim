namespace Owin.Scim.Validation
{
    using System;
    using System.Linq.Expressions;

    using FluentValidation;

    public class GenericExpressionValidator<T> : AbstractValidator<T>
    {
        public void Add<TProperty>(
            Expression<Func<T, TProperty>> ruleForExpression,
            Action<IRuleBuilderInitial<T, TProperty>> ruleBuilder)
        {
            ruleBuilder(RuleFor(ruleForExpression));
        }

        public void Add(Action<AbstractValidator<T>> validatorConfiguration)
        {
            validatorConfiguration(this);
        }
    }
}