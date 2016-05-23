namespace Owin.Scim.Tests.Integration.SchemaExtensions
{
    using System;
    using System.Net;

    using FluentValidation;

    using Model;
    using Model.Users;

    using Scim.Validation;

    public class MyUserSchemaValidator : ResourceExtensionValidatorBase<ScimUser, MyUserSchema>
    {
        protected override void ConfigureDefaultRuleSet()
        {
            RuleFor(user => user.Ref)
                .Must(_ref => _ref == null || Uri.IsWellFormedUriString(_ref, UriKind.RelativeOrAbsolute))
                .WithState(user => new ScimError(
                    HttpStatusCode.BadRequest,
                    ScimErrorType.InvalidSyntax,
                    "The attribute '$ref' must have a valid url."));
        }

        protected override void ConfigureCreateRuleSet()
        {
        }

        protected override void ConfigureUpdateRuleSet()
        {
        }

        public override string ExtensionSchema
        {
            get { return MyUserSchema.Schema; }
        }
    }
}