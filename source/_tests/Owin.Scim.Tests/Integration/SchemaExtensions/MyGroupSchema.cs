namespace Owin.Scim.Tests.Integration.SchemaExtensions
{
    using System;

    using Model;

    public class MyGroupSchema : ResourceExtension
    {
        public const string Schema = "urn:scim:mycustom:schema:1.0:Group";

        public string AnotherName { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsGood { get; set; }

        public MySubClass[] ComplexData { get; set; }

        protected override string SchemaIdentifier
        {
            get { return Schema; }
        }

        public override int CalculateVersion()
        {
            return new
            {
                AnotherName,
                EndDate,
                IsGood,
                ComplexData
            }.GetHashCode();
        }

        public class MySubClass
        {
            public string Value { get; set; }
            public string DisplayName { get; set; }
        }
    }
}