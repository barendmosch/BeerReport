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
using System.Net.Http.Headers;

namespace BeerAppServerSide {
    public class WeatherController {

        private IWeatherService weatherService; 
        private IExceptionHandler exceptionHandler;

        public WeatherController(IWeatherService weatherService, IExceptionHandler exceptionHandler) {
            this.weatherService = weatherService;
            this.exceptionHandler = exceptionHandler;
        }

        /* Make a generic exceptionHandler */

        [FunctionName("WeatherInformation")]
        public async Task<HttpResponseMessage> Run( [HttpTrigger(AuthorizationLevel.Anonymous, 
            "get", "post", Route = "weather/today")] HttpRequestMessage req, ILogger log) {

            if (req.Method == HttpMethod.Get){
                try {
                    MemoryStream dataStream = await weatherService.GetBeerReport();

                    HttpResponseMessage httpResponseMessage = new HttpResponseMessage {
                        Content = new StreamContent(dataStream)
                    };
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

                    return httpResponseMessage;
                } catch (Exception e) {
                    log.LogError(e.ToString());
                    return exceptionHandler.ReturnException(e);
                }
            } else if (req.Method == HttpMethod.Post) {
                try {

                    //string fileName = "";
                    //
                    //using (StringReader reader = new StringReader(await req.Content.ReadAsStringAsync())) {
                    //    fileName = JsonConvert.DeserializeObject<string>(reader.ReadToEnd());   
                    //}

                    await weatherService.CreateBeerReport();

                    return new HttpResponseMessage(HttpStatusCode.Created);
                } catch (Exception e) {
                    log.LogError(e.ToString());
                    return exceptionHandler.ReturnException(e);
                }
            }
            throw new NotImplementedException();
        }
    }
}
