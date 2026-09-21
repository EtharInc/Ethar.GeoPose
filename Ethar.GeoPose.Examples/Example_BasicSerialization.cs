using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Ethar.GeoPose.DataTypes;
using Ethar.GeoPose.StructuralDataUnits;
using Newtonsoft.Json;

namespace Ethar.GeoPose.Examples
{
    /// <summary>
    /// This example shows how to serialize and deserialize a basic SDU.
    /// </summary>
    internal class Example_BasicSerialization
    {
        internal static async Task SerialiseGeoPose()
        {
            var sdu = new BasicYawPitchRollSdu(new YawPitchRollAngles(1, 2, 3),
                new TangentPointPosition(1, 2, 3));

            var converted = JsonConvert.SerializeObject(sdu);
            var content = new StringContent(converted, Encoding.UTF8, "application/json");

            var client = new HttpClient();

            // Replace the endpoint with the endpoint you want to send the data to. 
            var response = await client.PostAsync("YourEndpoint", content);
        }

        internal static async Task<BasicYawPitchRollSdu> DeSerialiseGeoPose()
        {
            var client = new HttpClient();

            var lat = 1;
            var lon = 2;
            var height = 3;

            var response = await client.GetAsync($"https://service.geopose.io/solar/solarpose/Ypr?longitude={lon}&latitude={lat}&height={height}");
            var json = await response.Content.ReadAsStringAsync();
            var sdu = JsonConvert.DeserializeObject<BasicYawPitchRollSdu>(json);
            return sdu;
        }
    }
}