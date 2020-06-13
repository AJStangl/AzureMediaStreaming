using System;
using System.Linq;
using AzureMediaStreaming.Context.Models;
using AzureMediaStreaming.DataModels.Interfaces;

namespace AzureMediaStreaming.DataModels.RequestResponse
{
    public class VideoSearchRequest : IAddress
    {
        public DateTimeOffset? Date { get; set; }
        public DateTimeOffset? Time { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }

    public class VideoSearchResponse : IAddress
    {
        public string Id { get; set; }
        public string VideoName { get; set; }
        public string VideoUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        public static VideoSearchResponse CreateInstanceFromAsset(AssetEntity assetEntity)
        {
            return new VideoSearchResponse
            {
                Id = assetEntity.Id.ToString(),
                VideoName = assetEntity.FileName,
                VideoUrl = assetEntity.StreamingUrl?.FirstOrDefault()?.Url,
                Street = assetEntity.AssetMetaDataEntity?.Street,
                ZipCode = assetEntity.AssetMetaDataEntity?.ZipCode,
                City = assetEntity.AssetMetaDataEntity?.City,
                State = assetEntity.AssetMetaDataEntity?.State,
                CreatedDate = assetEntity.CreatedDate,
                Date = assetEntity.AssetMetaDataEntity?.Date.GetValueOrDefault().LocalDateTime.ToShortDateString(),
                Time = assetEntity.AssetMetaDataEntity?.Time.GetValueOrDefault().LocalDateTime.ToShortTimeString()
            };
        }
    }
}