using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AzureMediaStreaming.AzureServices;
using AzureMediaStreaming.DataModels;
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

        [HttpPost]
        [Route("[action]")]
        public async Task Video([FromForm(Name = "file")] IFormFile formFile)
        {
            throw new FileLoadException("The file is too large.\\nSubmit a file less that 30 Mb.");
            // Upload the file if less than ~30 MB
            if (formFile.Length < 30000000)
            {
                // TODO: Do stuff here like add a loading icon
                var foo = await _azureStreamingService.UploadFileAsync(formFile);
            }
            else
            {
                throw new FileLoadException("The file is too large.\\nSubmit a file less that 30 Mb.");
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<VideoModel>> Video()
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
                throw new Exception("An error has occured while obtaining the video.");
            }
        }
    }
}