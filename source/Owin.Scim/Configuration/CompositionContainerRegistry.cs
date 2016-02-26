namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;

    using NContext.Configuration;

    public class CompositionContainerRegistry : IApplicationComponent
    {
        private readonly IEnumerable<object> _Entries;

        public CompositionContainerRegistry(IEnumerable<object> entries)
        {
            _Entries = entries;
        }

        public void Configure(ApplicationConfigurationBase applicationConfiguration)
        {
            if (IsConfigured)
                return;

            // Compose app container
            var batch = new CompositionBatch();
            foreach (var entry in _Entries)
            {
                var entryType = entry.GetType();
                var contractName = AttributedModelServices.GetContractName(entryType);
                var typeIdentity = AttributedModelServices.GetTypeIdentity(entryType);
                var metadata = new Dictionary<string, object> { { "ExportTypeIdentity", typeIdentity } };
                batch.AddExport(new Export(contractName, metadata, () => entry));
            }

            applicationConfiguration.CompositionContainer.Compose(batch);

            IsConfigured = true;
        }

        public bool IsConfigured { get; private set; }
    }
}