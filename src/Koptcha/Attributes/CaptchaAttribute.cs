using KKoptchaExtensions;
using Koptcha.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Koptcha.Attributes
{
    public class CaptchaAttribute : ActionFilterAttribute
    {
        public int Threshold { get; set; } = -1;
        public int Duration { get; set; } = -1;
        public CaptchaType CaptchaType { get; set; }
        public string FieldName { get; set; } = null;
        public string GlobalCacheName { get; set; } = null;
       
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheProvider = context.HttpContext.RequestServices.GetService<IDistributedCache>();
            var captchaOptions = context.HttpContext.RequestServices.GetService<IOptions<CaptchaOptions>>()?.Value ??
                                 new CaptchaOptions();
            var descriptor = context?.ActionDescriptor as ControllerActionDescriptor;

            if (!captchaOptions.IsCaptchaEnabled)
            {
                return;
            }
            
            string actionName = descriptor.ActionName;
            string controllerName = descriptor.ControllerName;

            if (captchaOptions.IsEnabledThreshold)
            {
                string thresholdKeyName = $"{controllerName}_{actionName}_{CaptchaType.ToString()}_Threshold";

                if (Threshold == -1)
                {
                    Threshold = captchaOptions.Threshold;
                }
            }

            if (captchaOptions.IsEnabledDuration)
            {
                string durationKeyName = $"{controllerName}_{actionName}_{CaptchaType.ToString()}_Duration";

                if (Duration == -1)
                {
                    Duration = captchaOptions.Duration;
                }
            }

            string cacheKeyPattern =
                context.GetCacheKey(captchaOptions.DomainName, CaptchaType, FieldName, GlobalCacheName);

            int cacheVisitedCount = cacheProvider.Get<int>(cacheKeyPattern);

            if (cacheVisitedCount >= Threshold)
            {
                var tokenHeader = context.HttpContext?.Request?.Headers?.TryGetValue(
                    captchaOptions.CaptchaTokenHeaderName,
                    out StringValues token);
                if (tokenHeader.HasValue && !string.IsNullOrEmpty(token))
                {
                    await CheckCaptchaTokenAsync(context, cacheProvider, cacheKeyPattern, cacheVisitedCount, captchaOptions, token.ToString());
                }
                else
                {
                    AddToCache(cacheProvider, cacheKeyPattern, cacheVisitedCount);
                    context.Result = new ContentResult()
                    {
                        Content = "Too Many Requests",
                        StatusCode = (int)HttpStatusCode.TooManyRequests
                    };
                }
            }
            else
            {
                AddToCache(cacheProvider, cacheKeyPattern, cacheVisitedCount);
            }

            await base.OnActionExecutionAsync(context, next);
        }

        private async Task CheckCaptchaTokenAsync(ActionExecutingContext context, IDistributedCache cacheProvider,
            string cacheKeyPattern, int cacheVisitedCount, CaptchaOptions captchaOptions, string token)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string url = captchaOptions.CaptchaControlBaseUrl
                        .Replace(captchaOptions.CaptchaSecretKeyHolder, captchaOptions.CaptchaSecretKey)
                        .Replace(captchaOptions.CaptchaTokenHolder, token);
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var captchaResultJson = await response.Content.ReadAsStringAsync();
                        var captchaResult = JsonConvert.DeserializeObject<CaptchaResponse>(captchaResultJson);

                        if (captchaResult.Success)
                        {
                            cacheProvider.Remove(cacheKeyPattern);
                        }
                        else
                        {
                            AddToCache(cacheProvider, cacheKeyPattern, cacheVisitedCount);
                            context.Result = new ContentResult()
                            {
                                Content = "Too Many Requests",
                                StatusCode = (int)HttpStatusCode.TooManyRequests
                            };
                        }
                    }
                    else
                    {
                        AddToCache(cacheProvider, cacheKeyPattern, cacheVisitedCount);
                        context.Result = new ContentResult()
                        {
                            Content = "Too Many Requests",
                            StatusCode = (int)HttpStatusCode.TooManyRequests
                        };
                    }
                }
            }
            catch (Exception)
            {
                AddToCache(cacheProvider, cacheKeyPattern, cacheVisitedCount);
                context.Result = new ContentResult()
                {
                    Content = "Too Many Requests",
                    StatusCode = (int)HttpStatusCode.TooManyRequests
                };
            }
        }

        private void AddToCache(IDistributedCache cacheProvider, string key, int visitedCount)
        {
            visitedCount++;
            cacheProvider.Set<int>(key, visitedCount, new DistributedCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromSeconds(Duration)
            });
        }
    }
}