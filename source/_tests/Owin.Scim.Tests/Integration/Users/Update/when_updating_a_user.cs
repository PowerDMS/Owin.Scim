namespace Owin.Scim.Tests.Integration.Users.Update
{
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using Machine.Specifications;

    using Marvin.JsonPatch;

    using Model;
    using Model.Users;

    public abstract class when_updating_a_user : using_a_scim_server
    {
        Establish context = async () =>
        {
            var userName = UserNameUtility.GenerateUserName();
            var existingUser = new User
            {
                UserName = userName
            };

            // Insert the first user so there's one already in-memory.
            var userRecord = await Server
                .HttpClient
                .PostAsync("users", new ObjectContent<User>(existingUser, new JsonMediaTypeFormatter()))
                .AwaitResponse()
                .AsTask;

            _UserId = (await userRecord.Content.ReadAsAsync<User>()).Id;
        };

        Because of = async () =>
        {
            Response = await Server
                .HttpClient
                .SendAsync(
                    new HttpRequestMessage(
                        new HttpMethod("PATCH"), "users/" + _UserId)
                    {
                        Content = new ObjectContent<PatchRequest<User>>(
                            new PatchRequest<User>
                            {
                                Operations = PatchDocument
                            }, 
                            new JsonMediaTypeFormatter())
                    })
                .AwaitResponse()
                .AsTask;
        };

        protected static JsonPatchDocument<User> PatchDocument;

        protected static HttpResponseMessage Response;

        private static string _UserId;
    }
}