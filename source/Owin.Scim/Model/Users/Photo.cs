namespace Owin.Scim.Model.Users
{
    using System;

    using Newtonsoft.Json;

    public class Photo : MultiValuedAttribute
    {
        [JsonProperty("value")]
        public new Uri Value { get; set; }
    }
}