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
            GroupDto = new Group
            {
                DisplayName = "hello",
                ExternalId = "hello"
            };
        };

        It should_return_created = () => Response.StatusCode.ShouldEqual(HttpStatusCode.Created);
    }
}