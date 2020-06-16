using System;
using System.Linq;
using System.Threading.Tasks;
using AzureMediaStreaming.ActionResults;
using AzureMediaStreaming.AzureServices;
using AzureMediaStreaming.DataModels.RequestResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AzureMediaStreaming.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly IAzureMediaMethods _azureMediaMethods;
        private readonly IAzureStreamingService _azureStreamingService;
        private readonly ILogger<MediaController> _logger;

        public MediaController(
            IAzureStreamingService azureStreaming,
            IAzureMediaMethods azureMediaMethods,
            ILogger<MediaController> logger)
        {
            _azureMediaMethods = azureMediaMethods;
            _azureStreamingService = azureStreaming;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Video()
        {
            var locatorName = "locator-ca2fc45b-7b10-41de-a68e-baea2d532f5f-20200607_072358.mp4";
            _logger.LogInformation("Getting streaming...");
            try
            {
                var videoUrls = await _azureMediaMethods.GetStreamingUrlsAsync(locatorName);
                var videoUrl = videoUrls.FirstOrDefault();

                return Ok(new VideoStreamResponse
                {
                    VideoName = "Demo Video",
                    VideoUrl = videoUrl
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error has occured trying to obtain data.");
                var videoResultError = VideoResultError.CreateInstance(
                    "An error has occured trying to obtain data.",
                    HttpContext,
                    ErrorType.InternalServer);
                return new VideoResponse(videoResultError, 500);
            }
        }

        [HttpGet]
        [Route("[action]/{videoId}")]
        public IActionResult Video(string videoId)
        {
            var assetEntity = _azureStreamingService.GetAssetById(videoId);
            // TODO: I'm not sure if we really need to do what we do in the method above. It might be sufficient to use
            // what we stored in the Database
            if (string.IsNullOrWhiteSpace(assetEntity.StreamingUrl.FirstOrDefault()?.Url))
                return new VideoResponse(
                    VideoResultError.CreateInstance("Video Not Found", HttpContext, ErrorType.NotFound),
                    StatusCodes.Status404NotFound);

            return Ok(new VideoStreamResponse
            {
                VideoName = assetEntity.FileName,
                VideoUrl = assetEntity.StreamingUrl.FirstOrDefault()?.Url
            });
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Video([FromForm] VideoUploadRequest videoUploadRequest)
        {
            _logger.LogInformation("Starting upload...");
            try
            {
                var result = await _azureStreamingService.UploadFileAsync(videoUploadRequest);
                return new VideoResponse(
                    VideoUploadResponse.CreateInstanceFromAsset(result),
                    StatusCodes.Status200OK);
            }
            catch (Exception exception)
            {
                _logger.LogError("Error uploading", exception);
                var error = VideoResultError.CreateInstance(
                    "An error has occured attempting to upload the video.",
                    HttpContext,
                    ErrorType.InternalServer);

                return new VideoResponse(error, StatusCodes.Status422UnprocessableEntity);
            }
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Identity.Application")]
        [Route("[action]")]
        public async Task<IActionResult> LatestVideo()
        {
            try
            {
                _logger.LogInformation("Getting latest videos...");
                var result = await _azureStreamingService.SearchForVideoAsync(null);
                var videoSearchResponses = result.Select(VideoSearchResponse.CreateInstanceFromAsset);
                return new VideoResponse(videoSearchResponses, StatusCodes.Status200OK);
            }
            catch (Exception exception)
            {
                _logger.LogError($"An error has occured during: {HttpContext.Request.Path}", exception);
                return new VideoResponse(
                    VideoResultError.CreateInstance("An error has occured", HttpContext, ErrorType.InternalServer),
                    StatusCodes.Status500InternalServerError);
            }
        }
    }
}