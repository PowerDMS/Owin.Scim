namespace Owin.Scim.Tests.Integration
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Web.Http.Dispatcher;

    using Endpoints.Users;

    public class TestAssembliesResolver : DefaultAssembliesResolver
    {
        public override ICollection<Assembly> GetAssemblies()
        {
            ICollection<Assembly> baseAssemblies = base.GetAssemblies();
            List<Assembly> assemblies = new List<Assembly>(baseAssemblies);
            var controllersAssembly = Assembly.Load(Assembly.GetAssembly(typeof(UsersController)).GetName());
            baseAssemblies.Add(controllersAssembly);
            return assemblies;
        }
    }
}