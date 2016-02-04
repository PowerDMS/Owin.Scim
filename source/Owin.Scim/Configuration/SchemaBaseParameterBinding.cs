namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Controllers;
    using System.Web.Http.Metadata;
    
    using Newtonsoft.Json.Linq;

    using Patching.Helpers;

    public class SchemaBaseParameterBinding : HttpParameterBinding
    {
        private readonly ISchemaTypeFactory _SchemaTypeFactory;

        public SchemaBaseParameterBinding(
            HttpParameterDescriptor parameter,
            ISchemaTypeFactory schemaTypeFactory) : base(parameter)
        {
            _SchemaTypeFactory = schemaTypeFactory;
        }

        public override async Task ExecuteBindingAsync(
            ModelMetadataProvider metadataProvider,
            HttpActionContext actionContext,
            CancellationToken cancellationToken)
        {
            var jsonBytes = await actionContext.Request.Content.ReadAsByteArrayAsync();

            var jsonReader = Descriptor
                .Configuration
                .Formatters
                .JsonFormatter
                .CreateJsonReader(typeof(IDictionary<string, object>), new MemoryStream(jsonBytes), Encoding.UTF8);

            var serializer = Descriptor
                .Configuration
                .Formatters
                .JsonFormatter
                .CreateJsonSerializer();

            var dictionary = serializer
                .Deserialize<IDictionary<string, object>>(jsonReader);

            if (!dictionary.ContainsCaseInsensitiveKey(ScimConstants.Schemas.Key))
                throw new Exception(""); // TODO: (DG) invalid scim message

            var schemasValue = dictionary.GetValueForCaseInsensitiveKey(ScimConstants.Schemas.Key);
            if (schemasValue == null)
                throw new Exception(""); // TODO: (DG) no schemas specified
            
            var schemaType = _SchemaTypeFactory.GetSchemaType(((JArray)schemasValue).ToObject<ISet<string>>());
            var schemaReader = Descriptor
                .Configuration
                .Formatters
                .JsonFormatter
                .CreateJsonReader(schemaType, new MemoryStream(jsonBytes), Encoding.UTF8);
            
            actionContext.ActionArguments[Descriptor.ParameterName] = serializer.Deserialize(schemaReader, schemaType);
        }
    }
}