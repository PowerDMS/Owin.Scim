namespace Owin.Scim.Tests.Integration.Users.Create
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;

    using Newtonsoft.Json;

    using Machine.Specifications;

    /// <summary>
    /// What gets popu
    /// </summary>
    public class with_a_fully_populated_multi_attributes_user : using_a_scim_server
    {
        Establish context = () =>
        {
            UserDto = new MyUser
            {
                // older un-supported schema
                Schemas = new[] { @"urn:ietf:params:scim:schemas:core:2.0:User" },
                UserName = "Oops",
                Emails = new[]
                {
                    new MyMultiAttribute { Value = "My@BAD.net", Display="Ma Bad", Primary=true},
                    new MyMultiAttribute { Value = "My@BAD.net", Type = "same email"}
                },
                PhoneNumbers = new[]
                {
                    new MyMultiAttribute {Value="800123555", Display = "Here is my number", Type="toll-free" },
                    new MyMultiAttribute {Value="(407) 123-5555", Display = "so call me maybe", Type="local" },
                    new MyMultiAttribute {Value="1.407.123.5555", Type="local", Primary = false}
                },
                Ims = new[]
                {
                    new MyMultiAttribute {Value = "ICQ", Display = "Icq"},
                    new MyMultiAttribute {Value = "fb", Display = "facebook", Type = "personal"}
                },
                Photos = new[]
                {
                    new MyMultiAttribute {Value = "http://WeirdPhoto/23", Display = "Icq"}
                }
            };
        };

        Because of = () =>
        {
            Response = Server
                .HttpClient
                .PostAsync("users", new ObjectContent<MyUser>(UserDto, new ScimJsonMediaTypeFormatter
                {
                    SerializerSettings = { NullValueHandling = NullValueHandling.Ignore }
                }))
                .Result;

            StatusCode = Response.StatusCode;

            if (StatusCode == HttpStatusCode.Created)
            {
                var responseBody = Response.Content.ReadAsStringAsync().Result;
                CreatedUser = JsonConvert.DeserializeObject<MyUser>(responseBody);
            }

            Error = StatusCode != HttpStatusCode.BadRequest ? null : Response.Content
                .ReadAsAsync<System.Collections.Generic.IEnumerable<Model.ScimError>>(ScimJsonMediaTypeFormatter.AsArray())
                .Result
                .Single();
        };

        It should_return_created = () => Response.StatusCode.ShouldEqual(HttpStatusCode.Created);

        /// <summary>
        /// Note on echoing values: per protocol, service provider is required to echo a full saved state of the resource
        /// However, it is up to the service provider to decide what values to ignore, e.g. you might send multiple addresses
        /// but the service provider might just save one, or it might ignore display or type attributes.
        /// </summary>
        It should_cannonize_email_values = () =>
        {
            CreatedUser.ShouldNotBeNull();
            CreatedUser.Emails.Length.ShouldEqual(UserDto.Emails.Length);
            for (int i = 0; i < CreatedUser.Emails.Length; i++)
            {
                CreatedUser.Emails[i].Value.ShouldEqual(UserDto.Emails[i].Value);
                CreatedUser.Emails[i].Display.ShouldBeEqualIgnoringCase(UserDto.Emails[i].Value);
                CreatedUser.Emails[i].Type.ShouldEqual(UserDto.Emails[i].Type);
                CreatedUser.Emails[i].Primary.ShouldEqual(UserDto.Emails[i].Primary ?? false);
            }
        };

        It should_cannonize_phone_values = () =>
        {
            CreatedUser.ShouldNotBeNull();
            CreatedUser.PhoneNumbers.Length.ShouldEqual(UserDto.PhoneNumbers.Length);
            for (int i = 0; i < CreatedUser.PhoneNumbers.Length; i++)
            {
                CreatedUser.PhoneNumbers[i].Value.ShouldEqual(UserDto.PhoneNumbers[i].Value);
                UserDto.PhoneNumbers[i].Value.ToCharArray()
                    .ShouldContain(CreatedUser.PhoneNumbers[i].Display.ToCharArray());
                CreatedUser.PhoneNumbers[i].Type.ShouldEqual(UserDto.PhoneNumbers[i].Type);
                CreatedUser.PhoneNumbers[i].Primary.ShouldEqual(UserDto.PhoneNumbers[i].Primary ?? false);
            }
        };

        protected static MyUser UserDto;

        protected static MyUser CreatedUser;

        protected static HttpResponseMessage Response;

        protected static HttpStatusCode StatusCode;

        protected static Model.ScimError Error;

        /// <summary>
        /// Using my own user class to test interoperativity and not favor our own User class
        /// </summary>
        public class MyUser
        {
            public string[] Schemas { get; set; }
            public string UserName { get; set; }

            public MyMultiAttribute[] Emails { get; set; }

            public MyMultiAttribute[] PhoneNumbers { get; set; }

            public MyMultiAttribute[] Ims { get; set; }

            public MyMultiAttribute[] Photos { get; set; }
        }

        public class MyMultiAttribute
        {
            public string Value;
            public string Display;
            public string Type;
            public bool? Primary;
        }

        public class MyMultiAttribute2
        {
            public string Value;
            public string Display;
            public string Type;
        }

        public class MyMultiAttribute3
        {
            public string Value;
            public string Display;
        }

        public class MyMultiAttribute4
        {
            public string Value;
        }
    }
}