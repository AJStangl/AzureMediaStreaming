using System.Collections.Generic;
using System.Threading.Tasks;
using AzureMediaStreaming.DataModels.Models;
using Microsoft.Azure.Management.Media.Models;

namespace AzureMediaStreaming.AzureServices
{
    public interface IAzureMediaMethods
    {
        public Task<Asset> CreateInputAssetAsync(MediaAsset mediaAsset);
        public Task<Asset> CreateOutputAssetAsync(string inputAssetName, string outputAssetName);

        public Task<Job> SubmitJobAsync(string transformName, string jobName, string inputAssetName,
            string outputAssetName);

        public Task<Job> WaitForJobToFinishAsync(string transformName, string jobName);
        public Task<StreamingLocator> CreateStreamingLocatorAsync(string assetName, string locatorName);
        public Task<IList<string>> GetStreamingUrlsAsync(string locatorName);
        public Task DownloadOutputAssetAsync(string assetName);
        public Task CleanUpAsync(string transformName, List<string> assetNames, string jobName);
        public Task<Transform> GetOrCreateTransformAsync();
    }
}