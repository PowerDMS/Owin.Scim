namespace ConsoleHost
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading.Tasks;

    using Nito.AsyncEx;

    using Owin.Scim.Model.Users;

    class Program
    {
        static void Main(string[] args)
        {
            using (Microsoft.Owin.Hosting.WebApp.Start<CompositionRoot>("http://localhost:8080"))
            {
                AsyncContext.Run(TestScimApi);
                Console.ReadLine();
            }
        }

        private static async Task TestScimApi()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:8080/scim/")
            };

            Write("");
            Write("Creating user...");
            var response = await client.PostAsync("users", new ObjectContent<User>(new User { UserName = "daniel" }, new JsonMediaTypeFormatter()));
            var user = await response.Content.ReadAsAsync<User>(new[] { new JsonMediaTypeFormatter() });
            Write(await response.Content.ReadAsStringAsync());
            Write("");


            Write("Getting user " + user.Id);
            var users = await client.GetStringAsync("users/" + user.Id);
            Write(users);
            Write("");

        }

        private static void Write(string text)
        {
            Console.WriteLine(text);
        }
    }
}
