

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Numerics;
using Microsoft.AspNetCore.Mvc.Formatters.Internal;

namespace DemoFunctions
{
    public static class BusinessFunctions
    {
        [FunctionName(nameof(NumberCheck))]
        public static async Task<IActionResult> NumberCheck(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation($"{nameof(NumberCheck)} function processed a request.");

            string numberParameter = req.Query["number"];

            if(BigInteger.TryParse(numberParameter, out BigInteger value))
            {
                return new OkObjectResult(value % 2 == 0 ? "Even" : "Odd");
            }
            else
            {
                await Task.Delay(1);
                return new BadRequestObjectResult("Was it an invalid number you tried to pass?");
            }
        }
    }
}
