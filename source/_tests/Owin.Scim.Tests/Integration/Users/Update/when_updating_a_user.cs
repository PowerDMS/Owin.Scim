namespace Owin.Scim.Tests.Integration.Users.Update
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading.Tasks;

    using Machine.Specifications;

    using Model.Users;

    public class when_updating_a_user : using_a_scim_server
    {
        Because of = async () =>
        {
            // Insert the first user so there's one already in-memory.
            var userRecord = await Server
                .HttpClient
                .PostAsync("users", new ScimObjectContent<ScimUser>(UserToUpdate))
                .AwaitResponse()
                .AsTask;

            UserToUpdate = await userRecord.Content.ScimReadAsAsync<ScimUser>().AwaitResponse().AsTask;

            Task.Delay(100).Await();

            PatchResponse = await Server
                .HttpClient
                .SendAsync(
                    new HttpRequestMessage(
                        new HttpMethod("PATCH"), "users/" + UserToUpdate.Id)
                    {
                        Content = PatchContent
                    })
                .AwaitResponse()
                .AsTask;

            if (PatchResponse.StatusCode == HttpStatusCode.OK)
                UpdatedUser = await PatchResponse.Content.ScimReadAsAsync<ScimUser>();

            if (PatchResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorText = PatchResponse.Content.ReadAsStringAsync().Result;
                Error = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.ScimError>(errorText);
            }
        };

        protected static ScimUser UserToUpdate;

        protected static HttpContent PatchContent;

        protected static HttpResponseMessage PatchResponse;

        protected static ScimUser UpdatedUser;

        protected static Model.ScimError Error;
    }
}