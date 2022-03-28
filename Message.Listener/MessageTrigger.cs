using System.Threading.Tasks;
using System;
using System.IO;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Message.Listener;

public static class MessageTrigger
{
    [FunctionName("MessageTrigger")]
    public static async Task RunAsync(
        [QueueTrigger("api", Connection = "StorageConnectionAppSetting")]
        Message message, ILogger log, string id)
    {
        string guid = message.guid;
        int number = message.Number;
        int squaredNumber = number * number;
        UploadFile(squaredNumber, guid);
    }
    
    private static void UploadFile(int squaredNumber, string guid)
    {
        string connectionString = Environment.GetEnvironmentVariable("StorageConnectionAppSetting");

        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("files");
        
        string localPath = Path.GetTempPath();
        string fileName = $"{guid}" + ".txt";
        string localFilePath = Path.Combine(localPath, fileName);

        File.WriteAllTextAsync(localFilePath, $"{squaredNumber}");

        BlobClient blobClient = containerClient.GetBlobClient(fileName);
        blobClient.UploadAsync(localFilePath, true);
    }
}