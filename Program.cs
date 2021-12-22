using Microsoft.Azure.Storage.Blob;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Azure Blob Storage Demo\n");

            ProcessAsync().GetAwaiter().GetResult();

        }
       
        private static async Task ProcessAsync()
        {
            string storageConnectionString = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
            CloudStorageAccount storageAccount;

            if(CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                Console.WriteLine("Valid connection string.\r\n");

                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference($"demoblobs{Guid.NewGuid().ToString()}");

                await  cloudBlobContainer.CreateIfNotExistsAsync();

                Console.WriteLine("Um container foi criado");
                Console.ReadKey();

                string localPath = Path.GetTempPath();
                string localFileName = "BlobDemo_" + Guid.NewGuid().ToString() + ".txt";
                string sourceFile = Path.Combine(localPath, localFileName);

                File.WriteAllText(sourceFile, "Hello World!");

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(localFileName);
                await cloudBlockBlob.UploadFromFileAsync(sourceFile);

                Console.WriteLine("Upload success");
                Console.ReadKey();

                BlobContinuationToken blobContinuationToken = null;

                do
                {
                    var results = await cloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                    
                    foreach (IListBlobItem item in results.Results)
                    {
                        Console.WriteLine(item.Uri);
                    }

                }while(blobContinuationToken != null);

                string destinationFile = sourceFile.Replace(".txt", "_DOWNLOADED.txt");
                Console.WriteLine($"Downloading blob to {destinationFile}");
                await cloudBlockBlob.DownloadToFileAsync(destinationFile,FileMode.Create);

                 Console.WriteLine($"Downloading blob to {destinationFile} - Success");
                 Console.ReadKey();

                 await cloudBlobContainer.DeleteIfExistsAsync();







            }
            else
            {
                Console.WriteLine("Invalid connection string");
            } 

        }

    }
}


// See https://aka.ms/new-console-template for more information




