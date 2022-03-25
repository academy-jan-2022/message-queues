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
        string message, ILogger log, string id)
    {
        int number = Int16.Parse(message);
        int squaredNumber = number * number;
        UploadFile(squaredNumber, id);
    }
    
    private static void UploadFile(int squaredNumber, string id)
    {
        string connectionString = Environment.GetEnvironmentVariable("StorageConnectionAppSetting");

        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("files");
        
        string localPath = "./";
        string fileName = $"{id}" + ".txt";
        string localFilePath = Path.Combine(localPath, fileName);

        File.WriteAllTextAsync(localFilePath, $"{squaredNumber}");

        BlobClient blobClient = containerClient.GetBlobClient(fileName);

        Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

        blobClient.UploadAsync(localFilePath, true);
    }
    
}