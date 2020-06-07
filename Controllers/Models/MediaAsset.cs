using System;
using System.Collections.Generic;
using AzureMediaStreaming.Context.Models;
using Microsoft.AspNetCore.Http;

namespace AzureMediaStreaming.Controllers.Models
{
    public class MediaAsset
    {
        public MediaAsset(Guid guid, IFormFile formFile)
        {
            UniqueId = guid;
            FormFile = formFile;
            AssetName = $"{UniqueId:D}-{formFile.FileName}";
            InputAssetName = $"Input-{UniqueId:D}-{formFile.FileName}";
            OutputAssetName = $"Output-{UniqueId:D}-{formFile.FileName}";
            JobName = $"job-{AssetName}";
            LocatorName = $"locator-{AssetName}";
        }

        public Guid UniqueId { get; set; }
        public IFormFile FormFile { get; set; }
        public string AssetName { get; set; }
        public string InputAssetName { get; set; }
        public string OutputAssetName { get; set; }
        public string JobName { get; set; }
        public string LocatorName { get; set; }
        public ICollection<StreamingUrl> StreamingUrls { get; set; }
        public AssetEntity CopyToAssetEntity()
        {
            return new AssetEntity
            {
                Id = UniqueId,
                FileName = FormFile.FileName,
                AssetName = AssetName,
                InputAssetName = InputAssetName,
                OutputAssetName = OutputAssetName,
                JobName = JobName,
                LocatorName = LocatorName,
                StreamingUrl = StreamingUrls
            };
        }
    }
}