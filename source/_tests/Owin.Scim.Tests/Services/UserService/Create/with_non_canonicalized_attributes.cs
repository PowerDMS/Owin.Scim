namespace Owin.Scim.Tests.Services.UserService.Create
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Extensions;

    using Machine.Specifications;

    using Model.Users;

    public class with_non_canonicalized_attributes : when_creating_a_user
    {
        Establish context = () =>
        {
            ClientUserDto = new User
            {
                UserName = "daniel",
                Active = true,
                Addresses = new List<MailingAddress>
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
                        Display = "daniel", // readOnly, will be ignored
                        Value = "daniel.gioulakis@POWERDMS.com"
                    }
                },
                Photos = new List<Photo>
                {
                    new Photo { Value = new Uri("http://example.COM/me.jpg"), Primary = true },
                    new Photo { Value = new Uri("http://example.COM/me2.jpg"), Primary = true },
                    new Photo { Value = new Uri("http://example.COM/me3.jpg"), Primary = true }
                }
            };
        };

        It should_canonicalize_emails =
            () => Result
                .GetRight()
                .Emails
                .All(email => ShouldExtensions.ShouldBeLowercase(email.Display.Substring(email.Display.IndexOf('@') + 1)));

        It should_contain_only_one_primary = () => Result.GetRight().Photos.Count(p => p.Primary).ShouldEqual(1);
    }
}