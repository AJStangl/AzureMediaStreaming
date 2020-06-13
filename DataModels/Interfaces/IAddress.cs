namespace AzureMediaStreaming.DataModels.Interfaces
{
    public interface IAddress
    {
        public string Street { get; set; }

        public string ZipCode { get; set; }

        public string City { get; set; }

        public string State { get; set; }
    }
}