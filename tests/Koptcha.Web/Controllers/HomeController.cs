using System.Diagnostics;
using Koptcha.Attributes;
using Koptcha.Models;
using Microsoft.AspNetCore.Mvc;
using Koptcha.Web.Models;

namespace Koptcha.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Captcha(CaptchaType = CaptchaType.Ip, Duration = 5, Threshold = 3)]
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
