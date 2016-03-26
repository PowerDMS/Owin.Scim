namespace Owin.Scim.Model
{
    using System;

    using Configuration;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ScimTypeDefinitionAttribute : Attribute
    {
        public ScimTypeDefinitionAttribute(Type definitionType)
        {
            if (!typeof(IScimTypeDefinition).IsAssignableFrom(definitionType))
                throw new ArgumentException("DefinitionType must be of type ScimTypeDefinition.");

            DefinitionType = definitionType;
        }

        public Type DefinitionType { get; private set; }
    }
}