namespace Owin.Scim.Tests.Integration.Users.Create
{
    using System.Net;

    using Machine.Specifications;

    using Model.Users;

    using Ploeh.AutoFixture;
    
    public class with_a_fully_populated_simple_attributes_user : when_creating_a_user
    {
        Establish context = () =>
        {
            var autoFixture = new Fixture();

            UserDto = autoFixture.Build<User>()
                .With(x => x.UserName, UserNameUtility.GenerateUserName())
                .With(x => x.Password, "somePass")
                .With(x => x.PreferredLanguage, "en-US,en,es")
                .With(x => x.Locale, "en-US")
                .With(x => x.Timezone, @"US/Eastern")
                .With(x => x.ProfileUrl, null)
                .With(x => x.Emails, null)
                .With(x => x.PhoneNumbers, null)
                .With(x => x.Ims, null)
                .With(x => x.Photos, null)
                .With(x => x.Addresses, null)
                .Create();
        };

        It should_return_created = () => Response.StatusCode.ShouldEqual(HttpStatusCode.Created);

        It should_return_header_location = () => Response.Headers.Location.ShouldNotBeNull();

        It should_return_the_user = () => CreatedUser.Id.ShouldNotBeEmpty();

        It should_return_meta = () =>
        {
            CreatedUser.Meta.ShouldNotBeNull();
            CreatedUser.Meta.ResourceType.ShouldEqual("User");
            CreatedUser.Meta.Created.ShouldNotBeNull();
            CreatedUser.Meta.LastModified.ShouldEqual(CreatedUser.Meta.Created);
            CreatedUser.Meta.Location.ShouldNotBeNull();
            CreatedUser.Meta.Version.ShouldNotBeNull();
        };

        It should_echo_create_values = () =>
        {
            CreatedUser.Schemas.ShouldEqual(UserDto.Schemas);
            CreatedUser.UserName.ShouldEqual(UserDto.UserName);
            CreatedUser.DisplayName.ShouldEqual(UserDto.DisplayName);
            CreatedUser.NickName.ShouldEqual(UserDto.NickName);
            CreatedUser.ProfileUrl.ShouldEqual(UserDto.ProfileUrl);
            CreatedUser.Title.ShouldEqual(UserDto.Title);
            CreatedUser.UserType.ShouldEqual(UserDto.UserType);
            CreatedUser.PreferredLanguage.ShouldEqual(UserDto.PreferredLanguage);
            CreatedUser.Locale.ShouldEqual(UserDto.Locale);
            CreatedUser.Timezone.ShouldEqual(UserDto.Timezone);
            CreatedUser.Active.ShouldEqual(UserDto.Active);
            CreatedUser.Password.ShouldBeNull();
        };
    }
}