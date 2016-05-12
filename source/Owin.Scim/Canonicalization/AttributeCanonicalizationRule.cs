namespace Owin.Scim.Canonicalization
{
    using System.ComponentModel;

    using Configuration;

    using Extensions;

    public class AttributeCanonicalizationRule<TAttribute> : ICanonicalizationRule
    {
        private readonly PropertyDescriptor _PropertyDescriptor;

        private readonly IScimTypeAttributeDefinition _Definition;

        private readonly AttributeCanonicalizationFunc<TAttribute> _CanonicalizationRule;

        public AttributeCanonicalizationRule(
            PropertyDescriptor propertyDescriptor,
            IScimTypeAttributeDefinition definition,
            AttributeCanonicalizationAction<TAttribute> canonicalizationRule)
        {
            _PropertyDescriptor = propertyDescriptor;
            _Definition = definition;
            _CanonicalizationRule = (TAttribute value, IScimTypeAttributeDefinition attributeDefinition) =>
            {
                canonicalizationRule.Invoke(value, attributeDefinition);
                return value;
            };
        }

        public AttributeCanonicalizationRule(
            PropertyDescriptor propertyDescriptor,
            IScimTypeAttributeDefinition definition,
            AttributeCanonicalizationFunc<TAttribute> canonicalizationRule)
        {
            _PropertyDescriptor = propertyDescriptor;
            _Definition = definition;
            _CanonicalizationRule = canonicalizationRule;
        }

        public void Execute(object instance, ref object state)
        {
            if (_PropertyDescriptor.PropertyType.IsTerminalObject())
            {
                var currentValue = (TAttribute)_PropertyDescriptor.GetValue(instance);
                _PropertyDescriptor.SetValue(instance, _CanonicalizationRule(currentValue, _Definition));
            }
            else
            {
                _CanonicalizationRule((TAttribute)instance, _Definition);
            }
        }
    }
}