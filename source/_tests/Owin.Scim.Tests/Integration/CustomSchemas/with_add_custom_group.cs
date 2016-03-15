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

    public class with_add_custom_group : using_a_scim_server
    {
        Establish context = () =>
        {
            GroupDto = new Group
            {
                DisplayName = UserNameUtility.GenerateUserName()
            };

            GroupDto.Extension<MyGroupSchema>().AnotherName = "anything";
            GroupDto.Extension<MyGroupSchema>().IsGood = true;
            GroupDto.Extension<MyGroupSchema>().EndDate = DateTime.Today;
            GroupDto.Extension<MyGroupSchema>().ComplexData = new MyGroupSchema.MySubClass
            {
                DisplayName = "hello",
                Value = "world"
            };
        };

        Because of = () =>
        {
            Response = Server
                .HttpClient
                .PostAsync("groups", new ObjectContent<Group>(GroupDto, new ScimJsonMediaTypeFormatter()))
                .Result;

            var bodyText = Response.Content.ReadAsStringAsync().Result;

            CreatedGroup = Response.StatusCode == HttpStatusCode.Created
                ? JsonConvert.DeserializeObject<Group>(bodyText)
                : null;

            Error = Response.StatusCode == HttpStatusCode.BadRequest
                ? JsonConvert.DeserializeObject<ScimError>(bodyText)
                : null;
        };

        It should_return_created = () => Response.StatusCode.ShouldEqual(HttpStatusCode.Created);

        It should_return_the_group = () => CreatedGroup.Id.ShouldNotBeEmpty();

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