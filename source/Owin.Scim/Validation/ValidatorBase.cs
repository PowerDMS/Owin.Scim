namespace Owin.Scim.Validation
{
    using System.Threading.Tasks;
    using System.Transactions;

    /// <summary>
    /// Defines an abstraction for validating entities.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ValidatorBase<TEntity> : IValidator<TEntity>
    {
        public async Task<ValidationResult> ValidateAsync(TEntity entity, string ruleSet = RuleSetConstants.Default)
        {
            if (Transaction.Current != null)
            {
                using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
                {
                    return await ValidateAsyncInternal(entity, ruleSet);
                }
            }

            return await ValidateAsyncInternal(entity, ruleSet);
        }

        protected abstract Task<ValidationResult> ValidateAsyncInternal(TEntity entity, string ruleSet = RuleSetConstants.Default);
    }
}