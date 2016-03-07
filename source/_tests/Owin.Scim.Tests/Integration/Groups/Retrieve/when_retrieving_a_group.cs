namespace Owin.Scim.Tests.Integration.Groups.Retrieve
{
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Model.Groups;

    public class when_retrieving_a_group : using_existing_user_and_group
    {
        Because of = () =>
        {
            Response = Server
                .HttpClient
                .GetAsync("groups/" + GroupId)
                .Result;

            var bodyText = Response.StatusCode == HttpStatusCode.OK
                ? Response.Content.ReadAsStringAsync().Result
                : null;

            RetrievedGroup = bodyText != null
                ? Newtonsoft.Json.JsonConvert.DeserializeObject<Group>(bodyText)
                : null;

            Error = Response.StatusCode == HttpStatusCode.BadRequest
                ? Response.Content.ReadAsAsync<Model.ScimError>(ScimJsonMediaTypeFormatter.AsArray()).Result
                : null;
        };

        protected static string GroupId;

        protected static Group GroupDto;

        protected static Group RetrievedGroup;

        protected static HttpResponseMessage Response;

        protected static Model.ScimError Error;
    }
}