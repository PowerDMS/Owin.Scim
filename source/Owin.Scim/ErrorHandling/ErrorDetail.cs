namespace Owin.Scim.ErrorHandling
{

    public static class ErrorDetail
    {
        public static string AttributeImmutable(string attribute)
        {
            return string.Format(@"Attribute {0} is immutable and cannot be changed.", attribute);
        }

        public static string AttributeRequired(string attribute)
        {
            return string.Format(@"Attribute {0} is required.", attribute);
        }

        public static string AttributeReadOnly(string attribute)
        {
            return string.Format(@"Attribute {0} is readOnly.", attribute);
        }

        public static string AttributeUnique(string attribute)
        {
            return string.Format(@"Attribute {0} MUST be unique.", attribute);
        }
    }
}