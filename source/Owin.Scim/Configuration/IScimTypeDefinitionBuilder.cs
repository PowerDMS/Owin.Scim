namespace Owin.Scim.Configuration
{
    using System;

    public interface IScimTypeDefinitionBuilder
    {
        Type ResourceType { get; }
    }
}