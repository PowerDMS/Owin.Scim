namespace Owin.Scim.Tests.Integration.Schemas
{
    using System.Collections.Generic;
    using System.Net.Http;

    using Machine.Specifications;

    using Model;

    using Newtonsoft.Json.Linq;

    using v2.Model;

    public class when_retrieving_schemas : using_a_scim_server
    {
        Because of = async () =>
        {
            Response = await Server.HttpClient.GetAsync("v2/schemas/" + SchemaId ?? "").Await().AsTask;

            if (Response.IsSuccessStatusCode)
            {
                var json = await Response.Content.ReadAsStringAsync();
                if (SchemaId == null)
                {
                    Schemas = await Response.Content.ReadAsAsync<IEnumerable<ScimSchema2>>();
                    JsonData = JArray.Parse(json);
                }
                else
                {
                    Schemas = new List<ScimSchema> { await Response.Content.ReadAsAsync<ScimSchema2>() };
                    JsonData = new JArray();
                    JsonData.Add(JObject.Parse(json));
                }
            }
        };

        protected static string SchemaId;

        protected static HttpResponseMessage Response;

        protected static IEnumerable<ScimSchema> Schemas;

        protected static JArray JsonData;
    }
}