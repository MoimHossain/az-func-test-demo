using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DemoFunctions
{
    public class ScheduledFuncs
    {
        // every 5 secs */15 * * * * *
        // every min 0 * * * * *
        // every 5 min 0 */5 * * * *

        [FunctionName(nameof(TimerFunction))]
        public void TimerFunction([TimerTrigger("*/15 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger {myTimer.Schedule.ToString()} function executed at: {DateTime.Now}");
        }
    }
}
