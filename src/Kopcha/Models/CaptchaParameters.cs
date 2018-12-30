using System.Collections.Generic;

namespace Kopcha.Models
{
    public class CaptchaParameters
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string GlobalCacheName { get; set; }
        public string CaptchaToken { get; set; }
        public string IpAddress { get; set; }
        public List<CaptchaField> Fields { get; set; }
    }
}