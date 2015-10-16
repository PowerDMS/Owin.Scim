namespace Owin.Scim.Validation
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines an interface for <typeparamref name="TEntity"/> validation.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IValidator<in TEntity>
    {
        /// <summary>
        /// Validates the specified entity. Optionally, you may provide a comma-delimited <paramref name="ruleSet"/>.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="ruleSet">The rule set.</param>
        /// <returns>Task</returns>
        Task<ValidationResult> ValidateAsync(TEntity entity, string ruleSet = RuleSetConstants.Default);
    }
}