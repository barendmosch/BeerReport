using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System;
using System.Text;
using Newtonsoft.Json.Linq;

namespace BeerAppServerSide {

     internal class WeatherService : IWeatherService{

        public static readonly double ConstFahrCelcConversion = 17.778;

        // Weather code Amsterdam: 249758
        public WeatherService(){}
        public async Task<string> GetWeather(int countryCode){
            var url = $"http://dataservice.accuweather.com/forecasts/v1/daily/1day/{countryCode}?apikey=1yh0Gg86WIPFrrl34VFmJ1hrV9B4yC3B&language=en-us&details=false&metric=false";

            HttpClient newClient = new HttpClient();
            HttpRequestMessage newRequest = new HttpRequestMessage(HttpMethod.Get, string.Format(url));

            HttpResponseMessage response = await newClient.SendAsync(newRequest);
            
            /* Read the responseBody and paste to a JObject */
            string json = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(json);

            /* Data to put into the queue for later */
            string weatherType = data.DailyForecasts[0].Day.IconPhrase;
            int minFahrenheit = data.DailyForecasts[0].Temperature.Minimum.Value;
            int maxFahrenheit = data.DailyForecasts[0].Temperature.Maximum.Value; 
            double avgCelciusToday = ((minFahrenheit - ConstFahrCelcConversion) * (maxFahrenheit - ConstFahrCelcConversion)) / 2;

            Console.WriteLine(weatherType);
            Console.WriteLine(avgCelciusToday);

            StorageAccount storageAccount = null;

            return json;
        }
    }
}