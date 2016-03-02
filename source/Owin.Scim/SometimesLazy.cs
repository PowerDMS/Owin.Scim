namespace Owin.Scim
{
    using System;

    public class SometimesLazy<T>
    {
        private readonly Lazy<T> _LazyValue;

        private readonly T _UnlazyValue;
         
        public SometimesLazy(T value)
        {
            _UnlazyValue = value;
        }

        public SometimesLazy(Func<T> valueFactory)
        {
            _LazyValue = new Lazy<T>(valueFactory);
        }

        public bool IsValueCreated
        {
            get
            {
                return _LazyValue == null
                    ? true
                    : _LazyValue.IsValueCreated;
            }
        }

        public T Value
        {
            get
            {
                return _LazyValue == null
                    ? _UnlazyValue
                    : _LazyValue.Value;
            }
        }
    }
}