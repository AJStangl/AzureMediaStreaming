using System;
using System.Linq;
using System.Threading.Tasks;
using AzureMediaStreaming.AzureServices;
using AzureMediaStreaming.Settings;
using AzureMediaStreaming.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AzureMediaStreaming.Controllers
{
    [Route("[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly IAzureMediaService _azureMediaService;
        private readonly ILogger<MediaController> _logger;
        public MediaController(IAzureMediaService azureMediaService, ILogger<MediaController> logger)
        {
            _azureMediaService = azureMediaService;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<VideoModel> Video()
        {
            // TODO: Implement everything
            string locatorName = "locator-c7943896de4d4cb6a6484fc878028fd7";
            _logger.LogInformation("Getting streaming");
            try
            {
                var videoUrls = await _azureMediaService.GetStreamingUrlsAsync(locatorName);
                string videoUrl = videoUrls.FirstOrDefault();

                return new VideoModel
                {
                    VideoName = "Demo Video",
                    VideoUrl = videoUrl
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error has occured trying to obtain data");
                throw;
            }

        }

        // [HttpGet]
        // [Route("[action]")]
        // public async Task Doit([FromServices] IAzureStreamingService azureStreamingService)
        // {
        //     await azureStreamingService.UploadAndRetrieve();
        // }

        // [HttpGet]
        // [Route("[action]")]
        // public IActionResult Settings([FromServices] IConfiguration configuration)
        // {
        //     return Ok(configuration.GetSection(nameof(ClientSettings)).Get<ClientSettings>());
        // }
    }
}