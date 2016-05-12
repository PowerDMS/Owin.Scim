namespace Owin.Scim.ErrorHandling
{
    public static class ScimErrorDetail
    {
        public static string AttributeImmutable(string attribute)
        {
            return string.Format(@"Attribute '{0}' is immutable and cannot be changed.", attribute);
        }

        public static string AttributeRequired(string attribute)
        {
            return string.Format(@"Attribute '{0}' is required.", attribute);
        }

        public static string AttributeReadOnly(string attribute)
        {
            return string.Format(@"Attribute '{0}' is readOnly.", attribute);
        }

        public static string AttributeUnique(string attribute)
        {
            return string.Format(@"Attribute '{0}' MUST be unique.", attribute);
        }

        public static string NotFound(string resourceId)
        {
            return string.Format(@"Resource '{0}' not found.", resourceId);
        }
    }
}