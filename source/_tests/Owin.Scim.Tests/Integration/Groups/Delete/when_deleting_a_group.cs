namespace Owin.Scim.Tests.Integration.Groups.Delete
{
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Model.Groups;

    public class when_deleting_a_group : using_existing_user_and_group
    {
        Because of = () =>
        {
            Response = Server
                .HttpClient
                .DeleteAsync("v2/groups/" + GroupId)
                .Result;

            Error = Response.StatusCode == HttpStatusCode.BadRequest
                ? Response.Content.ScimReadAsAsync<Model.ScimError>().Result
                : null;
        };

        protected static string GroupId;

        protected static ScimGroup GroupDto;

        protected static ScimGroup RetrievedGroup;

        protected static HttpResponseMessage Response;

        protected static Model.ScimError Error;
    }
}