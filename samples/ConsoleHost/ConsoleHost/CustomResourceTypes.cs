namespace ConsoleHost
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using FluentValidation;

    using Owin.Scim.Configuration;
    using Owin.Scim.Endpoints;
    using Owin.Scim.ErrorHandling;
    using Owin.Scim.Model;
    using Owin.Scim.Validation;

    public class Tenant : Resource
    {
        public Tenant()
        {
            Meta = new ResourceMetadata("Tenant");
        }

        public override string SchemaIdentifier
        {
            get { return "urn:custom:schemas:Tenant"; }
        }

        public string Name { get; set; }
    }

    [RoutePrefix("tenants")]
    public class TenantsController : ScimControllerBase
    {
        private static readonly IDictionary<string, Tenant> _DB = 
            new Dictionary<string, Tenant>();

        public TenantsController(ScimServerConfiguration serverConfiguration) 
            : base(serverConfiguration)
        {
        }

        [Route(Name = "CreateTenant")]
        public Task<HttpResponseMessage> Post(Tenant tenant)
        {
            tenant.Id = Guid.NewGuid().ToString("N");
            _DB.Add(tenant.Id, tenant);

            return Task.FromResult(Request.CreateResponse(HttpStatusCode.Created, tenant));
        }

        [Route("{tenantId}", Name = "GetTenant")]
        public Task<HttpResponseMessage> Get(string tenantId)
        {
            Tenant tenant;
            if (_DB.TryGetValue(tenantId, out tenant))
                return Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, tenant));

            return Task.FromResult(Request.CreateResponse(HttpStatusCode.NotFound));
        }
    }

    public class TenantValidator : ResourceValidatorBase<Tenant>
    {
        public TenantValidator(
            ScimServerConfiguration serverConfiguration,
            ResourceExtensionValidators extensionValidators) 
            : base(serverConfiguration, extensionValidators)
        {
        }

        protected override void ConfigureDefaultRuleSet()
        {
            RuleFor(tenant => tenant.Name)
                .NotEmpty()
                .WithState(t =>
                    new ScimError(
                        HttpStatusCode.BadRequest,
                        ScimErrorType.InvalidValue,
                        ScimErrorDetail.AttributeRequired("name")));
        }

        protected override void ConfigureCreateRuleSet()
        {
        }

        protected override void ConfigureUpdateRuleSet()
        {
        }
    }

    public class TenantDefinition : ScimResourceTypeDefinitionBuilder<Tenant>
    {
        public TenantDefinition(ScimServerConfiguration serverConfiguration) 
            : base(
                  serverConfiguration, 
                  "Tenant", 
                  "urn:custom:schemas:Tenant", 
                  "tenants", 
                  typeof(TenantValidator), 
                  schemaIdentifiers => schemaIdentifiers.Contains("urn:custom:schemas:Tenant"))
        {
            For(tenant => tenant.Name)
                .SetRequired(true)
                .SetDescription("The name of the tenant.");
        }
    }
}