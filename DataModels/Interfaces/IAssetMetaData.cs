using System;

namespace AzureMediaStreaming.DataModels.Interfaces
{
    public interface IAssetMetaData
    {
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
}