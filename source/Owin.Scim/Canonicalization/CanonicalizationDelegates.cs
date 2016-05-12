namespace Owin.Scim.Canonicalization
{
    using Configuration;

    public delegate void CanonicalizationAction<in T>(T value);

    public delegate void AttributeCanonicalizationAction<in T>(T value, IScimTypeAttributeDefinition attributeDefinition);

    public delegate void StatefulCanonicalizationAction<in T>(T value, ref object state);

    public delegate void StatefulAttributeCanonicalizationAction<in T>(T value, IScimTypeAttributeDefinition attributeDefinition, ref object state);

    public delegate T CanonicalizationFunc<T>(T value);

    public delegate T AttributeCanonicalizationFunc<T>(T value, IScimTypeAttributeDefinition attributeDefinition);

    public delegate T StatefulCanonicalizationFunc<T>(T value, ref object state);

    public delegate T StatefulAttributeCanonicalizationFunc<T>(T value, IScimTypeAttributeDefinition attributeDefinition, ref object state);
}