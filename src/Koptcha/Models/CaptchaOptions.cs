namespace KKoptchaModels
{
    public class CaptchaOptions
    {
        public bool IsEnabledThreshold { get; set; } = false;
        public bool IsEnabledDuration { get; set; } = true;
        public int Threshold { get; set; } = 5;
        public int Duration { get; set; } = 60;
        public string DomainName { get; set; }
        public string CaptchaTokenHeaderName { get; set; } = "x-captcha-token";
        public string CaptchaControlBaseUrl { get; set; }
        public string CaptchaSecretKeyHolder { get; set; } = "{secretkey}";
        public string CaptchaTokenHolder { get; set; } = "{token}";
        public string CaptchaSecretKey { get; set; }
        public bool IsCaptchaEnabled { get; set; }
    }
}