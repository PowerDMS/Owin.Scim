namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Controllers;
    using System.Web.Http.Metadata;
    
    using Newtonsoft.Json.Linq;

    using ErrorHandling;

    using Extensions;

    using Model;

    using Newtonsoft.Json;

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
            
            actionContext.ActionArguments[Descriptor.ParameterName] =
                JsonConvert.DeserializeObject(
                    jsonString,
                    schemaType,
                    Descriptor
                        .Configuration
                        .Formatters
                        .JsonFormatter
                        .SerializerSettings);
        }
    }
}