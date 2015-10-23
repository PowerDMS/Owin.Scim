namespace Owin.Scim.Tests.Services.UserService.Create
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using Configuration;

    using Extensions;

    using FakeItEasy;

    using Machine.Specifications;

    using Mappings;

    using Model;
    using Model.Users;

    using Repository;

    using Scim.Services;
    using Scim.Validation.Users;

    using Security;

    public abstract class when_creating_a_user
    {
        Establish context = () =>
        {
            Mapper.Initialize(c => new UserMapping().Configure(Mapper.Configuration));

            UserRepository = A.Fake<IUserRepository>();
            PasswordManager = A.Fake<IManagePasswords>();
            PasswordComplexityVerifier = A.Fake<IVerifyPasswordComplexity>();

            A.CallTo(() => UserRepository.IsUserNameAvailable(A<string>._))
                .Returns(true);
            
            A.CallTo(() => UserRepository.CreateUser(A<User>._))
                .ReturnsLazily(c =>
                {
                    var user = (User) c.Arguments[0];
                    user.Id = Guid.NewGuid().ToString("N");

                    return Task.FromResult(user);
                });

            _UserService = new UserService(
                new ScimServerConfiguration(), 
                UserRepository, 
                PasswordManager, 
                new UserValidator(UserRepository, PasswordComplexityVerifier, PasswordManager));
        };

        Because of = async () => Result = await _UserService.CreateUser(ClientUserDto).AwaitResponse().AsTask;

        protected static User ClientUserDto;

        protected static IUserRepository UserRepository;

        protected static IManagePasswords PasswordManager;

        protected static IVerifyPasswordComplexity PasswordComplexityVerifier;

        protected static IScimResponse<User> Result;

        private static IUserService _UserService;
    }

    public class with_non_canonicalized_attributes : when_creating_a_user
    {
        Establish context = () =>
        {
            ClientUserDto = new User
            {
                UserName = "daniel",
                Active = true,
                Addresses = new List<Address>
                {
                    
                },
                Entitlements = new List<Entitlement>
                {
                    new Entitlement { Value = "create_document" },
                    null
                },
                Emails = new List<Email>
                {
                    new Email
                    {
                        Display = "daniel", // not allowed, will be overwritten
                        Value = "daniel.gioulakis@POWERDMS.com"
                    }
                },
                Photos = new List<Photo>
                {
                    new Photo { Value = "http://example.COM/me.jpg", Primary = true },
                    new Photo { Value = "http://example.COM/me2.jpg", Primary = true },
                    new Photo { Value = "http://example.COM/me3.jpg", Primary = true }
                }
            };
        };

        It should_canonicalize_emails =
            () => Result
                .GetRight()
                .Emails
                .All(email => email.Display.Substring(email.Display.IndexOf('@') + 1).ShouldBeLowercase());

        It should_contain_only_one_primary = () => Result.GetRight().Photos.Count(p => p.Primary).ShouldEqual(1);

        It should_lowercase_the_value = () => Result.GetRight().Photos.All(photo => photo.Value.ShouldBeLowercase());
    }
}