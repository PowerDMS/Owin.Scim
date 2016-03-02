namespace Owin.Scim.Tests.Integration.Groups.Create
{
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Model.Groups;

    public class when_creating_a_group : using_a_scim_server
    {
        Because of = async () =>
        {
            Response = await Server
                .HttpClient
                .PostAsync("groups", new ObjectContent<Group>(GroupDto, new ScimJsonMediaTypeFormatter()))
                .AwaitResponse()
                .AsTask;

            var bodyText = Response.StatusCode == HttpStatusCode.Created
                ? await Response.Content.ReadAsStringAsync()
                : null;

            CreatedGroup = bodyText != null
                ? Newtonsoft.Json.JsonConvert.DeserializeObject<Group>(bodyText)
                : null;

            Error = Response.StatusCode == HttpStatusCode.BadRequest
                ? await Response.Content.ReadAsAsync<Model.ScimError>(ScimJsonMediaTypeFormatter.AsArray())
                : null;
        };
        
        protected static Group GroupDto;

        protected static Group CreatedGroup;

        protected static HttpResponseMessage Response;

        protected static Model.ScimError Error;
    }
}