namespace Owin.Scim.Configuration
{
    using System.ComponentModel.Composition;

    using NContext.Configuration;

    public class CompositionCatalogRegisrar : IApplicationComponent
    {
        private readonly ScimServerConfiguration _ServerConfiguration;

        public CompositionCatalogRegisrar(ScimServerConfiguration serverConfiguration)
        {
            _ServerConfiguration = serverConfiguration;
        }

        public void Configure(ApplicationConfigurationBase applicationConfiguration)
        {
            if (IsConfigured)
                return;

            // Compose app container
            applicationConfiguration.CompositionContainer
                .ComposeExportedValue(_ServerConfiguration);

            IsConfigured = true;
        }

        public bool IsConfigured { get; private set; }
    }
}