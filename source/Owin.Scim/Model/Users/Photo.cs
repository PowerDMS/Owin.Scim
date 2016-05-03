namespace Owin.Scim.Model.Users
{
    using System;
    
    public class Photo : MultiValuedAttribute
    {
        public new Uri Value { get; set; }
    }
}