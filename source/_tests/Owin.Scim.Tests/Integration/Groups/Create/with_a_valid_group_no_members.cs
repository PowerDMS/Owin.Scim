namespace Owin.Scim.Tests.Integration.Groups.Create
{
    using System;
    using System.Net;

    using Machine.Specifications;

    using Model.Groups;

    using v2.Model;

    public class with_a_valid_group_no_members : when_creating_a_group
    {
        Establish context = () =>
        {
            TestStartTime = DateTime.UtcNow;

            GroupDto = new ScimGroup2
            {
                DisplayName = "hello",
                ExternalId = "hello",
            };
        };

        It should_return_created = () => Response.StatusCode.ShouldEqual(HttpStatusCode.Created);

        It should_contain_id = () => CreatedGroup.Id.ShouldNotBeEmpty();

        It should_contain_meta = () =>
        {
            CreatedGroup.Meta.ShouldNotBeNull();
            CreatedGroup.Meta.Created.ShouldBeGreaterThanOrEqualTo(TestStartTime);
            CreatedGroup.Meta.LastModified.ShouldEqual(CreatedGroup.Meta.Created);
            CreatedGroup.Meta.Location.ShouldNotBeNull();
            CreatedGroup.Meta.Location.ShouldEqual(Response.Headers.Location);
            CreatedGroup.Meta.Version.ShouldNotBeNull();
            CreatedGroup.Meta.Version.ShouldEqual(Response.Headers.ETag.ToString());
        };

        private static DateTime TestStartTime;
    }
}