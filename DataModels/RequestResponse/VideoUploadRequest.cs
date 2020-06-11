using System;
using Microsoft.AspNetCore.Http;

namespace AzureMediaStreaming.DataModels.RequestResponse
{
    public class VideoUploadRequest
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
        public IFormFile File { get; set; }
    }

    public class VideoUploadResponse
    {
    }
}