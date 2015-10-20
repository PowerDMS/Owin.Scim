namespace Owin.Scim.Configuration
{
    using System.ComponentModel.Composition;

    using DryIoc;

    [InheritedExport]
    public interface IConfigureDryIoc
    {
        int Priority { get; }

        void ConfigureContainer(IContainer container);
    }
}