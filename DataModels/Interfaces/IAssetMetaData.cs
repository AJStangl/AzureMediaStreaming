using System;

namespace AzureMediaStreaming.DataModels.Interfaces
{
    public interface IAssetMetaData : IAddress
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public DateTimeOffset? Date { get; set; }

        public DateTimeOffset? Time { get; set; }
    }
}