namespace Owin.Scim.Tests.Integration.Groups.Replace
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Machine.Specifications;

    using Model;
    using Model.Groups;

    public class when_replacing_a_group : using_existing_user_and_group
    {
        Because of = async () =>
        {
            Task.Delay(100).Await();

            Response = await Server
                .HttpClient
                .PutAsync("groups/" + GroupId, new ScimObjectContent<Group>(GroupDto))
                .AwaitResponse()
                .AsTask;

            var bodyText = Response.StatusCode == HttpStatusCode.OK
                ? await Response.Content.ReadAsStringAsync()
                : null;

            CreatedGroup = bodyText != null
                ? Response.Content.ScimReadAsAsync<Group>().Result
                : null;

            Error = Response.StatusCode == HttpStatusCode.BadRequest
                ? await Response.Content.ScimReadAsAsync<ScimError>()
                : null;
        };

        protected static string GroupId;

        protected static Group GroupDto;

        protected static Group CreatedGroup;

        protected static HttpResponseMessage Response;

        protected static Model.ScimError Error;
    }
}