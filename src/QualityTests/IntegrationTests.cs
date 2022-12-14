using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using QualityTests.IntegrationSupport;
using System.Diagnostics;

namespace QualityTests
{
    [TestClass]
    public class IntegrationTests
    {
        [TestMethod]
        public async Task TestHttpInvocation()
        {
            // Arrange
            var numberToCheck = 6785634335678;

            // Act
            var response = await httpClient.GetStringAsync($"{GetBasePath()}/NumberCheck?number={numberToCheck}");

            // Assert
            Assert.AreEqual("Even", response);
        }


        [TestMethod]
        public async Task TestTimerInvocation()
        {
            // Arrange
            host.StartWatcher();
            // Act
            await Task.Delay(60 * 1000); // 60 secs
            var logs =  host.GetLogs();

            var occurance = logs.Count(s => s.Contains("Executing 'TimerFunction'"));

            // Assert
            Assert.IsTrue(occurance > 0);
        }

        #region Setting up test context
        private readonly static string workDir = @"C:\Git\moimhossain\az-func-test-demo\src\DemoFunctions\bin\Debug\net6.0";
        private readonly static ILogger logger = NullLoggerFactory.Instance.CreateLogger(nameof(UnitTests));
        private readonly static FuncProcessHost host = new FuncProcessHost();
        private readonly static uint port = 34698;
        private readonly static HttpClient httpClient = new HttpClient();

        private static string GetBasePath()
        {
            return $"http://localhost:{port}/api";
        }

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            Trace.WriteLine(context.TestRunDirectory);
            host.Start(port, workDir);
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            host.Stop();
        }
        #endregion
    }
}