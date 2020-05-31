using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureMediaStreaming.AzureServices
{
    public interface IAzureMediaService
    {
        public Task<Asset> CreateOutputAssetAsync(string assetName);
        public Task<Job> SubmitJobAsync(string transformName, string outputAssetName, string jobName);
        public Task<Job> WaitForJobToFinishAsync(string transformName, string jobName);
        public Task<StreamingLocator> CreateStreamingLocatorAsync(string assetName, string locatorName);
        public Task<IList<string>> GetStreamingUrlsAsync(string locatorName);
        public Task DownloadOutputAssetAsync(string assetName, string outputFolderName);
        public Task CleanUpAsync(string transformName, List<string> assetNames, string jobName);
        public Task<Transform> GetOrCreateTransformAsync(string transformName);
    }
}