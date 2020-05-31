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
    public interface IAzureStreamingService
    {
        public Task<IList<string>> UploadAndRetrieve();
    }
    class AzureStreamingService : IAzureStreamingService
    {
        private readonly IAzureMediaService _azureMediaService;
        private readonly IAzureMediaServicesClient _azureMediaServicesClient;
        private const string AdaptiveStreamingTransformName = "MyTransformWithAdaptiveStreamingPreset";
        private const string OutputFolderName = @"Output";

        private ILogger<AzureStreamingService> _logger;

        public AzureStreamingService(ILogger<AzureStreamingService> logger, IAzureMediaService azureMediaService, IAzureMediaServicesClient azureMediaServicesClient)
        {
            _logger = logger;
            _azureMediaService = azureMediaService;
            _azureMediaServicesClient = azureMediaServicesClient;
        }

        public async Task<IList<string>> UploadAndRetrieve()
        {
            IAzureMediaServicesClient client = _azureMediaServicesClient;

            client.LongRunningOperationRetryTimeout = 2;
            // TODO: Made a database model for jobs

            string uniqueness = Guid.NewGuid().ToString("N");
            string jobName = $"job-{uniqueness}";
            string locatorName = $"locator-{uniqueness}";
            string outputAssetName = $"output-{uniqueness}";

            // Ensure that you have the desired encoding Transform. This is really a one time setup operation.
            Transform transform = await _azureMediaService.GetOrCreateTransformAsync(AdaptiveStreamingTransformName);

            // Output from the encoding Job must be written to an Asset, so let's create one
            Asset outputAsset =
                await _azureMediaService.CreateOutputAssetAsync(outputAssetName);

            Job job = await _azureMediaService.SubmitJobAsync(AdaptiveStreamingTransformName, outputAsset.Name,
                jobName);

            job = await _azureMediaService.WaitForJobToFinishAsync(AdaptiveStreamingTransformName, jobName);

            if (job.State == JobState.Finished)
            {
                _logger.LogInformation("Job finished.");
                if (!Directory.Exists(OutputFolderName))
                    Directory.CreateDirectory(OutputFolderName);

                await _azureMediaService.DownloadOutputAssetAsync(outputAssetName, OutputFolderName);

                StreamingLocator locator =
                    await _azureMediaService.CreateStreamingLocatorAsync(outputAsset.Name, locatorName);

                IList<string> urls = await _azureMediaService.GetStreamingUrlsAsync(locator.Name);

                urls.ToList().ForEach(url => { _logger.LogInformation(url); });
                _logger.LogInformation(
                    "Done. Copy and paste one of the Streaming URLs into the Azure Media Player at 'http://aka.ms/azuremediaplayer'.");
                return urls;
                // TODO: Store the results in a database
            }

            return null;
        }
    }
}