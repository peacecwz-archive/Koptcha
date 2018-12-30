using System;
using System.Net.Http;
using System.Threading.Tasks;
using Kopcha.Extensions;
using Kopcha.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Kopcha.Services
{
    public class CaptchaService : ICaptchaService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly CaptchaOptions _captchaOptions;
        public CaptchaService(IDistributedCache distributedCache,
            IOptions<CaptchaOptions> captchaOptions)
        {
            _distributedCache = distributedCache;
            _captchaOptions = captchaOptions?.Value;
        }

        public async Task<bool> CheckCaptchaAsync(CaptchaParameters captchaParameters)
        {
            bool result = false;

            if (!_captchaOptions.IsCaptchaEnabled)
            {
                return result;
            }

            foreach (var captchaField in captchaParameters.Fields)
            {
                if (captchaField.IsEnabledThreshold)
                {
                    string thresholdKeyName =
                        $"{captchaParameters.ControllerName}_{captchaParameters.ActionName}_{captchaField.FieldType.ToString()}_Threshold";

                    if (captchaField.Threshold == -1)
                    {
                        captchaField.Threshold = _captchaOptions.Threshold;
                    }
                }

                if (captchaField.IsEnabledDuration)
                {
                    string durationKeyName = $"{captchaParameters.ControllerName}_{captchaParameters.ActionName}_{captchaField.FieldType.ToString()}_Duration";

                    if (captchaField.Threshold == -1)
                    {
                        captchaField.Duration = _captchaOptions.Duration;
                    }
                }

                string cacheKeyPattern = captchaField.GetCacheKey(_captchaOptions.DomainName,
                    captchaParameters.ControllerName, captchaParameters.ActionName, captchaParameters.GlobalCacheName,
                    captchaParameters.IpAddress);

                int cacheVisitCount = _distributedCache.Get<int>(cacheKeyPattern);
                if (cacheVisitCount >= captchaField.Threshold)
                {
                    if (!string.IsNullOrEmpty(captchaParameters.CaptchaToken))
                    {
                        result = await CheckCaptchaTokenAsync(cacheKeyPattern, cacheVisitCount, captchaField.Duration,
                            captchaParameters.CaptchaToken);
                    }
                    else
                    {
                        AddToCache(cacheKeyPattern, cacheVisitCount, captchaField.Duration);
                    }
                }
                else
                {
                    AddToCache(cacheKeyPattern, cacheVisitCount, captchaField.Duration);
                }
            }

            return result;
        }

        private async Task<bool> CheckCaptchaTokenAsync(string cacheKeyPattern, int cacheVisitedCount, int duration,
            string token)
        {
            bool result = false;
            using (var client = new HttpClient())
            {
                string url = _captchaOptions.CaptchaControlBaseUrl
                    .Replace(_captchaOptions.CaptchaSecretKeyHolder, _captchaOptions.CaptchaSecretKey)
                    .Replace(_captchaOptions.CaptchaTokenHolder, token);
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var captchaResultJson = await response.Content.ReadAsStringAsync();
                    var captchaResult = JsonConvert.DeserializeObject<CaptchaResponse>(captchaResultJson);

                    if (captchaResult.Success)
                    {
                        _distributedCache.Remove(cacheKeyPattern);
                    }
                    else
                    {
                        AddToCache(cacheKeyPattern, cacheVisitedCount, duration);
                        result = true;
                    }
                }
                else
                {
                    AddToCache(cacheKeyPattern, cacheVisitedCount, duration);
                    result = true;
                }
            }

            return result;
        }


        private void AddToCache(string key, int visitedCount, int duration)
        {
            visitedCount++;
            _distributedCache.Set<int>(key, visitedCount, new DistributedCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromSeconds(duration)
            });
        }
    }
}