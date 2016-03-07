namespace Owin.Scim.Tests.Integration.Groups.Delete
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Model.Users;
    using Model.Groups;

    public class with_no_id : when_deleting_a_group
    {
        /// <summary>
        /// Must have a pre-existing group with members in it
        /// </summary>
        private Establish context = () =>
        {
            GroupId = string.Empty;
        };

        It should_return_not_allowed = () => Response.StatusCode.ShouldEqual(HttpStatusCode.MethodNotAllowed);
    }
}