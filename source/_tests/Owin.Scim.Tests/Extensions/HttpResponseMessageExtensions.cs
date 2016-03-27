namespace Owin.Scim.Tests.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq.Expressions;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    internal static class HttpResponseMessageExtensions
    {
        public static async Task DeserializeTo(
            this HttpResponseMessage response,
            Expression<Func<IDictionary<string, object>>> jsonDataToSet = null)
        {
            var json = await response.Content.ReadAsByteArrayAsync();
            var formatter = new ScimJsonMediaTypeFormatter();
            var serializer = formatter.CreateJsonSerializer();
            var dictReader = formatter.CreateJsonReader(typeof(IDictionary<string, object>), new MemoryStream(json), Encoding.UTF8);

            var jsonData = (IDictionary<string, object>)serializer.Deserialize(dictReader, typeof(IDictionary<string, object>));
            var jsonMemberExp = jsonDataToSet.Body as MemberExpression;
            if (jsonMemberExp == null) throw new ArgumentException("Must be a MemberExpression!", "jsonDataToSet");

            jsonMemberExp.Member
                .DeclaringType
                .GetField(
                    jsonMemberExp.Member.Name,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .SetValue(
                    jsonMemberExp.Member.DeclaringType,
                    jsonData,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy,
                    null,
                    CultureInfo.CurrentUICulture);
        }

        public static async Task DeserializeTo<T>(
            this HttpResponseMessage response,
            Expression<Func<T>> instanceToSet,
            Expression<Func<IDictionary<string, object>>> jsonDataToSet = null)
        {
            var instMemberExp = instanceToSet.Body as MemberExpression;
            if (instMemberExp == null) throw new ArgumentException("Must be a MemberExpression!", "instanceToSet");

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created) return;

            var result = await DeserializeResponse<T>(response, jsonDataToSet != null);
            instMemberExp.Member
                .DeclaringType
                .GetField(
                    instMemberExp.Member.Name,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static |
                    BindingFlags.FlattenHierarchy)
                .SetValue(
                    instMemberExp.Member.DeclaringType,
                    result.Instance,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static |
                    BindingFlags.FlattenHierarchy,
                    null,
                    CultureInfo.CurrentUICulture);

            if (jsonDataToSet == null) return;

            var jsonMemberExp = jsonDataToSet.Body as MemberExpression;
            if (jsonMemberExp == null) throw new ArgumentException("Must be a MemberExpression!", "jsonDataToSet");

            jsonMemberExp.Member
                .DeclaringType
                .GetField(
                    jsonMemberExp.Member.Name,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .SetValue(
                    jsonMemberExp.Member.DeclaringType,
                    result.JsonData,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy,
                    null,
                    CultureInfo.CurrentUICulture);
        }

        private static async Task<ResponseHelper<T>> DeserializeResponse<T>(HttpResponseMessage response, bool withJson = false)
        {
            T instance;
            if (!withJson)
            {
                instance = await response.Content.ReadAsAsync<T>(new[] { new ScimJsonMediaTypeFormatter() });
                return new ResponseHelper<T>(null, instance);
            }

            var json = await response.Content.ReadAsByteArrayAsync();
            var formatter = new ScimJsonMediaTypeFormatter();
            var serializer = formatter.CreateJsonSerializer();
            var dictReader = formatter.CreateJsonReader(typeof(IDictionary<string, object>), new MemoryStream(json), Encoding.UTF8);
            var objReader = formatter.CreateJsonReader(typeof(T), new MemoryStream(json), Encoding.UTF8);

            var jsonData = (IDictionary<string, object>)serializer.Deserialize(dictReader, typeof(IDictionary<string, object>));
            instance = (T)serializer.Deserialize(objReader, typeof(T));

            return new ResponseHelper<T>(jsonData, instance);
        }

        internal class ResponseHelper<T>
        {
            public ResponseHelper(IDictionary<string, object> jsonData, T instance)
            {
                JsonData = jsonData;
                Instance = instance;
            }

            public IDictionary<string, object> JsonData { get; private set; }

            public T Instance { get; private set; }
        }
    }
}