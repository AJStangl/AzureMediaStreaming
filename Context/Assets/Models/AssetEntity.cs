using System;
using System.Collections.Generic;
using AzureMediaStreaming.Context.Common;
using AzureMediaStreaming.DataModels.Interfaces;
using Newtonsoft.Json;

namespace AzureMediaStreaming.Context.Assets.Models
{
    public class AssetEntity : BaseEntity
    {
        private AssetEntity()
        {
            StreamingUrl = new HashSet<StreamingUrl>();
        }

        public string FileName { get; set; }
        public string AssetName { get; set; }
        public string InputAssetName { get; set; }
        public string OutputAssetName { get; set; }
        public string JobName { get; set; }
        public string LocatorName { get; set; }
        public virtual ICollection<StreamingUrl> StreamingUrl { get; set; }
        public virtual AssetMetaDataEntity AssetMetaDataEntity { get; set; }

        internal static AssetEntity CreateInstance()
        {
            return new AssetEntity();
        }
    }

    public class AssetMetaDataEntity : BaseEntity, IAssetMetaData
    {
        [JsonIgnore] public virtual Guid AssetEntityId { get; set; }
        [JsonIgnore] public virtual AssetEntity AssetEntity { get; set; }
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

    public class StreamingUrl : BaseEntity
    {
        [JsonIgnore] public virtual Guid AssetEntityId { get; set; }
        [JsonIgnore] public virtual AssetEntity AssetEntity { get; set; }
        public string Url { get; set; }
    }
}