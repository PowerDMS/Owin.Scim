namespace Owin.Scim.Model
{
    using Configuration;

    public class ScimFeatureFilterDefinition : ScimTypeDefinitionBuilder<ScimFeatureFilter>
    {
        public ScimFeatureFilterDefinition()
        {
            For(f => f.Supported)
                .SetDescription(@"A boolean value specifying whether or not the operation is supported.")
                .SetRequired(true)
                .SetMutability(Mutability.ReadOnly);

            For(f => f.MaxResults)
                .SetDescription(@"An integer value specifying the maximum number of resources returned in a response.")
                .SetRequired(true)
                .SetMutability(Mutability.ReadOnly);
        }
    }
}