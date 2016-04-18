namespace Owin.Scim.Model.Users
{
    using System;

    [ScimTypeDefinition(typeof(PhotoDefinition))]
    public class Photo : MultiValuedAttribute
    {
        public new Uri Value { get; set; }
    }
}