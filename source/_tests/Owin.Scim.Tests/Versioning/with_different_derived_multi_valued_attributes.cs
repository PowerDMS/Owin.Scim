namespace Owin.Scim.Tests.Versioning
{
    using System.Collections.Generic;

    using Machine.Specifications;

    using Model.Users;

    public class with_different_derived_multi_valued_attributes : when_generating_a_User_etags<User>
    {
        Establish ctx = () =>
        {
            User = new User { Addresses = _Addresses };
        };

        Because of = () =>
        {
            _Addresses.Add(new Address
            {
                Type = "home",
                Country = "USA",
                Locality = "en-US",
                PostalCode = "10010",
                Region = "1",
                StreetAddress = "213 E 26th St."
            });

            User1ETag = User.GenerateETagHash();

            _Addresses.Add(new Address
            {
                Type = "work",
                Country = "USA",
                Locality = "en-US",
                PostalCode = "32801",
                Region = "1",
                StreetAddress = "150 E Robinson St."
            });

            User2ETag = User.GenerateETagHash();
        };

        It should_be_different_values = () => User1ETag.ShouldNotEqual(User2ETag);

        private static readonly List<Address> _Addresses = new List<Address>();
    }
}