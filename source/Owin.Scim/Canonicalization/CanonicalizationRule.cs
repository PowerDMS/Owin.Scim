namespace Owin.Scim.Canonicalization
{
    using System.ComponentModel;

    using Extensions;

    public class CanonicalizationRule<TAttribute> : ICanonicalizationRule
    {
        private readonly PropertyDescriptor _PropertyDescriptor;

        private readonly StatefulCanonicalizationFunc<TAttribute> _CanonicalizationRule;

        public CanonicalizationRule(
            PropertyDescriptor propertyDescriptor,
            StatefulCanonicalizationFunc<TAttribute> canonicalizationRule)
        {
            _PropertyDescriptor = propertyDescriptor;
            _CanonicalizationRule = canonicalizationRule;
        }

        public void Execute(object instance, ref object state)
        {
            if (_PropertyDescriptor.PropertyType.IsTerminalObject())
            {
                var currentValue = (TAttribute) _PropertyDescriptor.GetValue(instance);
                _PropertyDescriptor.SetValue(instance, _CanonicalizationRule(currentValue, ref state));
            }
            else
            {
                _CanonicalizationRule((TAttribute)instance, ref state);
            }
        }
    }
}