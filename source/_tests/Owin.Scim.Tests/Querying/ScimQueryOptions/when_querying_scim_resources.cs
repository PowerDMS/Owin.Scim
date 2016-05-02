namespace Owin.Scim.Tests.Querying.ScimQueryOptions
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Web.Http.Controllers;

    using Configuration;

    using Machine.Specifications;

    using Scim.Querying;

    public class when_querying_scim_resources
    {
        Establish context = () => ServerConfiguration = new ScimServerConfiguration();

        Because of = () => QueryOptions = QueryOptionsFactory();

        protected static Func<ScimQueryOptions> QueryOptionsFactory;

        protected static ScimQueryOptions QueryOptions;

        protected static ScimServerConfiguration ServerConfiguration;
    }
}