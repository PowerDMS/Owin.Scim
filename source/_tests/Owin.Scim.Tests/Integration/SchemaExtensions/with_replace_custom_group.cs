namespace Owin.Scim.Tests.Integration.SchemaExtensions
{
    using System;
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Model;
    using Model.Groups;

    using Newtonsoft.Json;

    using Users;

    public class with_replace_custom_group : using_a_scim_server
    {
        Establish context = () =>
        {
            var existingGroup = new ScimGroup
            {
                DisplayName = UserNameUtility.GenerateUserName()
            };

            Response = Server
                .HttpClient
                .PostAsync("groups", new ScimObjectContent<ScimGroup>(existingGroup))
                .Result;

            GroupDto = Response.Content.ScimReadAsAsync<ScimGroup>().Result;

            GroupDto.AddExtension(
                new MyGroupSchema
                {
                    AnotherName = "anything",
                    IsGood = true,
                    EndDate = DateTime.Today.ToUniversalTime(),
                    ComplexData = new[]
                    {
                        new MyGroupSchema.MySubClass
                        {
                            DisplayName = "hello",
                            Value = "world"
                        }
                    }
                });
        };

        Because of = () =>
        {
            Response = Server
                .HttpClient
                .PutAsync("groups/" + GroupDto.Id, new ScimObjectContent<ScimGroup>(GroupDto))
                .Result;

            var bodyText = Response.Content.ReadAsStringAsync().Result;

            CreatedGroup = Response.StatusCode == HttpStatusCode.OK
                ? Response.Content.ScimReadAsAsync<ScimGroup>().Result
                : null;

            Error = Response.StatusCode == HttpStatusCode.BadRequest
                ? JsonConvert.DeserializeObject<ScimError>(bodyText)
                : null;
        };

        It should_return_ok = () => Response.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_return_new_version = () => CreatedGroup.Meta.Version.ShouldNotEqual(GroupDto.Meta.Version);

        It should_return_custom_schema = () =>
            CreatedGroup
                .Extension<MyGroupSchema>()
                .ShouldBeLike(GroupDto.Extension<MyGroupSchema>());

        protected static ScimGroup GroupDto;

        protected static ScimGroup CreatedGroup;

        protected static HttpResponseMessage Response;

        protected static ScimError Error;
    }
}