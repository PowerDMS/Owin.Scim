namespace Owin.Scim.Tests.Integration.Users.Create
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Model;

    using Ploeh.AutoFixture;

    public class with_missing_enterprise_schema : using_a_scim_server
    {
        Establish context = () =>
        {
            var autoFixture = new Fixture();

            UserDto = new MyUser()
            {
                Schemas = new[] { @"urn:ietf:params:scim:schemas:core:2.0:User" },
                UserName = UserNameUtility.GenerateUserName(),
                Enterprise = autoFixture.Build<MyEnterprise>()
                    .With(x => x.Manager, autoFixture.Build<MyManager>()
                        .With(y => y.Ref, @"../Users/123")
                        .Create())
                    .Create()
            };
        };

        Because of = () =>
        {
            Response = Server
                .HttpClient
                .PostAsync("users", new ScimObjectContent<MyUser>(UserDto))
                .Result;

            StatusCode = Response.StatusCode;

            Error = StatusCode != HttpStatusCode.BadRequest ? null : Response.Content
                .ScimReadAsAsync<ScimError>()
                .Result;
        };

        It should_return_created = () => StatusCode.ShouldEqual(HttpStatusCode.Created);

        //   "This attribute ("schemas") may be used by parsers to define the attributes present 
        //    in the JSON structure that is the body to an HTTP request or response."
        // 
        // NOTE FROM AUTHOR: SCIM Interpretation
        // Resource extensions are always validated on the server.  Therefore, I've made "schemas"
        // a dynamic value based upon the resource instance and its extensions.  Extensions may be 
        // added with or without the schema identifier present in the schemas collection.
        // -Daniel Gioulakis
        It should_interpret_enterprise_schema = () =>
        {
            var createdUser = Response.Content
                .ScimReadAsAsync<MyUser>()
                .Result;

            createdUser.Id.ShouldNotBeNull();
            createdUser.Enterprise.ShouldNotBeNull();
        };

        protected static MyUser UserDto;

        protected static HttpResponseMessage Response;

        protected static HttpStatusCode StatusCode;

        protected static ScimError Error;

        public class MyUser
        {
            public string[] Schemas { get; set; }
            public string UserName { get; set; }
            public string Id { get; set; }

            [Newtonsoft.Json.JsonProperty(PropertyName = "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User")]
            public MyEnterprise Enterprise { get; set; }
        }

        public class MyEnterprise
        {
            public string EmployeeNumber { get; set; }
            public string CostCenter { get; set; }
            public string Organization { get; set; }
            public string Division { get; set; }
            public string Department { get; set; }
            public MyManager Manager { get; set; }
        }

        public class MyManager
        {
            public string Value { get; set; }
            [Newtonsoft.Json.JsonProperty(PropertyName = "$ref")]
            public string Ref { get; set; }
            public string DisplayName { get; set; }
        }
    }
}