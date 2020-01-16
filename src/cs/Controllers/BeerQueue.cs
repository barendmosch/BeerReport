using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Drawing;

namespace BeerAppServerSide {
    public static class BeerQueue {

        [FunctionName("beerqueue")]
        public static async void Run([QueueTrigger("beerqueue", Connection = "AzureWebJobsStorage")]string weatherQueue, ILogger log) {

            /* Weather API call URL: */
            string URLAmsterdamMap = "https://atlas.microsoft.com/map/static/png?subscription-key=QfyAdGi2BBFxOzcPeBC20NyHfDdeUesLn2gvx1xWTzY&api-version=1.0&style=main&zoom=11&center=4.895168,52.370216&height=1024&width=1024";

            log.LogInformation($"C# Queue trigger function processed: {weatherQueue}");

            /* Parse JSON to Weather object */
            WeatherInformation weatherInformation = JsonConvert.DeserializeObject<WeatherInformation>(weatherQueue);

            HttpClient newClient = new HttpClient();
            HttpRequestMessage newRequest = new HttpRequestMessage(HttpMethod.Get, string.Format(URLAmsterdamMap));

            HttpResponseMessage response = await newClient.SendAsync(newRequest);

            /* Read the responseBody and paste to a JObject */
            //var json = response.Content.ReadAsStringAsync();
            var result = await response.Content.ReadAsStreamAsync();
            Image image = Image.FromStream(result);
            Bitmap bitmap = (Bitmap)image;
            Console.WriteLine(bitmap.GetType());
        }
    }
}
