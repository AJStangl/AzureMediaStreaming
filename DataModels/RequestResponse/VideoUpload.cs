using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AzureMediaStreaming.Context.Models;
using AzureMediaStreaming.Controllers;
using AzureMediaStreaming.DataModels.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AzureMediaStreaming.DataModels.RequestResponse
{
    /// <summary>
    ///     Request model for JS Component VideoUpload bound to <see cref="MediaController" /> POST Video request.
    /// </summary>
    public class VideoUploadRequest : IAssetMetaData, IAddress
    {
        [Required] public IFormFile File { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public DateTimeOffset? Date { get; set; }
        public DateTimeOffset? Time { get; set; }
    }

    // TODO: Implement Members
    public class VideoUploadResponse
    {
        public string VideoId { get; set; }
        public string FileName { get; set; }
        public string StreamingUrl { get; set; }

        public static VideoUploadResponse CreateInstanceFromAsset(AssetEntity assetEntity)
        {
            return new VideoUploadResponse
            {
                VideoId = assetEntity.Id.ToString(),
                FileName = assetEntity.FileName,
                StreamingUrl = assetEntity.StreamingUrl.FirstOrDefault()?.Url
            };
        }
    }
}