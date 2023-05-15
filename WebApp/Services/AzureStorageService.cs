using Azure;
using Azure.Storage.Blobs;
using Domain.Services;
using Azure.Storage;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs.Models;

namespace WebApp.Services
{
    public class AzureStorageService : IAzureStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<AzureStorageService> _logger;
        private readonly string _containerName;
        private readonly EventId _eventId;

        public AzureStorageService(BlobServiceClient blobServiceClient, ILogger<AzureStorageService> logger)
        {
            _blobServiceClient = blobServiceClient;
            _logger = logger;
            _containerName = "blog";
            _eventId = new EventId(301, name: "AzureStorageService");
        }

        /// <summary>
        /// Generates a file name in format : <paramref name="containerName"/>-Guid-Date
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public static string GenerateFileName(string containerName) => $"{containerName}-{Guid.NewGuid()}-{DateTime.UtcNow:dd-M-yy}";

        public async Task<string> UploadImageAsync(string imgType, Stream imgStream)
        {
            try
            {
                _logger.LogInformation(_eventId, "Attempting to upload image.");
                BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

                string imgId = GenerateFileName(_containerName);

                var response = await blobContainerClient.UploadBlobAsync(imgId, imgStream, CancellationToken.None);

                if (response.GetRawResponse().Status == 201)
                {
                    _logger.LogInformation(_eventId, "Image uploaded successfully.");

                    var blobImg = blobContainerClient.GetBlobClient(imgId);
                    await blobImg.SetHttpHeadersAsync(new BlobHttpHeaders { ContentType = imgType });

                    return blobImg.Uri.AbsoluteUri;
                }
            }
            catch (RequestFailedException ex)
            {
                _logger.LogCritical(_eventId, "AZURE STORAGE - REQUEST FAILED", ex);
            }

            return "";
        }

        //public async Task UploadImageFromStreamAsync()
        //{
        //    BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        //    blobContainerClient.UploadBlobAsync()
        //}

        //public async Task<BlobContainerClient> CreateContainerAsync(string containerName)
        //{
        //    try
        //    {
        //        BlobContainerClient container = await _blobServiceClient.GetBlobContainerClient()
        //    }
        //}
    }
}