namespace Owin.Scim.Tests.Integration.Groups.Create
{
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Model.Groups;

    public class when_creating_a_group : using_existing_user_and_group
    {
        Because of = async () =>
        {
            Response = await Server
                .HttpClient
                .PostAsync("groups", new ScimObjectContent<ScimGroup>(GroupDto))
                .AwaitResponse()
                .AsTask;

            var bodyText = Response.StatusCode == HttpStatusCode.Created
                ? await Response.Content.ReadAsStringAsync()
                : null;

            CreatedGroup = bodyText != null
                ? Newtonsoft.Json.JsonConvert.DeserializeObject<ScimGroup>(bodyText)
                : null;

            Error = Response.StatusCode == HttpStatusCode.BadRequest
                ? await Response.Content.ScimReadAsAsync<Model.ScimError>()
                : null;
        };
        
        protected static ScimGroup GroupDto;

        protected static ScimGroup CreatedGroup;

        protected static HttpResponseMessage Response;

        protected static Model.ScimError Error;
    }
}