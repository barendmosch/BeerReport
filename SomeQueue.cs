using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Company.Function {
    public static class SomeQueue {
        [FunctionName("SomeQueue")]
        public static void Run([QueueTrigger("myqueue-items", Connection = "AzureWebJobsStorage")]string myQueueItem, ILogger log) {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
