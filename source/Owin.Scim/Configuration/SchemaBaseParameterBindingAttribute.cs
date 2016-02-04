namespace Owin.Scim.Configuration
{
    using System;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    
    public class SchemaBaseParameterBindingAttribute : ParameterBindingAttribute
    {
        public override HttpParameterBinding GetBinding(HttpParameterDescriptor parameter)
        {
            if (parameter == null)
            {
                throw new Exception("parameter");
            }

            if (parameter.Configuration.DependencyResolver == null)
            {
                return null;
            }

            return new SchemaBaseParameterBinding(
                parameter,
                parameter.Configuration.DependencyResolver.GetService(typeof(ISchemaTypeFactory)) as ISchemaTypeFactory);
        }
    }
}