using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Logging;
using System;
using Newtonsoft.Json;

namespace BeerAppServerSide {

    /* This is custom exceptionHandler which contains:

    - 400 BadRequest
    - 401 Unauthorized (not implemented)
    - 404 NotFound
    - 500 Internal Service Error
    - 503 Service Not Available
    
    Throwing one of the exceptions also stores the message in the log*/
    public class ExceptionHandler : IExceptionHandler {

        private readonly ILogger log;
        public ExceptionHandler(ILoggerFactory loggerFactory) {
            log = loggerFactory.CreateLogger(LogCategories.CreateFunctionUserCategory("ExceptionHandler"));
        }
        
        /*Undocumented filter parameter*/
        private readonly string badRequestMessage = $"{HttpStatusCode.BadRequest}: Bad request on the API services";

        /*Internal Service Error, most commonly thrown by missing required data in JSON*/
        private readonly string jsonRequired = $"RequestBody is missing required data";

        /*Some Token error message*/
        private readonly string cloudMessage = $"Could not open or find the requested Cloud services";

        public HttpResponseMessage ReturnException(Exception e) {
            if (e is BadRequestException) {
                return BadRequest();
            } else if (e is JsonSerializationException) {
                return JsonRequired();
            } else if (e is ServiceUnavailableException) {
                return CloudServicesNotWorking();
            } else {
                /* Everything else is seen as an generic exception */
                throw new Exception();
            }
        }
        public HttpResponseMessage BadRequest() {
            log.LogError(badRequestMessage);
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }

        public HttpResponseMessage JsonRequired() {
            log.LogError(jsonRequired);
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
        public HttpResponseMessage CloudServicesNotWorking() {
            log.LogError(cloudMessage);
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }
    }
}
