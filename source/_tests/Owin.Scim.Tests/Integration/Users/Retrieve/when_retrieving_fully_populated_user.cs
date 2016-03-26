namespace Owin.Scim.Tests.Integration.Users.Retrieve
{
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Model.Users;

    using Ploeh.AutoFixture;

    public class when_retrieving_fully_populated_user : using_a_scim_server
    {
        Because of = async () =>
        {
            var autoFixture = new Fixture();

            var existingUser = autoFixture.Build<User>()
                .With(x => x.UserName, UserNameUtility.GenerateUserName())
                .With(x => x.Password, "somePass")
                .With(x => x.PreferredLanguage, "en-US,en,es")
                .With(x => x.Locale, "en-US")
                .With(x => x.Timezone, @"US/Eastern")
                .With(x => x.Emails, null)
                .With(x => x.PhoneNumbers, null)
                .With(x => x.Ims, null)
                .With(x => x.Photos, null)
                .With(x => x.Addresses, null)
                .Create(seed: new User(typeof(EnterpriseUserExtension)));

            // Insert the first user so there's one already in-memory.
            Response = await Server
                .HttpClient
                .PostAsync("users", new ObjectContent<User>(existingUser, new ScimJsonMediaTypeFormatter()))
                .AwaitResponse()
                .AsTask;

            if (Response.StatusCode == HttpStatusCode.Created)
            {
                CreatedUser = await Response.Content.ReadAsAsync<User>(ScimJsonMediaTypeFormatter.AsArray()).Await().AsTask;
            }

            var client = Server.HttpClient;
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/scim+json"));

            Response = await Server
                .HttpClient
                .GetAsync("users/" + CreatedUser.Id)
                .AwaitResponse()
                .AsTask;
        };

        It should_return_same_data_echoed_during_create = 
            async () =>
        {
            Response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            RetrievedUser = await Response.Content.ReadAsAsync<User>(ScimJsonMediaTypeFormatter.AsArray()).AwaitResponse().AsTask;

            RetrievedUser.Meta.Version.ShouldEqual(CreatedUser.Meta.Version);
        };

        protected static HttpResponseMessage Response;

        protected static User CreatedUser;
        
        protected static User RetrievedUser;
    }
}