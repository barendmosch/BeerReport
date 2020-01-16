using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Logging;

namespace BeerAppServerSide {

     internal class WeatherService : IWeatherService{

        private readonly ILogger log;

        /* Some constant values */
        public static readonly int ConstFahrCelcConversion = 32;

        /* Weather code Amsterdam: 249758 */
        private static readonly int countryCode = 249758;

        /* Azure Storage account */
        CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(EnvironmentStrings.StorageAccount);

        public WeatherService(ILoggerFactory loggerFactory) {
            log = loggerFactory.CreateLogger(LogCategories.CreateFunctionUserCategory("ExceptionHandler"));
        }

        public async Task CreateBeerReport() {
            /* Get the weather information */
            log.LogInformation($"Getting weather forecast of Amsterdam on {DateTime.Today}");
            string json = await GetWeatherInformation();
            /* Filter the weather Information to get only the date and average celcius of today */
            WeatherInformation weatherInformation = GetNecessaryInformation(json);

            // WeatherInformation weatherInformation = new WeatherInformation(DateTime.Now, 12); // <----- THIS IS HERE BECAUSE MY WEATHERAPI ONLY ALLOWS 50 CALLS EACH DAY, SO I NEEDED TEST DATA!!!!

            /* Send the data to the queue */
            log.LogInformation($"Sending weather data to the Queue");
            SendDataToQueue(weatherInformation);
        }

        public async Task<MemoryStream> GetBeerReport() {
            log.LogInformation($"Getting the Azure Map");
            /* Make the Cloud services and get the map picture in the blob storage */
            CloudBlob cloudBlob;
            try {
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(EnvironmentStrings.BlobStorage);

                cloudBlob = cloudBlobContainer.GetBlockBlobReference("AmsterdamMapWeather.png");
            }catch(Exception) {
                throw new ServiceUnavailableException();
            }

            /* If blob exists */
            if (cloudBlob != null) {
                /* Download the image and put the data into a DataStream */
                MemoryStream memoryStream = new MemoryStream();
                cloudBlob.DownloadToStream(memoryStream);

                /* Little workaround by buffering and return the buffer to the controller */
                byte [] buffer = memoryStream.GetBuffer();
                return new MemoryStream(buffer);
            } else {
                throw new BlobDoesNotExistException();
            }
        }

        public async Task<string> GetWeatherInformation() {
            /* The URL for daily the weather forecast of Amsterdam */
            var url = $"http://dataservice.accuweather.com/forecasts/v1/daily/1day/{countryCode}?apikey=1yh0Gg86WIPFrrl34VFmJ1hrV9B4yC3B&language=en-us&details=false&metric=false";

            /* Try to perform a GET call to the above URL and read the data as JSON */
            try {
                HttpClient newClient = new HttpClient();
                HttpRequestMessage newRequest = new HttpRequestMessage(HttpMethod.Get, string.Format(url));

                HttpResponseMessage response = await newClient.SendAsync(newRequest);
                return await response.Content.ReadAsStringAsync();
            }catch(Exception) {
                throw new BadRequestException();
            }
        }

        /* Send the important weather information to the queue to trigger the queue */
        public void SendDataToQueue(WeatherInformation weatherInformation) {
            string jsonData = JsonConvert.SerializeObject(weatherInformation);
            try {
                /* Make the azure services */
                CloudQueueClient cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
                CloudQueue cloudQueue = cloudQueueClient.GetQueueReference("beerqueue");

                /* Create a queue if it doesnt exist already and pass the JSON as message to the queue */
                cloudQueue.CreateIfNotExists();
                cloudQueue.AddMessage(new CloudQueueMessage(jsonData));
            } catch (Exception) {
                throw new ServiceUnavailableException();
            }
        }

        /* Go through the weather information to only get the date and min / max fahrenheit
           return an object of WeatherInformation which contains the date and average Celcius of that day */
        public WeatherInformation GetNecessaryInformation(string json) {
            dynamic data = JObject.Parse(json);

            /* Data to put into the queue for later */
            DateTime weatherDate = data.DailyForecasts [0].Date;
            int minFahrenheit = data.DailyForecasts [0].Temperature.Minimum.Value;
            int maxFahrenheit = data.DailyForecasts [0].Temperature.Maximum.Value;
            double avgCelciusToday = GetAverageCelcius(minFahrenheit, maxFahrenheit);

            return new WeatherInformation(weatherDate, avgCelciusToday);
        }

        /* Get the Average celcius given by the min and max fahrenheit */
        public double GetAverageCelcius(int minFahr, int maxFahr) {
            double minCelc = (minFahr - 32) / 1.8;
            double maxCelc = (maxFahr - 32) / 1.8;
            return (minCelc + maxCelc) / 2;
        }
    }
}