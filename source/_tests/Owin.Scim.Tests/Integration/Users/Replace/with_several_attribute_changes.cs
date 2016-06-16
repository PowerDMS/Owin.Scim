namespace Owin.Scim.Tests.Integration.Users.Replace
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using Extensions;

    using Machine.Specifications;

    using Model.Users;

    using Ploeh.AutoFixture;

    using v2.Model;

    public class with_several_attribute_changes : when_replacing_a_user
    {
        Establish context = async () =>
        {
            var autoFixture = new Fixture();

            MutableUserPayload = autoFixture.Build<ScimUser2>()
                .With(x => x.UserName, UserNameUtility.GenerateUserName())
                .With(x => x.Password, "somePass!2")
                .With(x => x.PreferredLanguage, "en-US,en,es")
                .With(x => x.Locale, "en-US")
                .With(x => x.Timezone, @"US/Eastern")
                .With(x => x.Emails, null)
                .With(x => x.PhoneNumbers, null)
                .With(x => x.Ims, null)
                .With(x => x.Photos, null)
                .With(x => x.Addresses, null)
                .With(x => x.X509Certificates, null)
                .Create(seed: new ScimUser2());


            // Insert the first user so there's one already in-memory.
            var userRecord = Server
                .HttpClient
                .PostAsync("v2/users", new ScimObjectContent<ScimUser>(MutableUserPayload))
                .Result;

            await userRecord.DeserializeTo(() => OriginalUserRecord); // capture original user record

            // Common use case for replace is for client to send the full object back
            MutableUserPayload.Id = OriginalUserRecord.Id; // set server-assigned ID
            MutableUserPayload.UserName = UserNameUtility.GenerateUserName(); // new userName
            MutableUserPayload.Password = "someOtherPass!3"; // newPassword
            MutableUserPayload.Extension<EnterpriseUser2Extension>().EmployeeNumber = "007";

            UserDto = MutableUserPayload;
        };

        It should_return_success = () => Response.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_contain_the_new_values = () =>
        {
            UpdatedUserRecord.UserName.ShouldNotEqual(OriginalUserRecord.UserName);
            UpdatedUserRecord
                .Extension<EnterpriseUser2Extension>()
                .EmployeeNumber
                .ShouldNotEqual(OriginalUserRecord.Extension<EnterpriseUser2Extension>().EmployeeNumber);
        };

        It should_not_return_password = () => UpdatedUserRecord.Password.ShouldBeNull();

        It should_change_etag_with_new_version = () =>
        {
            UpdatedUserRecord.Meta.Version.ShouldNotBeNull();
            UpdatedUserRecord.Meta.Version.ShouldNotEqual(OriginalUserRecord.Meta.Version);
        };

        It should_change_last_modified = () =>
        {
            UpdatedUserRecord.Meta.LastModified.ShouldBeGreaterThan(UpdatedUserRecord.Meta.Created);
            UpdatedUserRecord.Meta.LastModified.ShouldBeGreaterThan(OriginalUserRecord.Meta.LastModified);
        };

        protected static ScimUser2 OriginalUserRecord;

        protected static ScimUser2 MutableUserPayload;
    }
}