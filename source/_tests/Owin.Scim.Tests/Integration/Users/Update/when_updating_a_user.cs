namespace Owin.Scim.Tests.Integration.Users.Update
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using Machine.Specifications;

    using Model.Users;

    public class when_updating_a_user : using_a_scim_server
    {
        Because of = async () =>
        {
            // Insert the first user so there's one already in-memory.
            var userRecord = await Server
                .HttpClient
                .PostAsync("users", new ObjectContent<User>(UserToUpdate, new JsonMediaTypeFormatter()))
                .AwaitResponse()
                .AsTask;

            UserToUpdate = userRecord.Content.ReadAsAsync<User>().Result;
            _UserId = UserToUpdate.Id;

            PatchResponse = await Server
                .HttpClient
                .SendAsync(
                    new HttpRequestMessage(
                        new HttpMethod("PATCH"), "users/" + _UserId)
                    {
                        Content = PatchContent
                    })
                .AwaitResponse()
                .AsTask;

            if (PatchResponse.StatusCode == HttpStatusCode.OK)
                UpdatedUser = await PatchResponse.Content.ReadAsAsync<User>();

            if (PatchResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorText = PatchResponse.Content.ReadAsStringAsync().Result;
                Error = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.ScimError>(errorText);
            }
        };

        protected static User UserToUpdate;

        protected static HttpContent PatchContent;

        protected static HttpResponseMessage PatchResponse;

        protected static User UpdatedUser;

        protected static Model.ScimError Error;

        private static string _UserId;
    }
}