namespace Owin.Scim.Tests.Integration.Schemas
{
    using System.Collections.Generic;
    using System.Net.Http;

    using Machine.Specifications;

    using Model;

    using Newtonsoft.Json.Linq;

    public class when_retrieving_schemas : using_a_scim_server
    {
        Because of = async () =>
        {
            Response = await Server.HttpClient.GetAsync("schemas/" + SchemaId ?? "").Await().AsTask;

            if (Response.IsSuccessStatusCode)
            {
                var json = await Response.Content.ReadAsStringAsync();
                if (SchemaId == null)
                {
                    Schemas = await Response.Content.ReadAsAsync<IEnumerable<ScimSchema>>();
                    JsonData = JArray.Parse(json);
                }
                else
                {
                    Schemas = new List<ScimSchema> { await Response.Content.ReadAsAsync<ScimSchema>() };
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