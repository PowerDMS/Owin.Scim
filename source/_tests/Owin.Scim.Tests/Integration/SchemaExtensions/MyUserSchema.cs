namespace Owin.Scim.Tests.Integration.SchemaExtensions
{
    using System;

    using Model;

    using Newtonsoft.Json;

    public class MyUserSchema : ResourceExtension
    {
        public const string Schema = "urn:scim:mycustom:schema:1.0:User";

        public string Guid { get; set; }

        public DateTime? EndDate { get; set; }

        public bool EnableHelp { get; set; }

        [JsonProperty("$ref")]
        public string Ref { get; set; }

        public MySubClass ComplexData { get; set; }

        public override int CalculateVersion()
        {
            return new
            {
                Guid,
                EndDate,
                EnableHelp
            }.GetHashCode();
        }

        public class MySubClass
        {
            public string Value { get; set; }
            public string DisplayName { get; set; }
        }
    }
}