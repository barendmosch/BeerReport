using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System;
using System.Text;

namespace BeerAppServerSide {
    public class WeatherController {

        private IWeatherService weatherService;
        public WeatherController(IWeatherService weatherService){
            this.weatherService = weatherService;
        }

        [FunctionName("WeatherInformation")]
        public async Task<HttpResponseMessage> Run( [HttpTrigger(AuthorizationLevel.Anonymous, 
            "get", Route = "weather/today/{countryCode}")] HttpRequestMessage req, ILogger log, int countryCode) {

            weatherService = new WeatherService();
            
            if (req.Method == HttpMethod.Get){
                try{
                    var json = await weatherService.GetWeather(countryCode);

                    return new HttpResponseMessage(HttpStatusCode.OK){
                        Content = new StringContent(json, Encoding.UTF8, "application/json")
                    }; 
                }catch(Exception e){
                    log.LogError(e.ToString());
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
            }else{
                throw new NotImplementedException();
            }
        }
    }
}
