using Microsoft.AspNetCore.Mvc;

namespace AzureMediaStreaming.Controllers
{
    [Route("[controller]")]
    public class MediaController : Controller
    {
        public IActionResult Video()
        {
            return Ok("Hello World");
        }
    }
}