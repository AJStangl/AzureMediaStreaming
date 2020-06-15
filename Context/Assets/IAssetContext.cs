using System.Collections.Generic;
using System.Threading.Tasks;
using AzureMediaStreaming.Context.Assets.Models;
using AzureMediaStreaming.DataModels.Models;
using AzureMediaStreaming.DataModels.RequestResponse;

namespace AzureMediaStreaming.Context.Assets
{
    public interface IAssetContext
    {
        public Task<AssetEntity> GetAssetsByName(string assetName);
        public Task<AssetEntity> CreateUpdateAssetEntity(MediaAsset mediaAsset);
        public Task<List<AssetEntity>> SearchForAssets(VideoSearchRequest videoSearchRequest);
        public AssetEntity GetAssetById(string assetId);
        public List<StreamingUrl> GetStreamingUrl();
    }
}