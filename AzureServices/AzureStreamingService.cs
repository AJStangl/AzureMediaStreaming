using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AzureMediaStreaming.Settings;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Rest.Azure;

namespace AzureMediaStreaming.AzureServices
{
    class AzureStreamingService
    {
        private const string AdaptiveStreamingTransformName = "MyTransformWithAdaptiveStreamingPreset";
        private const string OutputFolderName = @"Output";

        private ILogger<AzureStreamingService> _logger;
        public AzureStreamingService(ILogger<AzureStreamingService> logger)
        {
            _logger = logger;
        }

        // private async Task<IList<string>> UploadAndRetrieve()
        // {
        //     IAzureMediaServicesClient client = await _azureClient.CreateMediaServicesClientAsync();
        //
        //     client.LongRunningOperationRetryTimeout = 2;
        //     // TODO: Made a database model for jobs
        //
        //     string uniqueness = Guid.NewGuid().ToString("N");
        //     string jobName = $"job-{uniqueness}";
        //     string locatorName = $"locator-{uniqueness}";
        //     string outputAssetName = $"output-{uniqueness}";
        //
        //     // Ensure that you have the desired encoding Transform. This is really a one time setup operation.
        //     Transform transform = await _azureClient.GetOrCreateTransformAsync(client, AdaptiveStreamingTransformName);
        //
        //     // Output from the encoding Job must be written to an Asset, so let's create one
        //     Asset outputAsset =
        //         await _azureClient.CreateOutputAssetAsync(client, outputAssetName);
        //
        //     Job job = await _azureClient.SubmitJobAsync(client, AdaptiveStreamingTransformName, outputAsset.Name, jobName);
        //
        //     job = await _azureClient.WaitForJobToFinishAsync(client, AdaptiveStreamingTransformName, jobName);
        //
        //     if (job.State == JobState.Finished)
        //     {
        //         _logger.LogInformation("Job finished.");
        //         if (!Directory.Exists(OutputFolderName))
        //             Directory.CreateDirectory(OutputFolderName);
        //
        //         await _azureClient.DownloadOutputAssetAsync(client, outputAssetName, OutputFolderName);
        //
        //         StreamingLocator locator = await _azureClient.CreateStreamingLocatorAsync(client, outputAsset.Name, locatorName);
        //
        //         IList<string> urls = await _azureClient.GetStreamingUrlsAsync(client, locator.Name);
        //
        //         urls.ToList().ForEach(url =>
        //         {
        //             _logger.LogInformation(url);
        //         });
        //         _logger.LogInformation("Done. Copy and paste one of the Streaming URLs into the Azure Media Player at 'http://aka.ms/azuremediaplayer'.");
        //         return urls;
        //         // TODO: Store the results in a database
        //     }
        //     return null;

        }
}