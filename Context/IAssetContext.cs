using System.Collections.Generic;
using System.Threading.Tasks;
using AzureMediaStreaming.DataModels.Context;
using AzureMediaStreaming.DataModels.Models;

namespace AzureMediaStreaming.Context
{
    public interface IAssetContext
    {
        public Task<AssetEntity> GetAssetsByName(string assetName);
        public Task CreateUpdateAssetEntity(MediaAsset mediaAsset);
        public List<StreamingUrl> GetStreamingUrl();
    }
}