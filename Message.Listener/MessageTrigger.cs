using System.Threading.Tasks;
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Message.Listener;

public static class MessageTrigger
{
    [FunctionName("MessageTrigger")]
    public static async Task RunAsync(
        [QueueTrigger("api", Connection = "StorageConnectionAppSetting")]
        string message,
        ILogger log,
        string id)
    {
        log.LogInformation($"C# Queue trigger function processed: {message}");
        int squaredNumber = Int16.Parse(message) * Int16.Parse(message);
        log.LogInformation($"Squared number is: {squaredNumber}");
        log.LogInformation($"{id}");
    }
}