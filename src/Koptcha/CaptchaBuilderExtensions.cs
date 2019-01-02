using System;
using Koptcha.Attributes;
using Koptcha.Models;
using Koptcha.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Koptcha
{
    public static class CaptchaBuilderExtensions
    {
        public static IServiceCollection AddCaptcha(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
            var captchaConfiguration = configuration?.GetSection("Captcha");
            if (captchaConfiguration != null)
            {
                services.Configure<CaptchaOptions>(options =>
                {
                    if (string.IsNullOrEmpty(captchaConfiguration["DomainName"]))
                    {
                        throw new Exception("You have to add configuration");
                    }
                    options.CaptchaControlBaseUrl = captchaConfiguration["CaptchaControlBaseUrl"];
                    options.CaptchaSecretKey = captchaConfiguration["CaptchaSecretKey"];
                    options.CaptchaSecretKeyHolder = captchaConfiguration["CaptchaSecretKeyHolder"];
                    options.CaptchaTokenHeaderName = captchaConfiguration["CaptchaTokenHeaderName"];
                    options.DomainName = captchaConfiguration["DomainName"];

                    int.TryParse(captchaConfiguration["Duration"], out int duration);
                    options.Duration = duration;
                    int.TryParse(captchaConfiguration["Threshold"], out int threshold);
                    options.Threshold = threshold;

                    bool.TryParse(captchaConfiguration["IsCaptchaEnabled"], out bool isCaptchaEnabled);
                    options.IsCaptchaEnabled = isCaptchaEnabled;

                    bool.TryParse(captchaConfiguration["IsEnabledDuration"], out bool isEnabledDuration);
                    options.IsEnabledDuration = isEnabledDuration;

                    bool.TryParse(captchaConfiguration["IsEnabledThreshold"], out bool isEnabledThreshold);
                    options.IsEnabledThreshold = isEnabledThreshold;
                });
            }
            services.AddTransient<ICaptchaService, CaptchaService>();
            return services;
        }
    }
}