namespace Owin.Scim.Configuration
{
    using System;
    using System.ComponentModel;

    public class ScimTypeScalarMemberDefinitionBuilder<T, TMember> 
        : ScimTypeMemberDefinitionBuilder<T, TMember>
        where TMember : IConvertible
    {
        public ScimTypeScalarMemberDefinitionBuilder(
            ScimTypeDefinitionBuilder<T> scimTypeDefinitionBuilder,
            PropertyDescriptor descriptor)
            : base(scimTypeDefinitionBuilder, descriptor)
        {
        }
        
        public ScimTypeScalarMemberDefinitionBuilder<T, TMember> SetDescription(string description)
        {
            Description = description;
            return this;
        }

        public ScimTypeScalarMemberDefinitionBuilder<T, TMember> SetMutability(Mutable mutability)
        {
            Mutability = mutability;
            return this;
        }

        public ScimTypeScalarMemberDefinitionBuilder<T, TMember> SetReturned(Return returned)
        {
            Returned = returned;
            return this;
        }

        public ScimTypeScalarMemberDefinitionBuilder<T, TMember> SetUniqueness(Unique uniqueness)
        {
            Uniqueness = uniqueness;
            return this;
        }

        public ScimTypeScalarMemberDefinitionBuilder<T, TMember> SetCaseExact(bool caseExact)
        {
            CaseExact = caseExact;
            return this;
        }
    }
}