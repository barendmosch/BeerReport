using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Globalization;

namespace BeerAppServerSide {
    public class BeerQueue {

        [FunctionName("beerqueue")]
        public static async void Run([QueueTrigger("beerqueue", Connection = "AzureWebJobsStorage")]string weatherQueue, ILogger log) {
            log.LogInformation($"beerqueue triggered");
            /* Parse JSON to Weather object */
            WeatherInformation weatherInformation = JsonConvert.DeserializeObject<WeatherInformation>(weatherQueue);
            
            /* Make a Map object to perform image and blob actions */
            MapImage mapImage = new MapImage(weatherInformation);

            /* Replace the ',' sign with '.' to ensure that the coordinate values URLAmsterdamMap are doubles with dots to ensure 2 coordinates instead of 4 */
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            /* Get the coordinates of Amsterdam with '.' sign instead of ','*/
            string longitude = weatherInformation.Longitude.ToString(nfi);
            string latitude = weatherInformation.Latitude.ToString(nfi);
            
            /* Azure MAPS API call URL with the given key and Amsterdam coordinates */
            string URLAmsterdamMap = $"https://atlas.microsoft.com/map/static/png?subscription-key={EnvironmentStrings.AzureMaps}&api-version=1.0&style=main&zoom=11&center={longitude},{latitude}&height=1024&width=1024";

            /* Try to execute the Azure API GET call to get the Map */
            Stream result = null;
            try {
                HttpClient newClient = new HttpClient();
                HttpRequestMessage newRequest = new HttpRequestMessage(HttpMethod.Get, string.Format(URLAmsterdamMap));

                log.LogInformation($"Calling Azure Maps API");
                HttpResponseMessage response = await newClient.SendAsync(newRequest);

                /* Read the result and put it in a Stream (this is the image of a map of Amsterdam */
                result = await response.Content.ReadAsStreamAsync();
            }catch(Exception) {
                throw new BadRequestException();
            }

            /* Try to upload the map to the blob storage */
            try {
                log.LogInformation($"Uploading image to blob storage");
                mapImage.UploadImageToBlob(result);
            }catch(Exception) {
                throw new UploadException();
            }
        }
    }
}
