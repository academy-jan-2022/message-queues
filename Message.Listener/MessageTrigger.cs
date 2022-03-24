using System.Threading.Tasks;
using System;
using System.IO;
using System.Reflection.Metadata;
using Azure.Storage.Blobs;
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
        
        // Create a BlobServiceClient object which will be used to create a container client
        string connectionString =
            "DefaultEndpointsProtocol=https;AccountName=academy2022jan;AccountKey=RFAjw7rZH83DeZKPCic4zEgNpmPitHBFUvCO32DgN7jRn2Y3VWyZCLZ+6cHCVrHZf0AYq3lLFY+5LNF/097Kew==;EndpointSuffix=core.windows.net";
        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
        
        // get container client object
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("files");
        
        // Create a local file in the ./data/ directory for uploading and downloading
        string localPath = "./";
        string fileName = $"{id}" + ".txt";
        string localFilePath = Path.Combine(localPath, fileName);

        // Write text to the file
        await File.WriteAllTextAsync(localFilePath, $"{squaredNumber}");

        // Get a reference to a blob
        BlobClient blobClient = containerClient.GetBlobClient(fileName);

        Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

        // Upload data from the local file
        await blobClient.UploadAsync(localFilePath, true);
    }
    
}