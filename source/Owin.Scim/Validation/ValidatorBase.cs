namespace Owin.Scim.Validation
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Transactions;

    using FluentValidation;
    
    public abstract class ValidatorBase<TEntity> : AbstractValidator<TEntity>
    {
        public override Task<FluentValidation.Results.ValidationResult> ValidateAsync(
            ValidationContext<TEntity> context, 
            CancellationToken token = new CancellationToken())
        {
            if (Transaction.Current != null)
            {
                using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
                {
                    return base.ValidateAsync(context, token);
                }
            }

            return base.ValidateAsync(context, token);
        }
    }
}