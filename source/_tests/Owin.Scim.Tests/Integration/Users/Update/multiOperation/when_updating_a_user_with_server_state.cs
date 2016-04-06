namespace Owin.Scim.Tests.Integration.Users.Update.multiOperation
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading.Tasks;

    using Machine.Specifications;

    using Model.Users;

    public class when_updating_a_user_with_server_state : using_a_scim_server
    {
        Because of = async () =>
        {
            // Insert the first user so there's one already in-memory.
            var userRecord = await Server
                .HttpClient
                .PostAsync("users", new ObjectContent<User>(UserToUpdate, new JsonMediaTypeFormatter()))
                .AwaitResponse()
                .AsTask;

            UserToUpdate = await userRecord.Content.ReadAsAsync<User>().AwaitResponse().AsTask;

            Task.Delay(200).Await();

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
                UpdatedUser = await PatchResponse.Content.ReadAsAsync<User>();

            if (PatchResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorText = PatchResponse.Content.ReadAsStringAsync().Result;
                Error = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.ScimError>(errorText);

                var response = Server.HttpClient.GetAsync("users/" + UserToUpdate.Id).Result;
                ServerUser = response.Content.ReadAsAsync<User>().Result;
            }
        };

        protected static User UserToUpdate;

        protected static User ServerUser;

        protected static HttpContent PatchContent;

        protected static HttpResponseMessage PatchResponse;

        protected static User UpdatedUser;

        protected static Model.ScimError Error;
    }
}