using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System.Net.Http;
using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Diagnostics.HealthChecks;

[assembly: FunctionsStartup(typeof(QueueDetector.Startup))]

namespace QueueDetector
{
    public class Startup : FunctionsStartup
    {

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();

            var basicCircuitBreakerPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .CircuitBreakerAsync(2, TimeSpan.FromSeconds(3));

            builder.Services.AddHttpClient("FlakyService", client =>
            {
                client.BaseAddress = new Uri("http://localhost:8080/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }).AddPolicyHandler(basicCircuitBreakerPolicy);

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .AddEnvironmentVariables()
                .Build();

            var svcBusConnString = config.GetSection("Values:ServiceBusConnectionString").Value;
            var queueName = config.GetSection("Values:QueueName").Value;
            var databaseConnString = config.GetConnectionString("DatabaseConnection") ?? "";

            builder.Services
                .AddHealthChecks()
                .AddCheck<FunctionAppHealthCheck>("function-app")
                .AddAzureServiceBusQueue(
                    connectionString: svcBusConnString,
                    queueName: queueName,
                    failureStatus: HealthStatus.Degraded,
                    tags: new string[] { "queue", "servicebus" })
                .AddNpgSql(
                    npgsqlConnectionString: databaseConnString,
                    healthQuery: "SELECT 1;",
                    name: "sql",
                    failureStatus: HealthStatus.Degraded,
                    tags: new string[] { "db", "sql", "sqlserver" });
        }

        private void OnHalfOpen()
        {
            Console.WriteLine("Circuit in test mode, one request will be allowed.");
        }

        private void OnReset()
        {
            Console.WriteLine("Circuit closed, requests flow normally.");
        }

        private void OnBreak(DelegateResult<HttpResponseMessage> result, TimeSpan ts)
        {
            Console.WriteLine("Circuit cut, requests will not flow.");
        }
    }
}