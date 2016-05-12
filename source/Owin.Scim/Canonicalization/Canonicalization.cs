namespace Owin.Scim.Canonicalization
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Configuration;

    using Extensions;

    using Model;

    using NContext.Common;

    using Services;

    public static class Canonicalization
    {
        public static void Lowercase<T>(T attribute, Expression<Func<T, string>> expression)
        {
            var mE = expression.Body as MemberExpression;
            if (mE == null) throw new InvalidOperationException("Expression body must be a MemberExpression to an attribute's string property.");

            var pI = mE.Member as PropertyInfo;
            if (pI == null) throw new InvalidOperationException("Expression body member must be an attribute's string property.");

            var value = pI.GetValue(attribute) as string;

            if (string.IsNullOrWhiteSpace(value)) return;

            pI.SetValue(attribute, value.ToLower());
        }

        public static void EnforceSinglePrimaryAttribute(MultiValuedAttribute attribute, ref object state)
        {
            bool hasPrimary = false;
            if (state != null)
                hasPrimary = (bool)state;

            if (!hasPrimary && attribute.Primary)
            {
                state = true;
            }

            if (hasPrimary && attribute.Primary)
            {
                attribute.Primary = false;
            }
        }

        public static void Canonicalize<T, TProperty>(
            this T source,
            Expression<Func<T, TProperty>> property,
            Func<TProperty, TProperty> canonicalizationFunc)
        {
            source.Canonicalize(property, property, canonicalizationFunc);
        }

        public static void Canonicalize<T, TProperty, TOtherProperty>(
            this T source,
            Expression<Func<T, TProperty>> sourceProperty,
            Expression<Func<T, TOtherProperty>> targetProperty,
            Func<TProperty, TOtherProperty> canonicalizationFunc)
            where TProperty : TOtherProperty
        {
            if (source == null) return;
            if (sourceProperty == null) throw new ArgumentNullException("sourceProperty");
            if (targetProperty == null) throw new ArgumentNullException("targetProperty");
            if (canonicalizationFunc == null) throw new ArgumentNullException("canonicalizationFunc");

            var descriptors = GetDescriptors(sourceProperty, targetProperty);
            var sourceValue = descriptors.Item1.GetValue(source);

            // If sourceValue is null or default for type, just set target to default
            if (sourceValue == descriptors.Item1.PropertyType.GetDefaultValue())
            {
                descriptors.Item2.SetValue(source, sourceValue);
                return;
            }

            var canonicalizedValue = canonicalizationFunc((TProperty)sourceValue);
            descriptors.Item2.SetValue(source, canonicalizedValue);
        }

        private static Tuple<PropertyDescriptor, PropertyDescriptor> GetDescriptors<T, TProperty, TOtherProperty>(
            Expression<Func<T, TProperty>> sourceProperty,
            Expression<Func<T, TOtherProperty>> targetProperty)
        {
            var memberExpression = sourceProperty.Body as MemberExpression;
            if (memberExpression == null)
                throw new InvalidOperationException("sourceProperty must be of type MemberExpression.");

            var sourceDescriptor = TypeDescriptor.GetProperties(typeof(T)).Find(memberExpression.Member.Name, true);

            memberExpression = targetProperty.Body as MemberExpression;
            if (memberExpression == null)
                throw new InvalidOperationException("targetProperty must be of type MemberExpression.");

            var targetDescriptor = TypeDescriptor.GetProperties(typeof(T)).Find(memberExpression.Member.Name, true);

            return Tuple.Create(sourceDescriptor, targetDescriptor);
        }

        public static Uri EnforceScimUri(Uri uri, IScimTypeAttributeDefinition attributeDefinition, ScimServerConfiguration serverConfiguration)
        {
            /* 
             SCIM 2.0 Specification
             referenceTypes  A multi-valued array of JSON strings that indicate
                             the SCIM resource types that may be referenced.  Valid values
                             are as follows:

                             +  A SCIM resource type (e.g., "User" or "Group"),

                             +  "external" - indicating that the resource is an external
                                resource (e.g., a photo), or

                             +  "uri" - indicating that the reference is to a service
                                endpoint or an identifier (e.g., a schema URN).
            */

            if (attributeDefinition.ReferenceTypes != null && attributeDefinition.ReferenceTypes.Any())
            {
                IScimResourceTypeDefinition resourceDefinition = null;
                foreach (var referenceType in attributeDefinition.ReferenceTypes)
                {
                    if (referenceType.Equals(ScimConstants.ReferenceTypes.External, StringComparison.OrdinalIgnoreCase))
                    {
                        // do not accept relative URIs for an attribute which is defined as an external reference type
                        if (!uri.IsAbsoluteUri)
                            continue;

                        return uri;
                    }

                    if (referenceType.Equals(ScimConstants.ReferenceTypes.Uri, StringComparison.OrdinalIgnoreCase))
                    {
                        if (uri.ToString().StartsWith(ScimConstants.Defaults.URNPrefix, StringComparison.OrdinalIgnoreCase))
                            return uri; // uri is an identifier, possibly schema

                        // uri MUST be a valid SCIM resource
                        resourceDefinition = serverConfiguration
                            .ResourceTypeDefinitions
                            .SingleOrDefault(rtd => uri.AbsolutePath.IndexOf(rtd.Endpoint, StringComparison.OrdinalIgnoreCase) >= 0);

                        if (resourceDefinition != null)
                            break;

                        continue;
                    }

                    // uri MUST be a valid SCIM resource that's within the allowed referenceTypes
                    resourceDefinition = serverConfiguration
                        .ResourceTypeDefinitions
                        .SingleOrDefault(
                            rtd => 
                            rtd.Name.Equals(referenceType, StringComparison.OrdinalIgnoreCase) && 
                            uri.AbsolutePath.IndexOf(rtd.Endpoint, StringComparison.OrdinalIgnoreCase) >= 0);

                    if (resourceDefinition != null)
                        break;
                }

                // invalid URI for the associated reference types
                if (resourceDefinition == null)
                    return uri; // allow validation to handle invalid uri's

                return BuildScimUri(uri);
            }

            /*
                Relative URIs should be resolved as specified in
                Section 5.2 of [RFC3986].  However, the base URI for relative URI
                resolution MUST include all URI components and path segments up to,
                but not including, the Endpoint URI (the SCIM service provider root
                endpoint)
            */
            return BuildScimUri(uri);
        }

        private static Uri BuildScimUri(Uri uri)
        {
            var baseUri = AmbientRequestService.BaseUri;
            var builder = new UriBuilder(baseUri)
            {
                Query = uri.Query,
                Fragment = uri.Fragment
            };

            builder.Path += uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped)
                .ToMaybe()
                .Bind(path =>
                {
                    if (string.IsNullOrWhiteSpace(path))
                        return path.ToMaybe();

                    var basePath = AmbientRequestService.BasePath;
                    if (!string.IsNullOrWhiteSpace(basePath))
                        path = path.Replace(basePath, "");

                    return path
                        .Replace("\\..", "")
                        .Replace("/..", "")
                        .Replace('\\', '/')
                        .ToMaybe();
                })
                .FromMaybe("");

            return builder.Uri;
        }
    }
}