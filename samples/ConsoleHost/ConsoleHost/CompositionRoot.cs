namespace ConsoleHost
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Owin;
    using Owin.Scim.Extensions;

    internal class CompositionRoot
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            appBuilder.Map("/scim", app =>
            {
                app.UseScimServer(
                    new Predicate<FileInfo>[] {
                        fileInfo => fileInfo.Name.Equals("ConsoleHost.exe", StringComparison.OrdinalIgnoreCase)
                    },
                    configuration => { configuration.RequireSsl = false; });
            });

            var address = ((IList<IDictionary<string, object>>)appBuilder.Properties["host.Addresses"])[0];
            Console.WriteLine(string.Format("SCIM server is listening at: {0}", new UriBuilder(address["scheme"].ToString(), address["host"].ToString(), Convert.ToInt32(address["port"]), address["path"].ToString() + "/scim")));
        }
    }
}