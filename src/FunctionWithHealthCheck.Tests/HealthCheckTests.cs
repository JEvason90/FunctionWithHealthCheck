using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using QueueDetector;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using Microsoft.AspNetCore.Http;
using System.IO;
using Newtonsoft.Json;

namespace FunctionWithHealthCheck.Tests
{
    public class HealthCheckTests
    {
        private Mock<ILogger> _mockLogger;

        private Mock<HealthCheckService> _mockHealthCheckService;

        private Mock<ServiceBusExample> _mockServiceBusFunction;

        public HealthCheckTests()
        {
            _mockLogger = new Mock<ILogger>();
            _mockHealthCheckService = new Mock<HealthCheckService>();
            _mockServiceBusFunction = new Mock<ServiceBusExample>();
        }

        [Fact]
        public void If_A_Given_Function_Fails_Health_Check_Returns_500()
        {
            var function = new ServiceBusExample();

            function.Run(null, _mockLogger.Object);

            var healthCheck = new AppHealthCheck(_mockHealthCheckService.Object);

            var healthStatus = healthCheck.Run(CreateMockRequest().Object,_mockLogger.Object);

            Assert.True(healthStatus.IsFaulted);
        }

        private static Mock<HttpRequest> CreateMockRequest(object body = null)
        {            
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
        
            var json = JsonConvert.SerializeObject(body);
        
            sw.Write(json);
            sw.Flush();
        
            ms.Position = 0;
        
            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(x => x.Body).Returns(ms);
        
            return mockRequest;
        }
    }
}
