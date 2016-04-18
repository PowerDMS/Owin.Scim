namespace Owin.Scim.Model
{
    using Configuration;

    public class ScimFeatureBulkDefinition : ScimTypeDefinitionBuilder<ScimFeatureBulk>
    {
        public ScimFeatureBulkDefinition()
        {
            For(b => b.Supported)
                .SetDescription(@"A boolean value specifying whether or not the operation is supported.")
                .SetRequired(true)
                .SetMutability(Mutability.ReadOnly);

            For(b => b.MaxOperations)
                .SetDescription(@"An integer value specifying the maximum number of operations.")
                .SetRequired(true)
                .SetMutability(Mutability.ReadOnly);

            For(b => b.MaxPayloadSize)
                .SetDescription(@"An integer value specifying the maximum payload size in bytes.")
                .SetRequired(true)
                .SetMutability(Mutability.ReadOnly);

        }
    }
}