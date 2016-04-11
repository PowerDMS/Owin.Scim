namespace ConsoleHost
{
    using System;

    using Owin;
//    using Owin.Scim.Configuration;
//    using Owin.Scim.Extensions;

    class Program
    {
        static void Main(string[] args)
        {
            using (Microsoft.Owin.Hosting.WebApp.Start<CompositionRoot>("http://localhost:8080"))
            {
                Console.WriteLine("Press [enter] to quit...");
                Console.ReadLine();
            }
        }
    }

    internal class CompositionRoot
    {
        public void Configuration(IAppBuilder appBuilder)
        {
//            appBuilder.Map("/scim", app =>
//            {
//                app.UseScimServer(
//                    new ScimServerConfiguration { RequireSsl = true }
//                        .AddCompositionConditions(
//                            fileInfo => fileInfo.Name.StartsWith("ConsoleHost", StringComparison.OrdinalIgnoreCase) && 
//                            fileInfo.Extension.Equals(".exe", StringComparison.OrdinalIgnoreCase)));
//            });
        }
    }
}
