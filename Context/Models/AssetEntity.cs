using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AzureMediaStreaming.Context.Models
{
    public class AssetEntity : BaseEntity
    {
        public AssetEntity()
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
    }

    public class StreamingUrl : BaseEntity
    {
        [JsonIgnore] public virtual Guid AssetEntityId { get; set; }
        [JsonIgnore] public virtual AssetEntity AssetEntity { get; set; }
        public string Url { get; set; }
    }
}