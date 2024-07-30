// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using IdentityModel;

using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using IdentityServer4.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AbpVnext.Pro.Customs;

/// <summary>
/// Cookie-based session implementation
/// </summary>
/// <seealso cref="IUserSession" />
public class CustomDefaultUserSession : IUserSession
{
    /// <summary>
    /// The HTTP context accessor
    /// </summary>
    protected readonly IHttpContextAccessor HttpContextAccessor;

    /// <summary>
    /// The handlers
    /// </summary>
    protected readonly IAuthenticationHandlerProvider Handlers;

    /// <summary>
    /// The options
    /// </summary>
    protected readonly IdentityServerOptions Options;

    /// <summary>
    /// The clock
    /// </summary>
    protected readonly ISystemClock Clock;

    /// <summary>
    /// The logger
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// Gets the HTTP context.
    /// </summary>
    /// <value>
    /// The HTTP context.
    /// </value>
    protected HttpContext HttpContext => HttpContextAccessor.HttpContext;

    /// <summary>
    /// Gets the name of the check session cookie.
    /// </summary>
    /// <value>
    /// The name of the check session cookie.
    /// </value>
    protected string CheckSessionCookieName => Options.Authentication.CheckSessionCookieName;

    /// <summary>
    /// Gets the domain of the check session cookie.
    /// </summary>
    /// <value>
    /// The domain of the check session cookie.
    /// </value>
    protected string CheckSessionCookieDomain => Options.Authentication.CheckSessionCookieDomain;

    /// <summary>
    /// Gets the SameSite mode of the check session cookie.
    /// </summary>
    /// <value>
    /// The SameSite mode of the check session cookie.
    /// </value>
    protected SameSiteMode CheckSessionCookieSameSiteMode => Options.Authentication.CheckSessionCookieSameSiteMode;

    /// <summary>
    /// The principal
    /// </summary>
    protected ClaimsPrincipal principal;

    /// <summary>
    /// The properties
    /// </summary>
    protected AuthenticationProperties properties;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultUserSession"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    /// <param name="handlers">The handlers.</param>
    /// <param name="options">The options.</param>
    /// <param name="clock">The clock.</param>
    /// <param name="logger">The logger.</param>
    public CustomDefaultUserSession(
        IHttpContextAccessor httpContextAccessor,
        IAuthenticationHandlerProvider handlers,
        IdentityServerOptions options,
        ISystemClock clock,
        ILogger<IUserSession> logger)
    {
        HttpContextAccessor = httpContextAccessor;
        Handlers = handlers;
        Options = options;
        Clock = clock;
        Logger = logger;
    }

    // we need this helper (and can't call HttpContext.AuthenticateAsync) so we don't run
    // claims transformation when we get the principal. this also ensures that we don't
    // re-issue a cookie that includes the claims from claims transformation.
    //
    // also, by caching the _principal/_properties it allows someone to issue a new
    // cookie (via HttpContext.SignInAsync) and we'll use those new values, rather than
    // just reading the incoming cookie
    //
    // this design requires this to be in DI as scoped

    /// <summary>
    /// Authenticates the authentication cookie for the current HTTP request and caches the user and properties results.
    /// </summary>
    protected virtual async Task AuthenticateAsync()
    {
        if (principal == null || properties == null)
        {
            var scheme = await HttpContext.GetCookieAuthenticationSchemeAsync();

            var handler = await Handlers.GetHandlerAsync(HttpContext, scheme);
            if (handler == null)
            {
                throw new InvalidOperationException($"No authentication handler is configured to authenticate for the scheme: {scheme}");
            }

            var result = await handler.AuthenticateAsync();
            if (result != null && result.Succeeded)
            {
                principal = result.Principal;
                properties = result.Properties;
            }
        }
    }

    /// <summary>
    /// Creates a session identifier for the signin context and issues the session id cookie.
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="properties"></param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// principal
    /// or
    /// properties
    /// </exception>
    public virtual async Task<string> CreateSessionIdAsync(ClaimsPrincipal principal, AuthenticationProperties properties)
    {
        if (principal == null)
        {
            throw new ArgumentNullException(nameof(principal));
        }

        if (properties == null)
        {
            throw new ArgumentNullException(nameof(properties));
        }

        var currentSubjectId = (await GetUserAsync())?.GetSubjectId();
        var newSubjectId = principal.GetSubjectId();

        if (properties.GetSessionId() == null || currentSubjectId != newSubjectId)
        {
            properties.SetSessionId(CryptoRandom.CreateUniqueId(16, CryptoRandom.OutputFormat.Hex));
        }

        var sid = properties.GetSessionId();
        IssueSessionIdCookie(sid);

        this.principal = principal;
        this.properties = properties;

        return sid;
    }

    /// <summary>
    /// Gets the current authenticated user.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public virtual async Task<ClaimsPrincipal> GetUserAsync()
    {
        await AuthenticateAsync();

        return principal;
    }

    /// <summary>
    /// Gets the current session identifier.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public virtual async Task<string> GetSessionIdAsync()
    {
        await AuthenticateAsync();

        return properties?.GetSessionId();
    }

    /// <summary>
    /// Ensures the session identifier cookie asynchronous.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public virtual async Task EnsureSessionIdCookieAsync()
    {
        var sid = await GetSessionIdAsync();
        if (sid != null)
        {
            IssueSessionIdCookie(sid);
        }
        else
        {
            await RemoveSessionIdCookieAsync();
        }
    }

    /// <summary>
    /// Removes the session identifier cookie.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public virtual Task RemoveSessionIdCookieAsync()
    {
        if (HttpContext.Request.Cookies.ContainsKey(CheckSessionCookieName))
        {
            // only remove it if we have it in the request
            var options = CreateSessionIdCookieOptions();
            options.Expires = Clock.UtcNow.UtcDateTime.AddYears(-1);

            HttpContext.Response.Cookies.Append(CheckSessionCookieName, ".", options);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Creates the options for the session cookie.
    /// </summary>
    public virtual CookieOptions CreateSessionIdCookieOptions()
    {
        var secure = HttpContext.Request.IsHttps;
        var path = HttpContext.GetIdentityServerBasePath().CleanUrlPath();

        var options = new CookieOptions
        {
            HttpOnly = false,
            Secure = secure,
            Path = path,
            IsEssential = true,
            Domain = CheckSessionCookieDomain,
            SameSite = CheckSessionCookieSameSiteMode
        };

        return options;
    }

    /// <summary>
    /// Issues the cookie that contains the session id.
    /// </summary>
    /// <param name="sid"></param>
    public virtual void IssueSessionIdCookie(string sid)
    {
        if (Options.Endpoints.EnableCheckSessionEndpoint)
        {
            if (HttpContext.Request.Cookies[CheckSessionCookieName] != sid)
            {
                HttpContext.Response.Cookies.Append(
                    Options.Authentication.CheckSessionCookieName,
                    sid,
                    CreateSessionIdCookieOptions());
            }
        }
    }

    /// <summary>
    /// Adds a client to the list of clients the user has signed into during their session.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">clientId</exception>
    public virtual async Task AddClientIdAsync(string clientId)
    {
        if (clientId == null)
        {
            throw new ArgumentNullException(nameof(clientId));
        }

        await AuthenticateAsync();
        if (properties != null)
        {
            var clientIds = properties.GetClientList();
            if (!clientIds.Contains(clientId))
            {
                properties.AddClientId(clientId);
                await UpdateSessionCookie();
            }
        }
    }

    /// <summary>
    /// Gets the list of clients the user has signed into during their session.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public virtual async Task<IEnumerable<string>> GetClientListAsync()
    {
        await AuthenticateAsync();

        if (properties != null)
        {
            try
            {
                return properties.GetClientList();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error decoding client list");

                // clear so we don't keep failing
                properties.RemoveClientList();
                await UpdateSessionCookie();
            }
        }

        return Enumerable.Empty<string>();
    }

    // client list helpers
    private async Task UpdateSessionCookie()
    {
        await AuthenticateAsync();

        if (principal == null || properties == null)
        {
            throw new InvalidOperationException("User is not currently authenticated");
        }

        var scheme = await HttpContext.GetCookieAuthenticationSchemeAsync();
        await HttpContext.SignInAsync(scheme, principal, properties);
    }
}
