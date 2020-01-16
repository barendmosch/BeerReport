using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System;
using System.Text;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Storage.Blob;

namespace BeerAppServerSide {

     internal class WeatherService : IWeatherService{

        public static readonly int ConstFahrCelcConversion = 32;
        /* Weather code Amsterdam: 249758 */
        private static readonly int countryCode = 249758;

        /* Storage account */
        CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(EnvironmentStrings.StorageAccount);

        /* Weather API call URL: */
        // https://atlas.microsoft.com/map/static/png?subscription-key=QfyAdGi2BBFxOzcPeBC20NyHfDdeUesLn2gvx1xWTzY&api-version=1.0&style=main&zoom=11&center=4.895168,52.370216&height=1024&width=1024
        public WeatherService(){}
        public async Task CreateBeerReport() {
            //string json = await GetWeatherInformation();
            //WeatherInformation weatherInformation = GetNecessaryInformation(json);
            WeatherInformation weatherInformation = new WeatherInformation(DateTime.Now, 12);
            SendDataToQueue(weatherInformation);
        }

        public async Task<MemoryStream> GetBeerReport() {

            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(EnvironmentStrings.BlobStorage);

            CloudBlob cloudBlob = cloudBlobContainer.GetBlockBlobReference("AmsterdamMapWeather.png");
                
            if (cloudBlob != null) {
                MemoryStream memoryStream = new MemoryStream();
                cloudBlob.DownloadToStream(memoryStream);

                byte [] buffer = memoryStream.GetBuffer();
                return new MemoryStream(buffer);
            } else {
                Console.WriteLine("No blob to download in the container");
                throw new BadRequestException();
            }
        }

        public async Task<string> GetWeatherInformation() {
            var url = $"http://dataservice.accuweather.com/forecasts/v1/daily/1day/{countryCode}?apikey=1yh0Gg86WIPFrrl34VFmJ1hrV9B4yC3B&language=en-us&details=false&metric=false";

            try {
                HttpClient newClient = new HttpClient();
                HttpRequestMessage newRequest = new HttpRequestMessage(HttpMethod.Get, string.Format(url));

                HttpResponseMessage response = await newClient.SendAsync(newRequest);
                return await response.Content.ReadAsStringAsync();
            }catch(Exception e) {
                Console.WriteLine(e.ToString());
                /* Make a badrequest exception from this */
                throw new BadRequestException();
            }
        }

        public WeatherInformation GetNecessaryInformation(string json) {
            dynamic data = JObject.Parse(json);

            /* Data to put into the queue for later */
            DateTime weatherDate = data.DailyForecasts [0].Date;
            int minFahrenheit = data.DailyForecasts [0].Temperature.Minimum.Value;
            int maxFahrenheit = data.DailyForecasts [0].Temperature.Maximum.Value;
            double avgCelciusToday = GetAverageCelcius(minFahrenheit, maxFahrenheit);

            return new WeatherInformation(weatherDate, avgCelciusToday);
        }

        public double GetAverageCelcius(int minFahr, int maxFahr) {
            double minCelc = (minFahr - 32) / 1.8;
            double maxCelc = (maxFahr - 32) / 1.8;
            return (minCelc + maxCelc) / 2;
        }

        public void SendDataToQueue(WeatherInformation weatherInformation) {
            string jsonData = JsonConvert.SerializeObject(weatherInformation);
            try {
                CloudQueueClient cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
                CloudQueue cloudQueue = cloudQueueClient.GetQueueReference("beerqueue");

                cloudQueue.CreateIfNotExists();
                cloudQueue.AddMessage(new CloudQueueMessage(jsonData));
            }catch(Exception e) {
                Console.WriteLine(e.ToString());
                /* Throw new Cant create cloud exceptions or something */
                throw new ServiceUnavailableException();
            }
        }
    }
}