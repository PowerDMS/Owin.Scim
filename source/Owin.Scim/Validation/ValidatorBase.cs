namespace Owin.Scim.Validation
{
    using System.Threading.Tasks;
    using System.Transactions;

    using FluentValidation;
    
    public abstract class ValidatorBase<TEntity> : AbstractValidator<TEntity>
    {
        public override async Task<FluentValidation.Results.ValidationResult> ValidateAsync(ValidationContext<TEntity> context)
        {
            if (Transaction.Current != null)
            {
                using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
                {
                    return await base.ValidateAsync(context);
                }
            }

            return await base.ValidateAsync(context);
        }
    }
}