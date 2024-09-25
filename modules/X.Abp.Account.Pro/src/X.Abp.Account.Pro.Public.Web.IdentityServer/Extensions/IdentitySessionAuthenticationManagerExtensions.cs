// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using DeviceDetectorNET;

using IdentityServer4.Configuration;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace X.Abp.Account.Web.Extensions;

public static class IdentitySessionAuthenticationManagerExtensions
{
    public const string ExternalAuthenticationMethod = "external";

    public static class EnvironmentKeys
    {
        public const string IdentityServerBasePath = "idsvr:IdentityServerBasePath";
        [Obsolete("The IdentityServerOrigin constant is obsolete.")]
        public const string IdentityServerOrigin = "idsvr:IdentityServerOrigin";
        public const string SignOutCalled = "idsvr:IdentityServerSignOutCalled";
    }

    internal static async Task<string> GetCookieAuthenticationSchemeAsync(this HttpContext context, IAuthenticationSchemeProvider schemes)
    {
        var options = context.RequestServices.GetRequiredService<IdentityServerOptions>();
        if (options.Authentication.CookieAuthenticationScheme != null)
        {
            return options.Authentication.CookieAuthenticationScheme;
        }

        var scheme = await schemes.GetDefaultAuthenticateSchemeAsync();
        if (scheme == null)
        {
            throw new InvalidOperationException("No DefaultAuthenticateScheme found or no CookieAuthenticationScheme configured on IdentityServerOptions.");
        }

        return scheme.Name;
    }

    internal static (string Device, string DeviceInfo, string IpAddress) GetClientInfo(this HttpContext httpContext, ILogger logger)
    {
        string userAgent = httpContext.Request.Headers?["User-agent"];

        // var result = DeviceDetector.GetInfoFromUserAgent(userAgent);
        string device = null;
        string deviceInfo = null;
        string clientIP = null;
        if (!userAgent.IsNullOrWhiteSpace())
        {
            var deviceDetector = new DeviceDetector(userAgent);
            deviceDetector.Parse();
            if (deviceDetector.IsParsed())
            {
                var osInfo = deviceDetector.GetOs();
                if (deviceDetector.IsMobile())
                {
                    device = osInfo.Success ? osInfo.Match.Name : "Mobile";
                }
                else if (deviceDetector.IsBrowser())
                {
                    device = "Web";
                }
                else if (deviceDetector.IsDesktop())
                {
                    device = "Desktop";
                }

                if (osInfo.Success)
                {
                    deviceInfo = osInfo.Match.Name;
                }

                var clientInfo = deviceDetector.GetClient();
                if (clientInfo.Success)
                {
                    deviceInfo = deviceInfo.IsNullOrWhiteSpace() ? clientInfo.Match.Name : deviceInfo + " " + clientInfo.Match.Name;
                }
            }
        }

        try
        {
            clientIP = httpContext.Request.Headers?["X-Real-IP"];
            if (string.IsNullOrWhiteSpace(clientIP))
            {
                clientIP = httpContext.Request.Headers?["X-Forwarded-For"];
                if (string.IsNullOrWhiteSpace(clientIP))
                {
                    clientIP = httpContext.Connection.RemoteIpAddress.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogException(ex, LogLevel.Warning);
            clientIP = null;
        }

        return (device, deviceInfo, clientIP);
    }

    internal static string GetClientIpAddress(this HttpContext httpContext, ILogger logger)
    {
        string clientIP = null;
        try
        {
            clientIP = httpContext.Request.Headers?["X-Real-IP"];
            if (string.IsNullOrWhiteSpace(clientIP))
            {
                clientIP = httpContext.Request.Headers?["X-Forwarded-For"];
                if (string.IsNullOrWhiteSpace(clientIP))
                {
                    clientIP = httpContext.Connection.RemoteIpAddress.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogException(ex, LogLevel.Warning);
            clientIP = null;
        }

        return clientIP;
    }

    internal static void SetSignOutCalled(this HttpContext context)
    {
        if (context == null)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
        }

        context.Items[EnvironmentKeys.SignOutCalled] = "true";
    }
}
