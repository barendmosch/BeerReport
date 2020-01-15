using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BeerAppServerSide {

     internal class WeatherService : IWeatherService{

        // Weather code Amsterdam: 249758
        public WeatherService(){}
        public async Task<string> GetWeather(int countryCode){
            var url = $"http://dataservice.accuweather.com/forecasts/v1/daily/1day/{countryCode}?apikey=1yh0Gg86WIPFrrl34VFmJ1hrV9B4yC3B&language=en-us&details=false&metric=false";

            HttpClient newClient = new HttpClient();
            HttpRequestMessage newRequest = new HttpRequestMessage(HttpMethod.Get, string.Format(url));

            HttpResponseMessage response = await newClient.SendAsync(newRequest);
            
            string json = await response.Content.ReadAsStringAsync();
            
            dynamic data = JObject.Parse(json);

            string weatherType = data.DailyForecasts.Day.IconPhrase;
            int minFahrenheit = data.DailyForecasts.Temperature.Minimum.Value;
            int maxFahrenheit = data.DailyForecasts.Temperature.Maximum.Value; 

            Console.WriteLine(weatherType);
            Console.WriteLine(minFahrenheit);
            Console.WriteLine(maxFahrenheit); 


            return json;
        }
    }
}