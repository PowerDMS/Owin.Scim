namespace ConsoleHost
{
    using System;
    using System.Collections.Generic;

    using Autofac;

    using Kernel;

    using Newtonsoft.Json;

    using Owin;
    using Owin.Scim.Dependencies;
    using Owin.Scim.Extensions;

    internal class CompositionRoot
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            // Kernel application context ... typically done in the composite root of your main application.
            var autofacBuilder = new ContainerBuilder();
            autofacBuilder.RegisterType<KernelUserRepository>().AsSelf();
            autofacBuilder.RegisterType<KernelUserManager>().AsSelf();
            var autofacContainer = autofacBuilder.Build();

            // Scim application context.
            // This example also illustrates the use of DependencyResolution and architecturally-speaking,
            // the autofacContainer above should be injected into this CompositionRoot.Configuration method since it is created / owned by a different context.
            appBuilder.Map("/scim", app =>
            {
                app.UseScimServer(
                    configuration => {
                        configuration.RequireSsl = false;
                        configuration.EnableEndpointAuthorization = false;
                        configuration.HttpConfiguration.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented; // for example purposes
                        configuration.DependencyResolver = new FuncDependencyResolver(type => autofacContainer.Resolve(type));        // enables external dependency resolution in SCIM services
                });
            });

            var address = ((IList<IDictionary<string, object>>)appBuilder.Properties["host.Addresses"])[0];
            Console.WriteLine(string.Format("SCIM server is listening at: {0}", new UriBuilder(address["scheme"].ToString(), address["host"].ToString(), Convert.ToInt32(address["port"]), address["path"].ToString() + "/scim")));
        }
    }
}