namespace Owin.Scim.Tests.Extensions
{
    using System;
    using System.ComponentModel.Composition.Hosting;
    using System.Reflection;

    using Configuration;

    using NContext.Common;
    using NContext.Extensions;

    using v2.Model;

    public static class ScimServerConfigurationExtensions
    {
        public static ScimServerConfiguration WithTypeDefinitions(this ScimServerConfiguration serverConfiguration)
        {
            var compositionContainer =
                new CompositionContainer(
                    new AggregateCatalog(
                        new AssemblyCatalog(Assembly.GetExecutingAssembly()),
                        new AssemblyCatalog(Assembly.GetAssembly(typeof(ScimServerConfiguration))),
                        new AssemblyCatalog(Assembly.GetAssembly(typeof(ScimUser2)))));

            // discover and register all type definitions
            var owinScimAssembly = Assembly.GetExecutingAssembly();
            var typeDefinitions = compositionContainer.GetExportTypesThatImplement<IScimTypeDefinition>();
            foreach (var typeDefinition in typeDefinitions)
            {
                Type distinctTypeDefinition;
                var typeDefinitionTarget = GetTargetDefinitionType(typeDefinition); // the type of object being defined (e.g. User, Group, Name)
                if (serverConfiguration.TypeDefinitionRegistry.TryGetValue(typeDefinitionTarget, out distinctTypeDefinition))
                {
                    // already have a definition registered for the target type
                    // let's favor non-Owin.Scim definitions over built-in defaults
                    if (distinctTypeDefinition.Assembly == owinScimAssembly && typeDefinition.Assembly != owinScimAssembly)
                        serverConfiguration.TypeDefinitionRegistry[typeDefinitionTarget] = typeDefinition;

                    continue;
                }

                // register type definition
                serverConfiguration.TypeDefinitionRegistry[typeDefinitionTarget] = typeDefinition;
            }

            var enumerator = serverConfiguration.TypeDefinitionRegistry.Values.GetEnumerator();
            while (enumerator.MoveNext())
            {
                // creating type definitions may be expensive due to reflection
                // when a type definition is instantiated, it may implicitly instantiate/register other type 
                // definitions for complex attributes, therefore, no need to re-create the same definition more than once
                if (serverConfiguration.ContainsTypeDefinition(enumerator.Current)) continue;

                var typeDefinition = (IScimTypeDefinition)enumerator.Current.CreateInstance(serverConfiguration);
                serverConfiguration.AddTypeDefiniton(typeDefinition);
            }

            return serverConfiguration;
        }

        private static Type GetTargetDefinitionType(Type typeDefinition)
        {
            Type genericTypeDefinition;
            var baseType = typeDefinition.BaseType;
            if (baseType == null)
                throw new Exception("Invalid type defintion. Must inherit from either ScimResourceTypeDefinitionBuilder or ScimTypeDefinitionBuilder.");

            while (!baseType.IsGenericType ||
                (((genericTypeDefinition = baseType.GetGenericTypeDefinition()) != typeof(ScimResourceTypeDefinitionBuilder<>)) &&
                 genericTypeDefinition != typeof(ScimTypeDefinitionBuilder<>)))
            {
                if (baseType.BaseType == null)
                    throw new Exception("Invalid type defintion. Must inherit from either ScimResourceTypeDefinitionBuilder or ScimTypeDefinitionBuilder.");

                baseType = baseType.BaseType;
            }

            return baseType.GetGenericArguments()[0];
        }
    }
}