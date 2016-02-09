namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;

    using Model;
    using Model.Users;

    public class ScimServerConfiguration
    {
        private readonly ISet<AuthenticationScheme> _AuthenticationSchemes;

        private readonly IList<SchemaBindingRule> _SchemaBindingRules;

        public ScimServerConfiguration()
        {
            _AuthenticationSchemes = new HashSet<AuthenticationScheme>();
            _SchemaBindingRules = CreateDefaultBindingRules();

            RequireSsl = true;
        }

        private IList<SchemaBindingRule> CreateDefaultBindingRules()
        {
            var rules = new List<SchemaBindingRule>
            {
                new SchemaBindingRule(
                    schemaIdentifiers =>
                    {
                        if (schemaIdentifiers.Count == 1 &&
                            schemaIdentifiers.Contains(ScimConstants.Schemas.User))
                            return true;

                        return false;
                    },
                    typeof (User)),
                new SchemaBindingRule(
                    schemaIdentifiers =>
                    {
                        if (schemaIdentifiers.Contains(ScimConstants.Schemas.User) &&
                            schemaIdentifiers.Contains(ScimConstants.Schemas.UserEnterprise))
                            return true;

                        return false;
                    },
                    typeof (EnterpriseUser))
            };

            return rules;
        }

        public bool RequireSsl { get; set; }
        
        // TODO: should we make this Uri instead of string?
        public string PublicOrigin { get; set; }

        public IEnumerable<AuthenticationScheme> AuthenticationSchemes
        {
            get { return _AuthenticationSchemes; }
        }

        public IEnumerable<SchemaBindingRule> SchemaBindingRules
        {
            get { return _SchemaBindingRules; }
        }

        public ScimServerConfiguration AddAuthenticationScheme(AuthenticationScheme authenticationScheme)
        {
            // Enforce only one primary
            if (authenticationScheme.Primary)
            {
                foreach (var scheme in _AuthenticationSchemes)
                {
                    scheme.Primary = false;
                }
            }

            _AuthenticationSchemes.Add(authenticationScheme);
            return this;
        }

        public ScimServerConfiguration InsertSchemaBindingRule<TBindingTarget>(Predicate<ISet<string>> predicate) 
            where TBindingTarget : SchemaBase, new()
        {
            _SchemaBindingRules.Insert(0, new SchemaBindingRule(predicate, typeof (TBindingTarget)));

            return this;
        }
    }
}