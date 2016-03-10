namespace Owin.Scim.Tests.Integration.Groups.Update
{
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;
    using Newtonsoft.Json;

    using Model.Groups;

    public class when_updating_a_group : using_existing_user_and_group
    {
        Because of = () =>
        {
            PatchResponse = Server
                .HttpClient
                .SendAsync(
                    new HttpRequestMessage(
                        new HttpMethod("PATCH"), "groups/" + PatchGroupId)
                    {
                        Content = PatchContent
                    })
                .Result;

            if (PatchResponse.StatusCode == HttpStatusCode.OK)
                UpdatedGroup = PatchResponse.Content.ReadAsAsync<Group>().Result;

            if (PatchResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorText = PatchResponse.Content.ReadAsStringAsync().Result;
                Error = JsonConvert.DeserializeObject<Model.ScimError>(errorText);
            }
        };

        protected static string PatchGroupId;

        protected static HttpContent PatchContent;

        protected static HttpResponseMessage PatchResponse;

        protected static Group UpdatedGroup;

        protected static Model.ScimError Error;
    }
}