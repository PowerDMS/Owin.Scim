using System.Linq;

namespace Owin.Scim.Tests.Integration.CustomSchemas
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;
    using Newtonsoft.Json;

    using Model;
    using Model.Groups;
    using Users;

    public class with_patch_custom_group : using_a_scim_server
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
                            ""value"": {""value"":""world"", ""displayname"":""hello""}
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
                        new HttpMethod("PATCH"), "groups/" + GroupDto.Id)
                    {
                        Content = PatchContent
                    })
                .Result;

            var body = Response.Content.ReadAsStringAsync().Result;

            if (Response.StatusCode == HttpStatusCode.OK)
            {
                UpdatedGroup = JsonConvert.DeserializeObject<Group>(body);
            }
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

        [Ignore("Are we going to support nullable DateTime?")]
        It should_delete_enddate = () =>
            UpdatedGroup
                .Extension<MyGroupSchema>()
                .EndDate
                .ShouldBeNull();

        It should_add_complexdata = () =>
            UpdatedGroup
                .Extension<MyGroupSchema>()
                .ComplexData
                .Value
                .ShouldEqual("world");

        protected static Group GroupDto;

        protected static Group UpdatedGroup;

        protected static HttpResponseMessage Response;

        protected static HttpContent PatchContent;

        protected static ScimError Error;
    }
}