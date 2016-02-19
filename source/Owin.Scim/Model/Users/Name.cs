namespace Owin.Scim.Model.Users
{
    using System.ComponentModel;
    
    public class Name
    {
        [Description(@"
        The full name, including all middle
        names, titles, and suffixes as appropriate, formatted for display
        (e.g., 'Ms. Barbara J Jensen, III').")]
        public string Formatted { get; set; }

        [Description(@"
        The family name of the User, or
        last name in most Western languages (e.g., 'Jensen' given the full
        name 'Ms. Barbara J Jensen, III').")]
        public string FamilyName { get; set; }

        [Description(@"
        The given name of the User, or
        first name in most Western languages (e.g., 'Barbara' given the
        full name 'Ms. Barbara J Jensen, III').")]
        public string GivenName { get; set; }

        [Description(@"
        The middle name(s) of the User
        (e.g., 'Jane' given the full name 'Ms. Barbara J Jensen, III').")]
        public string MiddleName { get; set; }

        [Description(@"
        The honorific prefix(es) of the User, or
        title in most Western languages (e.g., 'Ms.' given the full name
        'Ms. Barbara J Jensen, III').")]
        public string HonorificPrefix { get; set; }

        [Description(@"
        The honorific suffix(es) of the User, or
        suffix in most Western languages(e.g., 'III' given the full name
        'Ms. Barbara J Jensen, III').")]
        public string HonorificSuffix { get; set; }
    }
}