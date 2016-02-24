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
                .PostAsync("users", new ObjectContent<EnterpriseUser>(UserToUpdate, new JsonMediaTypeFormatter()))
                .Result;

            _UserId = userRecord.Content.ReadAsAsync<EnterpriseUser>().Result.Id;

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
                UpdatedUser = PatchResponse.Content.ReadAsAsync<EnterpriseUser>().Result;

            if (PatchResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorText = PatchResponse.Content.ReadAsStringAsync();
            }
        };

        protected static EnterpriseUser UserToUpdate;

        protected static HttpContent PatchContent;

        protected static HttpResponseMessage PatchResponse;

        protected static EnterpriseUser UpdatedUser;

        private static string _UserId;
    }
}