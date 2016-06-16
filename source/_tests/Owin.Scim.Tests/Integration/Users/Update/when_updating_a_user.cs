namespace Owin.Scim.Tests.Integration.Users.Update
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    public class when_updating_a_user : using_a_scim_server
    {
        Because of = async () =>
        {
            // Insert the first user so there's one already in-memory.
            var userRecord = await Server
                .HttpClient
                .PostAsync("v2/users", new ScimObjectContent<ScimUser>(UserToUpdate))
                .AwaitResponse()
                .AsTask;

            UserToUpdate = await userRecord.Content.ScimReadAsAsync<ScimUser2>().AwaitResponse().AsTask;

            await Task.Delay(150).Await().AsTask;

            PatchResponse = await Server
                .HttpClient
                .SendAsync(
                    new HttpRequestMessage(
                        new HttpMethod("PATCH"), "v2/users/" + UserToUpdate.Id)
                    {
                        Content = PatchContent
                    })
                .AwaitResponse()
                .AsTask;

            if (PatchResponse.StatusCode == HttpStatusCode.OK)
                UpdatedUser = await PatchResponse.Content.ScimReadAsAsync<ScimUser2>();

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