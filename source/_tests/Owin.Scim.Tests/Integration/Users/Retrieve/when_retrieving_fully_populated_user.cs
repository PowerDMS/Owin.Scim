namespace Owin.Scim.Tests.Integration.Users.Retrieve
{
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Model.Users;

    using Ploeh.AutoFixture;

    using v2.Model;

    public class when_retrieving_fully_populated_user : using_a_scim_server
    {
        Because of = async () =>
        {
            var autoFixture = new Fixture();

            var existingUser = autoFixture.Build<ScimUser2>()
                .With(x => x.UserName, UserNameUtility.GenerateUserName())
                .With(x => x.Password, "somePass!1")
                .With(x => x.PreferredLanguage, "en-US,en,es")
                .With(x => x.Locale, "en-US")
                .With(x => x.Timezone, @"US/Eastern")
                .With(x => x.Emails, null)
                .With(x => x.PhoneNumbers, null)
                .With(x => x.Ims, null)
                .With(x => x.Photos, null)
                .With(x => x.Addresses, null)
                .With(x => x.X509Certificates, null)
                .Create(seed: new ScimUser2());

            // Insert the first user so there's one already in-memory.
            Response = await Server
                .HttpClient
                .PostAsync("v2/users", new ScimObjectContent<ScimUser>(existingUser))
                .AwaitResponse()
                .AsTask;

            if (Response.StatusCode == HttpStatusCode.Created)
            {
                CreatedUser = await Response.Content.ScimReadAsAsync<ScimUser2>().Await().AsTask;
            }

            var client = Server.HttpClient;
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/scim+json"));

            Response = await Server
                .HttpClient
                .GetAsync("v2/users/" + CreatedUser.Id)
                .AwaitResponse()
                .AsTask;
        };

        It should_return_same_data_echoed_during_create = 
            async () =>
        {
            Response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            RetrievedUser = await Response.Content.ScimReadAsAsync<ScimUser2>().AwaitResponse().AsTask;

            RetrievedUser.Meta.Version.ShouldEqual(CreatedUser.Meta.Version);
        };

        protected static HttpResponseMessage Response;

        protected static ScimUser CreatedUser;
        
        protected static ScimUser RetrievedUser;
    }
}