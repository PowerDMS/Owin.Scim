namespace Owin.Scim.Configuration
{
    using System.Collections.Generic;

    using Model;

    public class ScimServerConfiguration
    {
        private readonly ISet<AuthenticationScheme> _AuthenticationSchemes;

        public ScimServerConfiguration()
        {
            _AuthenticationSchemes = new HashSet<AuthenticationScheme>();
            RequireSsl = true;
        }

        public bool RequireSsl { get; set; }

        public string PublicOrigin { get; set; }

        public IEnumerable<AuthenticationScheme> AuthenticationSchemes
        {
            get { return _AuthenticationSchemes; }
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
    }
}