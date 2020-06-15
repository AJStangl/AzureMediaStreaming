using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureMediaStreaming.Context.Assets;
using AzureMediaStreaming.Context.Assets.Models;
using AzureMediaStreaming.DataModels.Models;
using AzureMediaStreaming.DataModels.RequestResponse;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Logging;

namespace AzureMediaStreaming.AzureServices
{
    // TODO: Come up with a better name.
    public interface IAzureStreamingService
    {
        public Task<AssetEntity> UploadFileAsync(VideoUploadRequest videoRequest);
        public AssetEntity GetAssetById(string assetId);
        public Task<List<AssetEntity>> SearchForVideoAsync(VideoSearchResponse videoSearchResponse);
    }

    internal class AzureStreamingService : IAzureStreamingService
    {
        private readonly IAssetContext _assetContext;
        private readonly IAzureMediaMethods _azureMediaMethods;
        private readonly ILogger<AzureStreamingService> _logger;

        public AzureStreamingService(ILogger<AzureStreamingService> logger, IAzureMediaMethods azureMediaMethods,
            IAssetContext assetContext)
        {
            _logger = logger;
            _azureMediaMethods = azureMediaMethods;
            _assetContext = assetContext;
        }

        public async Task<AssetEntity> UploadFileAsync(VideoUploadRequest videoRequest)
        {
            _logger.LogInformation("Starting Upload...");

            var mediaAsset = MediaAsset.CreateInstance(videoRequest);

            var asset = await _assetContext.GetAssetsByName(mediaAsset.AssetName);

            if (asset != null) return asset;

            _logger.LogInformation("Creating input asset...");
            var inputAsset = await _azureMediaMethods.CreateInputAssetAsync(mediaAsset);

            _logger.LogInformation("Creating output asset...");
            var outputAsset =
                await _azureMediaMethods.CreateOutputAssetAsync(mediaAsset.InputAssetName, mediaAsset.OutputAssetName);

            if (inputAsset.Name == outputAsset.Name)
            {
                _logger.LogWarning("Input and output can't be the same.");
                return null;
            }

            _logger.LogInformation("Creating Transform...");
            var transform = await _azureMediaMethods.GetOrCreateTransformAsync();

            _logger.LogInformation("Starting job...");

            var job = await _azureMediaMethods.SubmitJobAsync(transform.Name, mediaAsset.JobName, inputAsset.Name,
                outputAsset.Name);

            var waitForJob = await _azureMediaMethods.WaitForJobToFinishAsync(transform.Name, job.Name);

            if (waitForJob.State != JobState.Finished)
            {
                _logger.LogError("The job failed to complete...");
                return null;
            }

            //TODO: Clean up output Input Artifact


            _logger.LogInformation("Creating streaming Locator...");
            var locator =
                await _azureMediaMethods.CreateStreamingLocatorAsync(outputAsset.Name, mediaAsset.LocatorName);

            _logger.LogInformation("Obtaining streaming urls...");
            var urls = await _azureMediaMethods.GetStreamingUrlsAsync(locator.Name);

            var streamingUrls = urls.Select(url => new StreamingUrl
            {
                Url = url,
                AssetEntityId = mediaAsset.UniqueId
            }).ToHashSet();

            mediaAsset.StreamingUrls = streamingUrls;
            _logger.LogInformation("Storing Results in database...");
            var assetEntity = await _assetContext.CreateUpdateAssetEntity(mediaAsset);
            return assetEntity;
        }

        public async Task<List<AssetEntity>> SearchForVideoAsync(VideoSearchResponse videoSearchResponse)
        {
            // TODO: More robust search mechanism -- Currently just pull top 10 most recent uploaded videos. Searching
            // Require it's own service.
            if (videoSearchResponse == null) return await _assetContext.SearchForAssets(null);
            // TODO: Don't just throw an exception. Use a strategy
            throw new NotImplementedException("Support of searching assets not supported.");
        }

        public AssetEntity GetAssetById(string assetId)
        {
            return _assetContext.GetAssetById(assetId);
        }
    }
}