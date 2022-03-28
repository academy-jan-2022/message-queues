using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Message.Listener;

public static class MessageTrigger
{
    [FunctionName("MessageTrigger")]
    public static async Task RunAsync(
        [QueueTrigger("api", Connection = "StorageConnectionAppSetting")]
        Message message,
        ILogger log,
        string id
    ) =>
        await UploadFile(message.Number * message.Number, message.guid);

    private static Task UploadFile(int squaredNumber, string guid) =>
        new BlobServiceClient(
                Environment.GetEnvironmentVariable("StorageConnectionAppSetting")
            )
            .GetBlobContainerClient("files")
            .GetBlobClient($"{guid}" + ".txt")
            .UploadAsync(new BinaryData(Encoding.UTF8.GetBytes(squaredNumber.ToString())), true);
}
