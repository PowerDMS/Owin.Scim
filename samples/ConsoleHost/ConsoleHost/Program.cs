namespace ConsoleHost
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Owin;
    using Owin.Scim.Configuration;
    using Owin.Scim.Extensions;

    class Program
    {
        static void Main(string[] args)
        {
            using (Microsoft.Owin.Hosting.WebApp.Start<CompositionRoot>("http://localhost:8080"))
            {
                Console.ReadLine();
            }
        }
    }

    internal class CompositionRoot
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            appBuilder.Map("/scim", app =>
            {
                app.UseScimServer(
                    new ScimServerConfiguration { RequireSsl = false }
                        .AddCompositionConditions(
                            fileInfo => fileInfo.Name.StartsWith("ConsoleHost", StringComparison.OrdinalIgnoreCase) && 
                            fileInfo.Extension.Equals(".exe", StringComparison.OrdinalIgnoreCase)));
            });

            var address = ((IList<IDictionary<string, object>>)appBuilder.Properties["host.Addresses"])[0];
            Console.WriteLine(string.Format("SCIM server is listening at: {0}", new UriBuilder(address["scheme"].ToString(), address["host"].ToString(), Convert.ToInt32(address["port"]), address["path"].ToString() + "/scim")));
        }
    }
}
