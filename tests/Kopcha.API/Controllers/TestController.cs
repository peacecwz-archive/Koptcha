using System.Threading;
using Kopcha.Attributes;
using Kopcha.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kopcha.API.Controllers
{
    [ApiController]
    [Route("api/tests")]
    public class TestController : ControllerBase
    {
        // GET
        [Captcha(CaptchaType = CaptchaType.Ip, Duration = 5, Threshold = 20)]
        [HttpGet("test1")]
        public IActionResult Get()
        {
            return Ok("Test Ok");
        }
    }
}