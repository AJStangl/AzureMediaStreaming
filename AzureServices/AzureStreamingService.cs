using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureMediaStreaming.Context;
using AzureMediaStreaming.Context.Models;
using AzureMediaStreaming.Controllers.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Logging;

namespace AzureMediaStreaming.AzureServices
{
    public interface IAzureStreamingService
    {
        public Task<IList<string>> UploadFileAsync(IFormFile formFile);
    }

    class AzureStreamingService : IAzureStreamingService
    {
        private readonly IAzureMediaService _azureMediaService;
        private readonly IAssetContext _assetContext;
        private readonly ILogger<AzureStreamingService> _logger;

        public AzureStreamingService(ILogger<AzureStreamingService> logger, IAzureMediaService azureMediaService, IAssetContext assetContext)
        {
            _logger = logger;
            _azureMediaService = azureMediaService;
            _assetContext = assetContext;
        }

        public async Task<IList<string>> UploadFileAsync(IFormFile formFile)
        {
            _logger.LogInformation("Starting Upload...");
            var assetFile = new MediaAsset(Guid.NewGuid(), formFile);
            var asset = await _assetContext.GetAssetsByName(assetFile.FormFile.FileName);

            if (asset != null)
            {
                return asset.StreamingUrl.Select(x => x.Url).ToList();
            }

            _logger.LogInformation("Creating input asset...");
            var inputAsset = await _azureMediaService.CreateInputAssetAsync(assetFile);

            _logger.LogInformation("Creating output asset...");
            var outputAsset =
                await _azureMediaService.CreateOutputAssetAsync(assetFile.InputAssetName, assetFile.OutputAssetName);
            if (inputAsset.Name == outputAsset.Name)
            {
                _logger.LogWarning("Input and output can't be the same.");
                return null;
            }

            _logger.LogInformation("Creating Transform...");
            var transform = await _azureMediaService.GetOrCreateTransformAsync();

            _logger.LogInformation("Starting job...");
            var job = await _azureMediaService.SubmitJobAsync(transform.Name, assetFile.JobName, inputAsset.Name,
                outputAsset.Name);

            var waitForJob = await _azureMediaService.WaitForJobToFinishAsync(transform.Name, job.Name);

            if (waitForJob.State != JobState.Finished)
            {
                _logger.LogError("The job failed to complete...");
                return null;
            }

            _logger.LogInformation("Creating streaming Locator...");
            var locator =
                await _azureMediaService.CreateStreamingLocatorAsync(outputAsset.Name, assetFile.LocatorName);

            _logger.LogInformation("Obtaining streaming urls...");
            var urls = await _azureMediaService.GetStreamingUrlsAsync(locator.Name);

            var streamingUrls = urls.Select(url => new StreamingUrl
            {
                Url = url,
                AssetEntityId = assetFile.UniqueId,
            }).ToHashSet();

            assetFile.StreamingUrls = streamingUrls;
            _logger.LogInformation("Storing Results in database...");
            await _assetContext.CreateUpdateAssetEntity(assetFile);
            return urls;
        }
    }
}