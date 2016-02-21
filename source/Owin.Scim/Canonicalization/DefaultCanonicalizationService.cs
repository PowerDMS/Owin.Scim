namespace Owin.Scim.Canonicalization
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Configuration;

    using Extensions;

    public class DefaultCanonicalizationService
    {
        public void Canonicalize(object instance, IScimTypeDefinition typeDefinition)
        {
            foreach (var attribute in typeDefinition.AttributeDefinitions)
            {
                var attributeDescriptor = attribute.Key;
                var attributeDefinition = attribute.Value;

                var canonicalizationRules = attributeDefinition.GetCanonicalizationRules();
                if (canonicalizationRules == null || !canonicalizationRules.Any()) continue;

                var attributeValue = attributeDescriptor.GetValue(instance);
                if (attributeValue == null || attributeValue == attributeDescriptor.PropertyType.GetDefaultValue()) continue;

                object state = null;
                if (attributeDescriptor.PropertyType.IsTerminalObject())
                {
                    foreach (var canonicalizationRule in canonicalizationRules)
                    {
                        canonicalizationRule.Execute(instance, ref state);
                    }

                    continue;
                }

                if (!attributeDefinition.MultiValued)
                {
                    // attribute is complex
                    foreach (var canonicalizationRule in canonicalizationRules)
                    {
                        canonicalizationRule.Execute(instance, ref state);
                    }

                    // canonicalize the complex object instance as it may have sub-attributes 
                    // with defined canonicalization rules
                    Canonicalize(attributeDescriptor.GetValue(instance), attributeDefinition.TypeDefinitionBuilder);

                    continue;
                }
                
                // attribute is multi-valued (enumerable)
                var stateCache = new Dictionary<ICanonicalizationRule, object>();
                var enumerable = (IEnumerable)attributeValue;
                var enumerator = enumerable.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current == null) continue;

                    foreach (var canonicalizationRule in canonicalizationRules)
                    {
                        if (!stateCache.ContainsKey(canonicalizationRule))
                            stateCache[canonicalizationRule] = null;

                        var rState = stateCache[canonicalizationRule];
                        var currentElement = enumerator.Current;
                        canonicalizationRule.Execute(currentElement, ref rState);
                        stateCache[canonicalizationRule] = rState;
                    }

                    // recursively canonicalize each enumeration value as they may have sub-attributes with canonicalization rules
                    Canonicalize(enumerator.Current, attributeDefinition.TypeDefinitionBuilder);
                }
            }
        }
    }
}