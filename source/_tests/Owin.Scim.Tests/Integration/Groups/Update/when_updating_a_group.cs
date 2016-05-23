namespace Owin.Scim.Tests.Integration.Groups.Update
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Machine.Specifications;
    using Newtonsoft.Json;

    using Model.Groups;

    public class when_updating_a_group : using_existing_user_and_group
    {
        Because of = async () =>
        {
            Task.Delay(100).Await();

            PatchResponse = await Server
                .HttpClient
                .SendAsync(
                    new HttpRequestMessage(
                        new HttpMethod("PATCH"), "groups/" + PatchGroupId)
                    {
                        Content = PatchContent
                    })
                .Await().AsTask;

            if (PatchResponse.StatusCode == HttpStatusCode.OK)
                UpdatedGroup = PatchResponse.Content.ReadAsAsync<ScimGroup>().Result;

            if (PatchResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorText = PatchResponse.Content.ReadAsStringAsync().Result;
                Error = JsonConvert.DeserializeObject<Model.ScimError>(errorText);
            }
        };

        protected static string PatchGroupId;

        protected static HttpContent PatchContent;

        protected static HttpResponseMessage PatchResponse;

        protected static ScimGroup UpdatedGroup;

        protected static Model.ScimError Error;
    }
}