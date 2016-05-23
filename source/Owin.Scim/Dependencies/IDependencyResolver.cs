namespace Owin.Scim.Dependencies
{
    using System;

    /// <summary>
    /// Defines an abstraction for resolving external dependencies within Owin.Scim.
    /// </summary>
    public interface IDependencyResolver
    {
        /// <summary>
        /// Resolves the specified <paramref name="serviceType"/>, typically 
        /// from a dependency injection container.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>System.Object.</returns>
        object Resolve(Type serviceType);
    }
}