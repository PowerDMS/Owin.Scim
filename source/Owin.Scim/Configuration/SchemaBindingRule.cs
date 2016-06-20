namespace Owin.Scim.Configuration
{
    using System;

    public class SchemaBindingRule
    {
        private readonly SchemaBindingPredicate _Predicate;

        private readonly Type _Target;

        public SchemaBindingRule(SchemaBindingPredicate predicate, Type target)
        {
            _Predicate = predicate;
            _Target = target;
        }

        public SchemaBindingPredicate Predicate
        {
            get { return _Predicate; }
        }

        public Type Target
        {
            get { return _Target; }
        }
    }
}