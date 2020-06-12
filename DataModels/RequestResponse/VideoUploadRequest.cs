using System;
using System.ComponentModel.DataAnnotations;
using AzureMediaStreaming.Controllers;
using Microsoft.AspNetCore.Http;

namespace AzureMediaStreaming.DataModels.RequestResponse
{
    /// <summary>
    ///     Request model for JS Component VideoUpload bound to <see cref="MediaController" /> POST Video request.
    /// </summary>
    public class VideoUploadRequest
    {
        [Required] public string FirstName { get; set; }

        [Required] public string LastName { get; set; }

        [Required] public string PhoneNumber { get; set; }

        [Required] public string Street { get; set; }

        [Required] public string ZipCode { get; set; }

        [Required] public string City { get; set; }

        [Required] public string State { get; set; }

        [Required] public DateTimeOffset? Date { get; set; }

        [Required] public DateTimeOffset? Time { get; set; }

        [Required] public IFormFile File { get; set; }
    }

    public class VideoUploadResponse
    {
    }
}