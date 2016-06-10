namespace Owin.Scim.Tests.Integration.CustomResourceTypes
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Configuration;

    using Endpoints;

    [RoutePrefix("tenants")]
    public class TenantsController : ScimControllerBase
    {
        private static readonly IDictionary<string, Tenant> _DB =
            new Dictionary<string, Tenant>();

        public TenantsController(ScimServerConfiguration serverConfiguration) 
            : base(serverConfiguration)
        {
        }

        [Route("{tenantId}", Name = "GetTenant")]
        public Task<HttpResponseMessage> Get(string tenantId)
        {
            Tenant tenant;
            if (_DB.TryGetValue(tenantId, out tenant))
                return Task.FromResult(
                    Request.CreateResponse(
                        HttpStatusCode.OK,
                        SetMetaLocation(tenant, "GetTenant", new { tenantId = tenant.Id })));

            return Task.FromResult(Request.CreateResponse(HttpStatusCode.NotFound));
        }

        [Route(Name = "CreateTenant")]
        public Task<HttpResponseMessage> Post(Tenant tenant)
        {
            tenant.Id = Guid.NewGuid().ToString("N");
            _DB.Add(tenant.Id, tenant);

            return Task.FromResult(
                Request.CreateResponse(HttpStatusCode.Created, SetMetaLocation(tenant, "GetTenant", new { tenantId = tenant.Id })));
        }
    }
}