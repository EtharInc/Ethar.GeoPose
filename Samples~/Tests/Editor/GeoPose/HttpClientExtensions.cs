using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ethar.GeoPose.UnitTests
{
    public static class HttpClientExtensions
    {
        public static async Task<T> ReadAsJsonAsync<T>(this HttpClient client, string address)
        {
            var response = await client.GetAsync(address);
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}