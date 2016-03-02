namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Controllers;
    using System.Web.Http.Metadata;
    
    using Newtonsoft.Json.Linq;

    using ErrorHandling;

    using Extensions;

    using Model;

    using NContext.Common;

    using Newtonsoft.Json;

    public class ResourceParameterBinding : HttpParameterBinding
    {
        private readonly ScimServerConfiguration _ServerConfiguration;

        private readonly ISchemaTypeFactory _SchemaTypeFactory;

        public ResourceParameterBinding(
            HttpParameterDescriptor parameter,
            ScimServerConfiguration serverConfiguration,
            ISchemaTypeFactory schemaTypeFactory) 
            : base(parameter)
        {
            _ServerConfiguration = serverConfiguration;
            _SchemaTypeFactory = schemaTypeFactory;
        }

        public override async Task ExecuteBindingAsync(
            ModelMetadataProvider metadataProvider,
            HttpActionContext actionContext,
            CancellationToken cancellationToken)
        {
            var jsonString = await actionContext.Request.Content.ReadAsStringAsync();

            var jsonData = JObject.Parse(jsonString);
            var schemasKey = jsonData.FindKeyCaseInsensitive(ScimConstants.Schemas.Key);
            if (schemasKey == null)
            {
                throw new ScimError(System.Net.HttpStatusCode.BadRequest,
                    ScimErrorType.InvalidValue,
                    ErrorDetail.AttributeRequired(ScimConstants.Schemas.Key))
                    .ToResponseException();
            }

            var schemasValue = jsonData[schemasKey];
            if (schemasValue == null)
                throw new Exception(""); // TODO: (DG) no schemas specified
            
            var schemaType = _SchemaTypeFactory.GetSchemaType(((JArray)schemasValue).ToObject<ISet<string>>());
            if (!Descriptor.ParameterType.IsAssignableFrom(schemaType))
                throw new Exception(""); // TODO: (DG) binding rules resolved to a type which is not assignable to the action parameter's type

            //_ServerConfiguration.GetScimResourceTypeDefinition(schemaType)
            
            var resource = JsonConvert.DeserializeObject(
                    jsonString,
                    schemaType,
                    Descriptor
                        .Configuration
                        .Formatters
                        .JsonFormatter
                        .SerializerSettings);

            actionContext.ActionArguments[Descriptor.ParameterName] = resource;
        }
    }
}