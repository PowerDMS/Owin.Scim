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

    public class with_add_custom_group : using_a_scim_server
    {
        Establish context = () =>
        {
            GroupDto = new ScimGroup
            {
                DisplayName = UserNameUtility.GenerateUserName()
            };

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
                .PostAsync("groups", new ScimObjectContent<ScimGroup>(GroupDto))
                .Result;

            var bodyText = Response.Content.ReadAsStringAsync().Result;

            CreatedGroup = Response.StatusCode == HttpStatusCode.Created
                ? Response.Content.ScimReadAsAsync<ScimGroup>().Result
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

        protected static ScimGroup GroupDto;

        protected static ScimGroup CreatedGroup;

        protected static HttpResponseMessage Response;

        protected static ScimError Error;
    }
}