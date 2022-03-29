using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Message.Listener;

public static class MessageTrigger
{
    private static readonly Lazy<TelemetryClient> TelemetryClientLazy =
        new(() =>
        {
            var config = TelemetryConfiguration.CreateDefault();
            config.InstrumentationKey =
                Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");
            return new TelemetryClient(config);
        });
    
    private static TelemetryClient TelemetryClient => TelemetryClientLazy.Value;

    [FunctionName("MessageTrigger")]
    public static async Task RunAsync(
        [QueueTrigger("api", Connection = "StorageConnectionAppSetting")]
        Message message,
        ILogger log,
        string id
    )
    {
        var correlationID = message.correlationId;
        TelemetryClient.TrackTrace(
            "C# Queue trigger function processed a mesage",
            new Dictionary<string, string>
            {
                { "correlationID", correlationID.ToString() }
            }
        );

        await UploadFile(message.Number * message.Number, message.guid);
    }

    private static Task UploadFile(int squaredNumber, string guid) =>
        new BlobServiceClient(
                Environment.GetEnvironmentVariable("StorageConnectionAppSetting")
            )
            .GetBlobContainerClient("files")
            .GetBlobClient($"{guid}" + ".txt")
            .UploadAsync(new BinaryData(Encoding.UTF8.GetBytes(squaredNumber.ToString())), true);
}
