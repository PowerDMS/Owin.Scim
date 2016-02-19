namespace Owin.Scim.Configuration
{
    using Model;

    public delegate void ComplexCanonicalizationRule<in T>(T attribute, ref object state);

    public delegate T CanonicalizationFunc<T>(T value);

    public delegate void CanonicalizationAction<T>(T value);

    public delegate void StatefulCanonicalizationAction<T, TState>(T value, ref TState state);
}