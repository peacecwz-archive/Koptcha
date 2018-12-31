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
        
        [Captcha(CaptchaType = CaptchaType.Email, Duration = 5, Threshold = 20, FieldName = "EmailAddress")]
        [HttpPost("test2")]
        public IActionResult GetWithEmail([FromBody]EmailRequest request)
        {
            return Ok($"Test Ok with {request.EmailAddress}");
        }
        
        [Captcha(CaptchaType = CaptchaType.IpAndEmail, Duration = 5, Threshold = 20, FieldName = "EmailAddress")]
        [HttpPost("test3")]
        public IActionResult GetWithEmailAndIp([FromBody]EmailRequest request)
        {
            return Ok($"Test Ok with {request.EmailAddress}");
        }
        
        [Captcha(CaptchaType = CaptchaType.Custom, Duration = 5, Threshold = 20, FieldName = "Username")]
        [HttpPost("test4")]
        public IActionResult GetWithUsername([FromBody]EmailRequest request)
        {
            return Ok($"Test Ok with {request.Username}");
        }
    }

    public class EmailRequest
    {
        public string EmailAddress { get; set; }
        public string Username { get; set; }
    }
}