using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FunctionWithHealthCheck
{
    public class ServiceBusExample
    {

        [FunctionName("ServiceBusExample")]
        public void Run([ServiceBusTrigger("fncsvcbqsl-svcbus-queue", Connection = "ServiceBusConnectionString")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
