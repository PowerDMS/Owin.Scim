namespace Owin.Scim.Canonicalization
{
    public delegate void CanonicalizationAction<in T>(T value);

    public delegate void StatefulCanonicalizationAction<in T>(T value, ref object state);

    public delegate T CanonicalizationFunc<T>(T value);

    public delegate T StatefulCanonicalizationFunc<T>(T value, ref object state);
}