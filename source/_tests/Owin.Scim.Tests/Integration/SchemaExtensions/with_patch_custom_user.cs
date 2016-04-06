namespace Owin.Scim.Tests.Integration.SchemaExtensions
{
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model;
    using Model.Users;

    using Newtonsoft.Json;

    using Users;

    public class with_patch_custom_user : using_a_scim_server
    {
        Establish context = () =>
        {
            var existingUser = new User
            {
                UserName = UserNameUtility.GenerateUserName()
            };

            Response = Server
                .HttpClient
                .PostAsync("users", new ObjectContent<User>(existingUser, new ScimJsonMediaTypeFormatter()))
                .Result;

            UserDto = Response.Content.ReadAsAsync<User>(ScimJsonMediaTypeFormatter.AsArray()).Result;

           
            PatchContent = new StringContent(
                @"
                    {
                        ""schemas"": [""urn:ietf:params:scim:api:messages:2.0:PatchOp"",
                                      ""urn:scim:mycustom:schema:1.0:User""],
                        ""Operations"": [{
                            ""op"": ""add"",
                            ""path"": ""urn:scim:mycustom:schema:1.0:User:guid"",
                            ""value"": ""something new""
                        },
                        {
                            ""op"": ""add"",
                            ""path"": ""urn:scim:mycustom:schema:1.0:User:$ref"",
                            ""value"": ""http://localhost/users/12345""
                        },
                        {
                            ""op"": ""replace"",
                            ""path"": ""urn:scim:mycustom:schema:1.0:User:enablehelp"",
                            ""value"": false
                        },
                        {
                            ""op"": ""remove"",
                            ""path"": ""urn:scim:mycustom:schema:1.0:User:enddate""
                        },
                        {
                            ""op"": ""add"",
                            ""path"": ""urn:scim:mycustom:schema:1.0:User:complexdata.value"",
                            ""value"": ""its complicated""
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
                        new HttpMethod("PATCH"), "users/" + UserDto.Id)
                    {
                        Content = PatchContent
                    })
                .Result;

            var body = Response.Content.ReadAsStringAsync().Result;

            if (Response.StatusCode == HttpStatusCode.OK)
            {
                UpdatedUser = JsonConvert.DeserializeObject<User>(body);
            }
        };

        It should_return_ok = () => Response.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_return_new_version = () => UpdatedUser.Meta.Version.ShouldNotEqual(UserDto.Meta.Version);

        It should_return_guid = () =>
            UpdatedUser
                .Extension<MyUserSchema>()
                .Guid
                .ShouldEqual("something new");

        It should_replace_enablehelp = () =>
            UpdatedUser
                .Extension<MyUserSchema>()
                .EnableHelp
                .ShouldEqual(false);

        It should_delete_enddate = () =>
            UpdatedUser
                .Extension<MyUserSchema>()
                .EndDate
                .ShouldBeNull();

        It should_add_complexdata = () =>
            UpdatedUser
                .Extension<MyUserSchema>()
                .ComplexData
                .Value
                .ShouldEqual("its complicated");

        protected static User UserDto;

        protected static User UpdatedUser;

        protected static HttpContent PatchContent;

        protected static HttpResponseMessage Response;

        protected static ScimError Error;
    }
}