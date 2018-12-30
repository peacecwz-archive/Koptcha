using System.Diagnostics;
using Kopcha.Attributes;
using Kopcha.Models;
using Microsoft.AspNetCore.Mvc;
using Kopcha.Web.Models;

namespace Kopcha.Web.Controllers
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
