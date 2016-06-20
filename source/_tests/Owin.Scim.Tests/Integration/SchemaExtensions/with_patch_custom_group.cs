namespace Owin.Scim.Tests.Integration.SchemaExtensions
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model;
    using Model.Groups;

    using Newtonsoft.Json;

    using Users;

    using v2.Model;

    public class with_patch_custom_group : using_a_scim_server
    {
        Establish context = () =>
        {
            var existingGroup = new ScimGroup2
            {
                DisplayName = UserNameUtility.GenerateUserName()
            };

            Response = Server
                .HttpClient
                .PostAsync("v2/groups", new ScimObjectContent<ScimGroup>(existingGroup))
                .Result;

            GroupDto = Response.Content.ScimReadAsAsync<ScimGroup2>().Result;

            PatchContent = new StringContent(
                @"
                    {
                        ""schemas"": [""urn:ietf:params:scim:api:messages:2.0:PatchOp"",
                                      ""urn:scim:mycustom:schema:1.0:Group""],
                        ""Operations"": [{
                            ""op"": ""add"",
                            ""path"": ""urn:scim:mycustom:schema:1.0:Group:anothername"",
                            ""value"": ""something new""
                        },
                        {
                            ""op"": ""replace"",
                            ""path"": ""urn:scim:mycustom:schema:1.0:Group:IsGood"",
                            ""value"": false
                        },
                        {
                            ""op"": ""remove"",
                            ""path"": ""urn:scim:mycustom:schema:1.0:Group:enddate""
                        },
                        {
                            ""op"": ""add"",
                            ""path"": ""urn:scim:mycustom:schema:1.0:Group:complexdata"",
                            ""value"": [{""value"":""world"", ""displayname"":""hello""}]
                       }]
                    }",
                Encoding.UTF8,
                "application/json");

        };

        Because of = () =>
        {
            Response = Server
                .HttpClient
                .SendAsync(
                    new HttpRequestMessage(
                        new HttpMethod("PATCH"), "v2/groups/" + GroupDto.Id)
                    {
                        Content = PatchContent
                    })
                .Result;
            
            if (Response.StatusCode == HttpStatusCode.OK)
                UpdatedGroup = Response.Content.ScimReadAsAsync<ScimGroup2>().Result;
        };

        It should_return_ok = () => Response.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_return_new_version = () => UpdatedGroup.Meta.Version.ShouldNotEqual(GroupDto.Meta.Version);

        It should_return_guid = () =>
            UpdatedGroup
                .Extension<MyGroupSchema>()
                .AnotherName
                .ShouldEqual("something new");

        It should_replace_enablehelp = () =>
            UpdatedGroup
                .Extension<MyGroupSchema>()
                .IsGood
                .ShouldEqual(false);

        It should_delete_enddate = () =>
            UpdatedGroup
                .Extension<MyGroupSchema>()
                .EndDate
                .ShouldBeNull();

        It should_add_complexdata = () =>
            UpdatedGroup
                .Extension<MyGroupSchema>()
                .ComplexData
                .First()
                .Value
                .ShouldEqual("world");

        protected static ScimGroup GroupDto;

        protected static ScimGroup UpdatedGroup;

        protected static HttpResponseMessage Response;

        protected static HttpContent PatchContent;

        protected static ScimError Error;
    }
}