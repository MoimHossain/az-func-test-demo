

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace QualityTests
{
    [TestClass]
    public class UnitTests
    {
        private readonly ILogger logger = NullLoggerFactory.Instance.CreateLogger(nameof(UnitTests));

        // Unit test the DemoFunctions.BusinessFunctions.NumberCheck function  for event numbers      
        [TestMethod]
        public async Task TestEvenNumbers()
        {
            // Arrange
            var request = GenerateHttpRequest("235466567788");

            // Act
            var response = await DemoFunctions.BusinessFunctions.NumberCheck(request, logger);

            // Assert
            Assert.IsInstanceOfType(response, typeof(OkObjectResult));
            Assert.AreEqual("Even", ((OkObjectResult)response).Value as string);
        }

        // Unit test the DemoFunctions.BusinessFunctions.NumberCheck function for Odd numbers       
        [TestMethod]
        public async Task TestOddNumbers()
        {
            // Arrange
            var request = GenerateHttpRequest("235466567781");

            // Act
            var response = await DemoFunctions.BusinessFunctions.NumberCheck(request, logger);

            // Assert
            Assert.IsInstanceOfType(response, typeof(OkObjectResult));
            Assert.AreEqual("Odd", ((OkObjectResult)response).Value as string);
        }

        // Unit test the DemoFunctions.BusinessFunctions.NumberCheck function for Invalid numbers       
        [TestMethod]
        public async Task TestInvalidNumbers()
        {
            // Arrange
            var request = GenerateHttpRequest("NotANumber");

            // Act
            var response = await DemoFunctions.BusinessFunctions.NumberCheck(request, logger);

            // Assert
            Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));
            Assert.AreEqual(400, ((BadRequestObjectResult)response).StatusCode);
        }

        private DefaultHttpRequest GenerateHttpRequest(string number)
        {
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues> { { "number", number.ToString() } })
            };
        }
    }
}