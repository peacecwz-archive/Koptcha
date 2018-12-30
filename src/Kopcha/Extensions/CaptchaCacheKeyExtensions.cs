using Kopcha.Common;
using Kopcha.Models;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace Kopcha.Extensions
{
    public static class CaptchaCacheKeyExtensions
    {
        public static string GetCacheKey(this ActionExecutingContext actionContext, string domainName, CaptchaType captchaType, string fieldName, string globalCacheName)
        {
            string ip = actionContext.HttpContext.Request.GetClientIpAddress();

            string signature = ip;

            var descriptor = actionContext?.ActionDescriptor as ControllerActionDescriptor;

            string actionName = descriptor.ActionName;

            string controllerName = descriptor.ControllerName;

            string baseCacheKey = $"{domainName}:{controllerName}_{actionName}:{captchaType.ToString()}_";

            switch (captchaType)
            {
                case CaptchaType.Ip:
                    signature = baseCacheKey + ip;
                    break;
                case CaptchaType.Email:
                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        var fieldValue = GetPropValue(actionContext.ActionArguments.FirstOrDefault().Value, fieldName);

                        if (!string.IsNullOrEmpty(fieldValue?.ToString()))
                        {
                            signature = baseCacheKey + fieldValue;
                        }
                        else
                        {
                            signature = baseCacheKey + ip;
                        }
                    }
                    else
                    {
                        signature = baseCacheKey + ip;
                    }
                    break;
                case CaptchaType.IpAndEmail:
                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        var fieldValue = GetPropValue(actionContext.ActionArguments.FirstOrDefault().Value, fieldName);

                        if (!string.IsNullOrEmpty(fieldValue?.ToString()))
                        {
                            signature = baseCacheKey + ip + "_" + fieldValue;
                        }
                        else
                        {
                            signature = baseCacheKey + ip;
                        }
                    }
                    else
                    {
                        signature = baseCacheKey + ip;
                    }
                    break;
                case CaptchaType.Global:
                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        var fieldValue = GetPropValue(actionContext.ActionArguments.FirstOrDefault().Value, fieldName);

                        if (!string.IsNullOrEmpty(fieldValue?.ToString()))
                        {
                            signature = (!string.IsNullOrEmpty(globalCacheName)
                                            ? globalCacheName
                                            : CaptchaConstants.GlobalCacheName) + "_" + fieldValue;
                        }
                        else
                        {
                            signature = (!string.IsNullOrEmpty(globalCacheName)
                                            ? globalCacheName
                                            : CaptchaConstants.GlobalCacheName) + "_" + ip;
                        }
                    }
                    else
                    {
                        signature = (!string.IsNullOrEmpty(globalCacheName)
                                        ? globalCacheName
                                        : CaptchaConstants.GlobalCacheName) + "_" + ip;
                    }
                    break;
            }

            return signature;
        }

        private static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName)?.GetValue(src, null);
        }

        public static string GetCacheKey(this CaptchaField parameters, string domainName, string controllerName, string actionName, string globalCacheName, string ipAddress)
        {
            string signature = ipAddress;

            string baseCacheKey = $"{domainName}:{controllerName}_{actionName}:{parameters.FieldType.ToString()}_";

            switch (parameters.FieldType)
            {
                case CaptchaType.Ip:
                    signature = baseCacheKey + ipAddress;
                    break;
                case CaptchaType.Email:
                    signature = baseCacheKey + parameters.FieldValue;
                    break;
                case CaptchaType.IpAndEmail:
                    signature = baseCacheKey + ipAddress + "_" + parameters.FieldValue;
                    break;
                case CaptchaType.Global:
                    if (!string.IsNullOrEmpty(parameters.FieldValue))
                    {
                        if (!string.IsNullOrEmpty(parameters.FieldValue))
                        {
                            signature = (!string.IsNullOrEmpty(globalCacheName)
                                            ? globalCacheName
                                            : CaptchaConstants.GlobalCacheName) + "_" + parameters.FieldValue;
                        }
                        else
                        {
                            signature = (!string.IsNullOrEmpty(globalCacheName)
                                            ? globalCacheName
                                            : CaptchaConstants.GlobalCacheName) + "_" + ipAddress;
                        }
                    }
                    else
                    {
                        signature = (!string.IsNullOrEmpty(globalCacheName)
                                        ? globalCacheName
                                        : CaptchaConstants.GlobalCacheName) + "_" + ipAddress;
                    }
                    break;
            }

            return signature;
        }
    }
}