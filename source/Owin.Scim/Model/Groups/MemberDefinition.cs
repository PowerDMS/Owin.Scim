namespace Owin.Scim.Model.Groups
{
    using Configuration;

    public class MemberDefinition : ScimTypeDefinitionBuilder<Member>
    {
        public MemberDefinition()
        {
            For(p => p.Value)
                .SetMutability(Mutability.Immutable);
            For(p => p.Ref)
                .SetMutability(Mutability.Immutable);
            For(p => p.Type)
                .SetMutability(Mutability.Immutable);
        }
    }
}