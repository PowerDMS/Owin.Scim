namespace Owin.Scim.Dependencies
{
    using System;

    public class FuncDependencyResolver : IDependencyResolver
    {
        private readonly Func<Type, object> _ResolveDependencyFunc;

        public FuncDependencyResolver(Func<Type, object> resolveDependencyFunc)
        {
            _ResolveDependencyFunc = resolveDependencyFunc;
        }

        public object Resolve(Type serviceType)
        {
            try
            {
                return _ResolveDependencyFunc(serviceType);
            }
            catch
            {
                return null;
            }
        }
    }
}