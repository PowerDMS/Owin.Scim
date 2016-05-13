namespace Owin.Scim.Security
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    using Configuration;

    using Model;

    public class ScimAuthorizationAttribute : AuthorizationFilterAttribute
    {
        private readonly ScimServerConfiguration _ServerConfiguration;

        public ScimAuthorizationAttribute(ScimServerConfiguration serverConfiguration)
        {
            _ServerConfiguration = serverConfiguration;
        }

        public override bool AllowMultiple
        {
            get { return false; }
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext == null)
                throw new ArgumentNullException("actionContext");

            if (SkipAuthorization(actionContext))
                return;

            if (!IsAuthorized(actionContext))
                HandleUnauthorizedRequest(actionContext);
        }

        protected virtual bool IsAuthorized(HttpActionContext actionContext)
        {
            if (actionContext == null)
                throw new ArgumentNullException("actionContext");

            var user = actionContext.ControllerContext.RequestContext.Principal;
            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
                return false;

            return true;
        }

        protected virtual void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            if (actionContext == null)
                throw new ArgumentNullException("actionContext");

            actionContext.Response = actionContext
                .ControllerContext
                .Request
                .CreateResponse(
                    HttpStatusCode.Unauthorized, 
                    new ScimError(HttpStatusCode.Unauthorized, detail: "Authorization failure. The authorization header is invalid or missing."));
        }

        private bool SkipAuthorization(HttpActionContext actionContext)
        {
            return 
                !_ServerConfiguration.EnableEndpointAuthorization || 
                actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any() || 
                actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }
    }
}