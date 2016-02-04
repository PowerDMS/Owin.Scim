namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;

    public class SchemaBindingRule
    {
        private readonly Predicate<ISet<string>> _Predicate;

        private readonly Type _Target;

        public SchemaBindingRule(Predicate<ISet<string>> predicate, Type target)
        {
            _Predicate = predicate;
            _Target = target;
        }

        public Predicate<ISet<string>> Predicate
        {
            get { return _Predicate; }
        }

        public Type Target
        {
            get { return _Target; }
        }
    }
}