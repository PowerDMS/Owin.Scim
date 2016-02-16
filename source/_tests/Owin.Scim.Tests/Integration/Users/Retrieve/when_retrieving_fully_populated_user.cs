namespace Owin.Scim.Tests.Integration.Users.Retrieve
{
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Model.Users;

    using Ploeh.AutoFixture;

    public class when_retrieving_fully_populated_user : using_a_scim_server
    {
        Because of = () =>
        {
            var autoFixture = new Fixture();

            var existingUser = autoFixture.Build<EnterpriseUser>()
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
                .Create();

            // Insert the first user so there's one already in-memory.
            Response = Server
                .HttpClient
                .PostAsync("users", new ObjectContent<EnterpriseUser>(existingUser, new ScimJsonMediaTypeFormatter()))
                .Result;

            if (Response.StatusCode == HttpStatusCode.Created)
            {
                JsonData = Response.Content.ReadAsStringAsync().Result;
                User = Newtonsoft.Json.JsonConvert.DeserializeObject<EnterpriseUser>(JsonData);
            }

            var client = Server.HttpClient;
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/scim+json"));

            Response = Server
                .HttpClient
                .GetAsync("users/" + User.Id)
                .Result;
        };

        private It should_return_same_data_echoed_during_create = () =>
        {
            Response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            NewJsonData = Response.Content.ReadAsStringAsync().Result;

            JsonData.ShouldEqual(NewJsonData);
        };

        protected static HttpResponseMessage Response;

        protected static User User;

        protected static string JsonData;

        protected static string NewJsonData;
    }
}