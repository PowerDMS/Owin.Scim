namespace Owin.Scim.Canonicalization
{
    using System.ComponentModel;

    using Configuration;

    using Extensions;

    public class StatefulAttributeCanonicalizationRule<TAttribute> : ICanonicalizationRule
    {
        private readonly PropertyDescriptor _PropertyDescriptor;

        private readonly IScimTypeAttributeDefinition _Definition;

        private readonly StatefulAttributeCanonicalizationFunc<TAttribute> _CanonicalizationRule;

        public StatefulAttributeCanonicalizationRule(
            PropertyDescriptor propertyDescriptor,
            IScimTypeAttributeDefinition definition,
            StatefulAttributeCanonicalizationAction<TAttribute> canonicalizationRule)
        {
            _PropertyDescriptor = propertyDescriptor;
            _Definition = definition;
            _CanonicalizationRule =
                (TAttribute value, IScimTypeAttributeDefinition attributeDefinition, ref object state) =>
                {
                    canonicalizationRule.Invoke(value, attributeDefinition, ref state);
                    return value;
                };
        }

        public StatefulAttributeCanonicalizationRule(
            PropertyDescriptor propertyDescriptor,
            IScimTypeAttributeDefinition definition,
            StatefulAttributeCanonicalizationFunc<TAttribute> canonicalizationRule)
        {
            _PropertyDescriptor = propertyDescriptor;
            _Definition = definition;
            _CanonicalizationRule = canonicalizationRule;
        }

        public void Execute(object instance, ref object state)
        {
            if (_PropertyDescriptor.PropertyType.IsTerminalObject())
            {
                var currentValue = (TAttribute) _PropertyDescriptor.GetValue(instance);
                _PropertyDescriptor.SetValue(instance, _CanonicalizationRule(currentValue, _Definition, ref state));
            }
            else
            {
                _CanonicalizationRule((TAttribute)instance, _Definition, ref state);
            }
        }
    }
}