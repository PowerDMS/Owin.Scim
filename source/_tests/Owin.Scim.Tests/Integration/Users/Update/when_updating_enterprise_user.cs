namespace Owin.Scim.Tests.Integration.Users.Update
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using Machine.Specifications;

    using Model.Users;

    public class when_updating_enterprise_user : using_a_scim_server
    {
        Because of = () =>
        {
            // Insert the first user so there's one already in-memory.
            var userRecord = Server
                .HttpClient
                .PostAsync("users", new ObjectContent<User>(UserToUpdate, new JsonMediaTypeFormatter()))
                .Result;

            _UserId = userRecord.Content.ReadAsAsync<User>().Result.Id;

            PatchResponse = Server
                .HttpClient
                .SendAsync(
                    new HttpRequestMessage(
                        new HttpMethod("PATCH"), "users/" + _UserId)
                    {
                        Content = PatchContent
                    })
                .Result;

            if (PatchResponse.StatusCode == HttpStatusCode.OK)
                UpdatedUser = PatchResponse.Content.ReadAsAsync<User>().Result;

            if (PatchResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorText = PatchResponse.Content.ReadAsStringAsync();
            }
        };

        protected static User UserToUpdate;

        protected static HttpContent PatchContent;

        protected static HttpResponseMessage PatchResponse;

        protected static User UpdatedUser;

        private static string _UserId;
    }
}