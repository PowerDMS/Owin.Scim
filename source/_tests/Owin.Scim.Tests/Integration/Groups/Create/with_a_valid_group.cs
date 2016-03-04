namespace Owin.Scim.Tests.Integration.Groups.Create
{
    using System;
    using System.Net;

    using Machine.Specifications;

    using Model.Groups;

    public class with_a_valid_group : when_creating_a_group
    {
        Establish context = () =>
        {
            // must test:
            // validation + canonization:
            // 1. displayName required
            // 2. members not required
            // 3. member.value and member.type required
            // 4. member.type canonical values = User/Group
            // 5. member.type/value must refer to a valid user or group

            GroupDto = new Group
            {
                DisplayName = "hello",
                ExternalId = "hello",
                //Members = new []{new Member {}, }
            };
        };

        It should_return_created = () => Response.StatusCode.ShouldEqual(HttpStatusCode.Created);
    }
}