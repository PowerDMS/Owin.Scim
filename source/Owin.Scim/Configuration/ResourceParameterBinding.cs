namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Controllers;
    using System.Web.Http.Metadata;
    
    using Newtonsoft.Json.Linq;

    using ErrorHandling;

    using Extensions;

    using Model;

    using Newtonsoft.Json;

    using Services;

    public class ResourceParameterBinding : HttpParameterBinding
    {
        private static readonly ConcurrentDictionary<Type, IEnumerable<string>> _RequiredResourceExtensionCache =
            new ConcurrentDictionary<Type, IEnumerable<string>>(); 

        private static readonly HttpMethod _Patch = new HttpMethod("patch");

        private readonly ISchemaTypeFactory _SchemaTypeFactory;

        public ResourceParameterBinding(
            HttpParameterDescriptor parameter,
            ISchemaTypeFactory schemaTypeFactory) 
            : base(parameter)
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

            // Enforce the request contains all required extensions for the resource.
            var resourceTypeDefinition = (IScimResourceTypeDefinition)ScimServerConfiguration.GetScimTypeDefinition(schemaType);
            var requiredExtensions = _RequiredResourceExtensionCache.GetOrAdd(resourceTypeDefinition.DefinitionType, resourceType => resourceTypeDefinition.SchemaExtensions.Where(e => e.Required).Select(e => e.Schema));
            if (requiredExtensions.Any())
            {
                foreach (var requiredExtension in requiredExtensions)
                {
                    if (jsonData[requiredExtension] == null)
                    {
                        // the request will be cut short by ModelBindingResponseAttribute and the response below will be returned
                        SetValue(actionContext, null);
                        actionContext.Response = actionContext.Request.CreateResponse(
                            HttpStatusCode.BadRequest,
                            new ScimError(
                                HttpStatusCode.BadRequest,
                                ScimErrorType.InvalidValue,
                                string.Format(
                                    "'{0}' is a required extension for this resource type '{1}'. The extension must be specified in the request content.", 
                                    requiredExtension, 
                                    ScimServerConfiguration.GetSchemaIdentifierForResourceType(schemaType))));
                        return;
                    }
                }
            }
            
            // When no attributes are specified for projection, the response should contain any attributes whose 
            // attribute definition Returned is equal to Returned.Request
            if (actionContext.Request.Method == HttpMethod.Post ||
                actionContext.Request.Method == _Patch ||
                actionContext.Request.Method == HttpMethod.Put)
            {
                var queryOptions = AmbientRequestMessageService.QueryOptions;
                if (!queryOptions.Attributes.Any())
                {
                    // TODO: (DG) if no attributes have been specified, fill the attributes artificially with jsonData keys for attributes defined as Returned.Request
                }
            }
            
            var resource = JsonConvert.DeserializeObject(
                    jsonString,
                    schemaType,
                    Descriptor
                        .Configuration
                        .Formatters
                        .JsonFormatter
                        .SerializerSettings);

            SetValue(actionContext, resource);
        }
    }
}