namespace Owin.Scim.Tests.Integration.Users.Replace
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using Machine.Specifications;

    using Model.Users;

    using Ploeh.AutoFixture;

    public class with_several_attribute_changes : using_a_scim_server
    {
        Establish context = () =>
        {
            var autoFixture = new Fixture();

            var existingUser = autoFixture.Build<EnterpriseUser>()
                .With(x => x.UserName, UserNameUtility.GenerateUserName())
                .With(x => x.Password, "somePass")
                .With(x => x.PreferredLanguage, "en-US,en,es")
                .With(x => x.Locale, "en-US")
                .With(x => x.Timezone, @"US/Eastern")
                .With(x => x.Emails, null)
                .With(x => x.PhoneNumbers, null)
                .With(x => x.Ims, null)
                .With(x => x.Photos, null)
                .With(x => x.Addresses, null)
                .Create();


            // Insert the first user so there's one already in-memory.
            var userRecord = Server
                .HttpClient
                .PostAsync("users", new ObjectContent<EnterpriseUser>(existingUser, new ScimJsonMediaTypeFormatter()))
                .Result;

            UserDto = userRecord.Content.ReadAsAsync<EnterpriseUser>(ScimJsonMediaTypeFormatter.AsArray()).Result;

            UserId = UserDto.Id;

            // Common use case for replace is for client to send the full object back
            UserDto.UserName = UserNameUtility.GenerateUserName();
            UserDto.Password = "someOtherPass";
            UserDto.Enterprise.EmployeeNumber = "007";
        };

        Because of = async () =>
        {
            Response = await Server
                .HttpClient
                .PutAsync("users/" + UserId, new ObjectContent<EnterpriseUser>(UserDto, new ScimJsonMediaTypeFormatter()))
                .AwaitResponse()
                .AsTask;

            if (Response.StatusCode == HttpStatusCode.OK)
            {
                UpdatedUser = Response.Content.ReadAsAsync<EnterpriseUser>(ScimJsonMediaTypeFormatter.AsArray()).Result;
            }
        };

        It should_contain_the_new_values = () =>
        {
            Response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            UpdatedUser.UserName.ShouldEqual(UserDto.UserName);
            UpdatedUser.Enterprise.EmployeeNumber.ShouldEqual(UserDto.Enterprise.EmployeeNumber);
        };

        It should_not_return_password = () => UpdatedUser.Password.ShouldBeNull();

        It should_change_etag_with_new_version = () =>
        {
            UpdatedUser.Meta.Version.ShouldNotBeNull();
            UpdatedUser.Meta.Version.ShouldNotEqual(UserDto.Meta.Version);
        };

        It should_change_last_modified = () =>
        {
            UpdatedUser.Meta.LastModified.ShouldBeGreaterThan(UpdatedUser.Meta.Created);
            UpdatedUser.Meta.LastModified.ShouldBeGreaterThan(UserDto.Meta.LastModified);
        };

        protected static EnterpriseUser UserDto;

        protected static EnterpriseUser UpdatedUser;

        protected static string UserId;

        protected static HttpResponseMessage Response;
    }
}