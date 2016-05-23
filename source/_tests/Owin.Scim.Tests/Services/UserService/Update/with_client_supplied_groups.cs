namespace Owin.Scim.Tests.Services.UserService.Update
{
    using System.Collections.Generic;
    using System.Linq;
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

            A.CallTo(() => GroupRepository.GetGroupsUserBelongsTo(A<string>._))
                .ReturnsLazily(() => _UserRecordGroups);

            ClientUserDto = new ScimUser
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
            };
        };

        It should_ignore_client_user_groups = () => Result.GetRight().Groups.ShouldContainOnly(_UserRecordGroups);
        
        It should_not_overwrite_references = () => Result.GetRight().Groups.Select(g => g.Value).ShouldContainOnly("1", "2");

        private static IEnumerable<UserGroup> _UserRecordGroups;

        private static Task<ScimUser> GetUserRecord()
        {
            return Task.FromResult(
                new ScimUser
                {
                    Id = "id",
                    UserName = "name",
                    Groups = _UserRecordGroups
                });
        }
    }
}