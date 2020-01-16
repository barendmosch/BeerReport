using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Logging;
using System;
using Newtonsoft.Json;

namespace BeerAppServerSide {

    /* Custom exceptionHandler to catch exception and log a hardcoded log message to the user */
    public class ExceptionHandler : IExceptionHandler {

        private readonly ILogger log;
        public ExceptionHandler(ILoggerFactory loggerFactory) {
            log = loggerFactory.CreateLogger(LogCategories.CreateFunctionUserCategory("ExceptionHandler"));
        }
        
        private readonly string badRequestMessage = $"Bad request occured calling the external API services";

        private readonly string cloudMessage = $"Could not open or find the requested Cloud services";

        private readonly string blobRequiredMessage = $"No blob to download in the container";

        private readonly string uploadFailedMessage = "Something went wrong with uploading the image to the blob container";

        public HttpResponseMessage ReturnException(Exception e) {
            if (e is BadRequestException) {
                return BadRequest();
            } else if (e is BlobDoesNotExistException) {
                return BlobRequired();
            } else if (e is ServiceUnavailableException) {
                return CloudServicesNotWorking();
            } else if(e is UploadException) {
                return UploadNotWorking();
            } else {
                /* Everything else is seen as an generic exception */
                throw new Exception();
            }
        }
        public HttpResponseMessage BadRequest() {
            log.LogError(badRequestMessage);
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    
        public HttpResponseMessage CloudServicesNotWorking() {
            log.LogError(cloudMessage);
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }
        public HttpResponseMessage BlobRequired() {
            log.LogError(blobRequiredMessage);
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }
        public HttpResponseMessage UploadNotWorking() {
            log.LogError(uploadFailedMessage);
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }
}
