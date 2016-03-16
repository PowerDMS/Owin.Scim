namespace Owin.Scim.Tests.Integration.CustomSchemas
{
    using System;
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;
    using Newtonsoft.Json;

    using Model;
    using Model.Groups;
    using Users;

    public class with_replace_custom_group : using_a_scim_server
    {
        Establish context = () =>
        {
            var existingGroup = new Group
            {
                DisplayName = UserNameUtility.GenerateUserName()
            };

            Response = Server
                .HttpClient
                .PostAsync("groups", new ObjectContent<Group>(existingGroup, new ScimJsonMediaTypeFormatter()))
                .Result;

            GroupDto = Response.Content.ReadAsAsync<Group>(ScimJsonMediaTypeFormatter.AsArray()).Result;

            GroupDto.Extension<MyGroupSchema>().AnotherName = "anything";
            GroupDto.Extension<MyGroupSchema>().IsGood = true;
            GroupDto.Extension<MyGroupSchema>().EndDate = DateTime.Today;
            GroupDto.Extension<MyGroupSchema>().ComplexData = new [] { new MyGroupSchema.MySubClass
            {
                DisplayName = "hello",
                Value = "world"
            }};
        };

        Because of = () =>
        {
            Response = Server
                .HttpClient
                .PutAsync("groups/" + GroupDto.Id, new ObjectContent<Group>(GroupDto, new ScimJsonMediaTypeFormatter()))
                .Result;

            var bodyText = Response.Content.ReadAsStringAsync().Result;

            CreatedGroup = Response.StatusCode == HttpStatusCode.OK
                ? JsonConvert.DeserializeObject<Group>(bodyText)
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

        protected static Group GroupDto;

        protected static Group CreatedGroup;

        protected static HttpResponseMessage Response;

        protected static ScimError Error;
    }
}