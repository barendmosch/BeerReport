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

        /* Dependency Injection for the service and the exceptionHandler */
        public WeatherController(IWeatherService weatherService, IExceptionHandler exceptionHandler) {
            this.weatherService = weatherService;
            this.exceptionHandler = exceptionHandler;
        }

        [FunctionName("WeatherInformation")]
        public async Task<HttpResponseMessage> Run( [HttpTrigger(AuthorizationLevel.Anonymous, 
            "get", "post", Route = "weather/today")] HttpRequestMessage req, ILogger log) {

            /* Get method for getting the image with text from the blob and return the image to the user */
            if (req.Method == HttpMethod.Get){
                log.LogInformation($"Calling {req.Method} Method");
                try {
                    /* Get the image as a Stream */
                    MemoryStream dataStream = await weatherService.GetBeerReport();

                    /* Return the stream as an PNG */
                    HttpResponseMessage httpResponseMessage = new HttpResponseMessage {
                        Content = new StreamContent(dataStream)
                    };
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

                    return httpResponseMessage;
                } catch (Exception e) {
                    return exceptionHandler.ReturnException(e);
                }
            /* Post method to get the weather forecast, put necessary data into the queue
                trigger the queue to make an Image from the Azure maps API, put the weather text onto the map
                and upload the map in a blob storage */
            } else if (req.Method == HttpMethod.Post) {
                log.LogInformation($"Calling {req.Method} Method");
                try {
                    /* Start the process of making a 'report' */
                    await weatherService.CreateBeerReport();

                    return new HttpResponseMessage(HttpStatusCode.Created);
                } catch (Exception e) {
                    return exceptionHandler.ReturnException(e);
                }
            } else {
                throw new NotImplementedException();
            }
        }
    }
}
