namespace Owin.Scim.Security
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class DefaultPasswordComplexityVerifier : IVerifyPasswordComplexity
    {
        public Task<bool> MeetsRequirements(string password)
        {
            if (String.IsNullOrWhiteSpace(password)) return Task.FromResult(false);
            
            var regexChecks = new[]
            {
                    new Regex(@"[a-z]", RegexOptions.Compiled | RegexOptions.Singleline),
                    new Regex(@"[A-Z]", RegexOptions.Compiled | RegexOptions.Singleline),
                    new Regex(@"[0-9]", RegexOptions.Compiled | RegexOptions.Singleline)
                };

            return Task.FromResult(regexChecks.All(x => x.IsMatch(password)));
        }
    }
}