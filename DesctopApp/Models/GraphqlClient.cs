using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace DesctopApp.Models
{
    static class GraphqlClient
    {
        public static GraphQLHttpClient CreateGraphqlClient()
        {
            var client = new GraphQLHttpClient("http://localhost:5000/graphql", new NewtonsoftJsonSerializer());

            return client;
        }

        public static GraphQLRequest RequestGetNews()
        {
            return new GraphQLRequest
            {
                Query = @"query { news{ iD, title, author, description, like, dislike } }",
            };
        }

        public static async Task<List<News>> GetAllNews()
        {
            var client = CreateGraphqlClient();
            var requst = RequestGetNews();

            try
            {
                var response = await client.SendQueryAsync<object>(requst);
                if (response.Errors == null)
                {
                    JObject data = JObject.Parse(response.Data?.ToString() ?? "{}");

                    return data["news"].ToObject<List<News>>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return default;
        }
    }
}
