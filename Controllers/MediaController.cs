using System.Linq;
using System.Threading.Tasks;
using AzureMediaStreaming.AzureServices;
using AzureMediaStreaming.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace AzureMediaStreaming.Controllers
{
    [Route("[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly IAzureMediaService _azureMediaService;
        public MediaController(IAzureMediaService azureMediaService)
        {
            _azureMediaService = azureMediaService;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<VideoModel> Video()
        {
            // TODO: Implement everything
            string locatorName = "locator-c7943896de4d4cb6a6484fc878028fd7";
            var videoUrls = await _azureMediaService.GetStreamingUrlsAsync(locatorName);
            string videoUrl = videoUrls.FirstOrDefault();

            return new VideoModel
            {
                VideoName = "Demo Video",
                VideoUrl = videoUrl
            };
        }

        [HttpGet]
        [Route("[action]")]
        public async Task Doit([FromServices] IAzureStreamingService azureStreamingService)
        {
            await azureStreamingService.UploadAndRetrieve();

        }
    }
}