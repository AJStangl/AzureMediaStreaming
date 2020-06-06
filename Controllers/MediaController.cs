﻿using System;
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
    [ApiController]
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
        public async Task<IActionResult> Video()
        {
            // TODO: Implement everything
            string locatorName = "locator-c7943896de4d4cb6a6484fc878028fd7";
            _logger.LogInformation("Getting streaming");
            try
            {
                var videoUrls = await _azureMediaService.GetStreamingUrlsAsync(locatorName);
                string videoUrl = videoUrls.FirstOrDefault();

                return Ok(new VideoModel
                {
                    VideoName = "Demo Video",
                    VideoUrl = videoUrl
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error has occured trying to obtain data");
                return BadRequest(e);
                throw;
            }
        }

        [HttpGet]
        public VideoModel Get()
        {
            return new VideoModel
            {
                VideoName = "Demo Video",
                VideoUrl =
                    "https://ajstangl-usea.streaming.media.azure.net/fc8b5b35-a2ca-4dbf-99c8-4740726e2529/Ignite-short.ism/manifest(format=m3u8-aapl)"
            };
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Settings([FromServices] IConfiguration configuration)
        {
            return Ok(configuration.GetSection(nameof(ClientSettings)).Get<ClientSettings>());
        }
    }
}