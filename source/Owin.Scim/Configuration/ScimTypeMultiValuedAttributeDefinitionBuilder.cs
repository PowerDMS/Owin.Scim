namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    using Model;

    using NContext.Extensions;

    public class ScimTypeMultiValuedAttributeDefinitionBuilder<T, TMultiValuedAttribute>
        : ScimTypeMemberDefinitionBuilder<T, TMultiValuedAttribute>
        where TMultiValuedAttribute : MultiValuedAttribute
    {
        private readonly ISet<IScimTypeMemberDefinitionBuilder> _SubAttributeDefinitions = 
            new HashSet<IScimTypeMemberDefinitionBuilder>();
        
        public ScimTypeMultiValuedAttributeDefinitionBuilder(
            ScimTypeDefinitionBuilder<T> scimTypeDefinitionBuilder,
            PropertyDescriptor descriptor)
            : base (scimTypeDefinitionBuilder, descriptor)
        {
        }

        public ScimTypeMultiValuedAttributeDefinitionBuilder<T, TMultiValuedAttribute> SetDescription(string description)
        {
            Description = description;
            return this;
        }

        public ScimTypeMultiValuedAttributeDefinitionBuilder<T, TMultiValuedAttribute> SetMutability(Mutable mutability)
        {
            Mutability = mutability;
            return this;
        }

        public ScimTypeMultiValuedAttributeDefinitionBuilder<T, TMultiValuedAttribute> SetReturned(Return returned)
        {
            Returned = returned;
            return this;
        }

        public ScimTypeMultiValuedAttributeDefinitionBuilder<T, TMultiValuedAttribute> SetUniqueness(Unique uniqueness)
        {
            Uniqueness = uniqueness;
            return this;
        }

        public ScimTypeMultiValuedAttributeDefinitionBuilder<T, TMultiValuedAttribute> ForSubAttributes(Action<ScimTypeDefinitionBuilder<TMultiValuedAttribute>> mva)
        {
            var typeBuilder = new ScimTypeDefinitionBuilder<TMultiValuedAttribute>(
                ScimTypeDefinitionBuilder.ScimServerConfiguration);

            mva(typeBuilder);

            _SubAttributeDefinitions.AddRange(typeBuilder.MemberDefinitions.Values);

            return this;
        }
    }
}