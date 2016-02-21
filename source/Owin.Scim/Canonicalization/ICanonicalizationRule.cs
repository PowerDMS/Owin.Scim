namespace Owin.Scim.Canonicalization
{
    public interface ICanonicalizationRule
    {
        void Execute(object instance, ref object state);
    }
}