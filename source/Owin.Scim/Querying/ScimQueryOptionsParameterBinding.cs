namespace Owin.Scim.Querying
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Controllers;
    using System.Web.Http.Metadata;

    using Configuration;

    using Extensions;

    public class ScimQueryOptionsParameterBinding : HttpParameterBinding
    {
        private readonly ScimServerConfiguration _ServerConfiguration;

        public ScimQueryOptionsParameterBinding(
            HttpParameterDescriptor descriptor,
            ScimServerConfiguration serverConfiguration) 
            : base(descriptor)
        {
            _ServerConfiguration = serverConfiguration;
        }

        public override async Task ExecuteBindingAsync(ModelMetadataProvider metadataProvider, HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            ScimQueryOptions queryOptions;
            
            if (actionContext.Request.Method == HttpMethod.Get)
                queryOptions = actionContext.Request.GetOwinContext().Request.Query.GetScimQueryOptions(_ServerConfiguration);
            else if (actionContext.Request.Method == HttpMethod.Post)
                queryOptions = await actionContext.Request.Content.ReadAsAsync<ScimQueryOptions>(cancellationToken);
            else
                throw new InvalidOperationException("You can only bind ScimQueryOptions on GET or POST query endpoints.");

            SetValue(actionContext, queryOptions);
        }
    }
}