namespace Owin.Scim.Extensions
{
    using System;

    public static class DelegateExtensions
    {
        public static Func<T, T2> AsFunc<T, T2>(this Delegate func)
        {
            return (Func<T, T2>)func;
        }
    }
}