﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AzureMediaStreaming.Context.Models;
using AzureMediaStreaming.DataModels.Interfaces;
using AzureMediaStreaming.DataModels.RequestResponse;
using Microsoft.AspNetCore.Http;

namespace AzureMediaStreaming.DataModels.Models
{
    public class MediaAsset
    {
        public Guid UniqueId { get; set; }
        public IFormFile FormFile { get; set; }
        public string AssetName { get; set; }
        public string InputAssetName { get; set; }
        public string OutputAssetName { get; set; }
        public string JobName { get; set; }
        public string LocatorName { get; set; }
        public ICollection<StreamingUrl> StreamingUrls { get; set; }
        public AssetMetaData AssetMetaData { get; set; }

        public static MediaAsset CreateInstance(VideoUploadRequest videoUploadRequest)
        {
            var uniqueId = Guid.NewGuid();
            var uniqueIdString = uniqueId.ToString("D");
            var assetName = $"{uniqueIdString}-{videoUploadRequest.File.FileName}";
            return new MediaAsset
            {
                UniqueId = Guid.NewGuid(),
                FormFile = videoUploadRequest.File,
                AssetName = assetName,
                InputAssetName = $"Input-{uniqueIdString}-{videoUploadRequest.File.FileName}",
                OutputAssetName = $"Output-{uniqueIdString}-{videoUploadRequest.File.FileName}",
                JobName = $"job-{assetName}",
                LocatorName = $"locator-{assetName}",
                AssetMetaData = new AssetMetaData
                {
                    FirstName = videoUploadRequest.FirstName,
                    LastName = videoUploadRequest.LastName,
                    PhoneNumber = videoUploadRequest.PhoneNumber,
                    Street = videoUploadRequest.Street,
                    ZipCode = videoUploadRequest.ZipCode,
                    City = videoUploadRequest.City,
                    State = videoUploadRequest.State,
                    Date = videoUploadRequest.Date,
                    Time = videoUploadRequest.Time
                }
            };
        }
    }

    /// <summary>
    ///     Provides metadata from the uploaded file.
    /// </summary>
    public class AssetMetaData : IAssetMetaData
    {
        [MaxLength(100)] public string FirstName { get; set; }

        [MaxLength(100)] public string LastName { get; set; }

        [MaxLength(100)] public string PhoneNumber { get; set; }

        [MaxLength(100)] public string Street { get; set; }

        [MaxLength(100)] public string ZipCode { get; set; }

        [MaxLength(100)] public string City { get; set; }

        [MaxLength(100)] public string State { get; set; }

        public DateTimeOffset? Date { get; set; }
        public DateTimeOffset? Time { get; set; }
    }
}