using System;
using System.Linq;
using System.Threading.Tasks;
using AzureMediaStreaming.AzureServices;
using AzureMediaStreaming.Settings;
using AzureMediaStreaming.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AzureMediaStreaming.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly IAzureStreamingService _azureStreamingService;
        private readonly IAzureMediaService _azureMediaService;
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
            // TODO: All sorts of stuff to validate that this is a file we want to save.
            // Upload the file if less than ~30 MB
            if (formFile.Length < 30000000)
            {
                // TODO: Do stuff here like add a loading icon
                var foo = await _azureStreamingService.UploadFileAsync(formFile);
            }
            else
            {
                // TODO: Add some sort of useful error page
                ModelState.AddModelError("File", "The file is too large.");
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<VideoModel> Video()
        {
            // TODO: Implement Search for videos
            string locatorName = "locator-ca2fc45b-7b10-41de-a68e-baea2d532f5f-20200607_072358.mp4";
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

        public RedirectResult Error()
        {
            // TODO: Actually have this do something
            return Redirect("/Error");
        }
    }
}