using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace QueueDetector
{
    public class AppHealthCheck
    {
        private HealthCheckService _healthCheck;
        public AppHealthCheck(HealthCheckService healthCheck)
        {
            _healthCheck = healthCheck;
        }
        
        [FunctionName("HealthCheck")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "health")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Calling Health Check End Point");

            var healthStatus = _healthCheck.CheckHealthAsync().Result;

            return new OkObjectResult(healthStatus);
        }
    }
}
