using Microsoft.AspNetCore.Http;

namespace Koptcha.Extensions
{
    public static class HttpRequestMessageExtensions
    {
        private const string HttpContext = "MS_HttpContext";
        private const string RemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";

        public static string GetClientIpAddress(this HttpRequest request)
        {
            string ipAdress = string.Empty;

            if (request?.HttpContext.Connection.RemoteIpAddress != null)
            {
                var userIpAddress = request.HttpContext.Connection.RemoteIpAddress.ToString();

                if (userIpAddress == "::1")
                {
                    userIpAddress = "localhost";
                }

                ipAdress = !string.IsNullOrEmpty(ipAdress) ? ipAdress : userIpAddress;
            }

            return ipAdress;
        }
    }
}