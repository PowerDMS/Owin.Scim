namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;
    
    public delegate bool SchemaBindingPredicate(ISet<string> schemas, Type objectType);
}