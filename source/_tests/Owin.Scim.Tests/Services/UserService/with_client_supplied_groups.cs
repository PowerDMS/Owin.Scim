namespace Owin.Scim.Tests.Services.UserService
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Threading.Tasks;

    using FakeItEasy;

    using Machine.Specifications;

    using Model.Users;

    public class with_client_supplied_groups : when_updating_a_user
    {
        Establish context = () =>
        {
            A.CallTo(() => UserRepository.GetUser(A<string>._))
                .ReturnsLazily(GetUserRecord);

            ClientUserDto = new User
            {
                Id = "id",
                UserName = "name",
                Groups = new List<UserGroup>
                {
                    new UserGroup { Display = "Group 3", Value = "3" },
                    new UserGroup { Display = "Group 4", Value = "4" }
                }
            };

            _UserRecordGroups = new List<UserGroup>
            {
                new UserGroup { Display = "Group 1", Value = "1" },
                new UserGroup { Display = "Group 2", Value = "2" }
            }.ToImmutableList();
        };

        It should_ignore_client_user_groups = () => Result.Groups.ShouldContainOnly(_UserRecordGroups);

        private static IEnumerable<UserGroup> _UserRecordGroups;

        private static async Task<User> GetUserRecord()
        {
            return new User
            {
                Id = "id",
                UserName = "name",
                Groups = _UserRecordGroups
            };
        }
    }
}