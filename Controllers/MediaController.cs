using System;
using System.Linq;
using System.Threading.Tasks;
using AzureMediaStreaming.ActionResults;
using AzureMediaStreaming.AzureServices;
using AzureMediaStreaming.DataModels;
using AzureMediaStreaming.DataModels.RequestResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AzureMediaStreaming.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly IAzureMediaService _azureMediaService;
        private readonly IAzureStreamingService _azureStreamingService;
        private readonly ILogger<MediaController> _logger;

        public MediaController(
            IAzureStreamingService azureStreaming,
            IAzureMediaService azureMediaService,
            ILogger<MediaController> logger)
        {
            _azureMediaService = azureMediaService;
            _azureStreamingService = azureStreaming;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Video()
        {
            var locatorName = "locator-ca2fc45b-7b10-41de-a68e-baea2d532f5f-20200607_072358.mp4";
            _logger.LogInformation("Getting streaming");
            try
            {
                var videoUrls = await _azureMediaService.GetStreamingUrlsAsync(locatorName);
                var videoUrl = videoUrls.FirstOrDefault();

                return Ok(new VideoModel
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
                    ErrorType.Generic);
                return new VideoResult(videoResultError, 500);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Video([FromForm] VideoUploadRequest videoUploadRequest)
        {
            _logger.LogInformation("Starting upload...");
            await Task.Delay(new TimeSpan(0, 0, 3));

            var foo = VideoResultError.CreateInstance(
                "An error occured while uploading the video.",
                HttpContext,
                ErrorType.Generic);

            var result = new VideoResult(foo, StatusCodes.Status422UnprocessableEntity);
            return result;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Search()
        {
            throw new NotImplementedException($"{Request.Path.Value} is not active");
        }
    }
}