﻿// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using OpenIddict.Abstractions;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;

using Volo.Abp.DependencyInjection;
using Volo.Abp.OpenIddict.Localization;

using X.Abp.Identity;

using static OpenIddict.Abstractions.OpenIddictConstants;
using static OpenIddict.Server.AspNetCore.OpenIddictServerAspNetCoreConstants;
using static OpenIddict.Server.OpenIddictServerEvents;

using Properties = OpenIddict.Server.AspNetCore.OpenIddictServerAspNetCoreConstants.Properties;

namespace AbpVnext.Pro.Customs;

/// <summary>
/// Provides the logic necessary to extract, validate and handle OpenID Connect requests.
/// </summary>
//[Dependency(ReplaceServices = true)]
//[ExposeServices(typeof(IIdentityUserAppService), typeof(OpenIddictServerAspNetCoreHandler), typeof(CustomOpenIddictServerAspNetCoreHandler))]
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class CustomOpenIddictServerAspNetCoreHandler : AuthenticationHandler<OpenIddictServerAspNetCoreOptions>,
    IAuthenticationRequestHandler,
    IAuthenticationSignInHandler,
    IAuthenticationSignOutHandler
{
    private readonly IStringLocalizerFactory _stringLocalizerFactory;
    private readonly IOpenIddictServerDispatcher _dispatcher;
    private readonly IOpenIddictServerFactory _factory;

    /// <summary>
    /// Creates a new instance of the <see cref="CustomOpenIddictServerAspNetCoreHandler"/> class.
    /// </summary>
#if SUPPORTS_TIME_PROVIDER
    public CustomOpenIddictServerAspNetCoreHandler(
        IStringLocalizerFactory stringLocalizerFactory,
        IOpenIddictServerDispatcher dispatcher,
        IOpenIddictServerFactory factory,
        IOptionsMonitor<OpenIddictServerAspNetCoreOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
        _stringLocalizerFactory = stringLocalizerFactory;
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }
#else
    public CustomOpenIddictServerAspNetCoreHandler(
        IStringLocalizerFactory stringLocalizerFactory,
        IOpenIddictServerDispatcher dispatcher,
        IOpenIddictServerFactory factory,
        IOptionsMonitor<OpenIddictServerAspNetCoreOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
        _stringLocalizerFactory = stringLocalizerFactory;
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }
#endif

    /// <inheritdoc/>
    public async Task<bool> HandleRequestAsync()
    {
        // Note: the transaction may be already attached when replaying an ASP.NET Core request
        // (e.g when using the built-in status code pages middleware with the re-execute mode).
        var transaction = Context.Features.Get<OpenIddictServerAspNetCoreFeature>()?.Transaction;
        if (transaction is null)
        {
            // Create a new transaction and attach the HTTP request to make it available to the ASP.NET Core handlers.
            transaction = await _factory.CreateTransactionAsync();
            transaction.Properties[typeof(HttpRequest).FullName!] = new WeakReference<HttpRequest>(Request);

            // Attach the OpenIddict server transaction to the ASP.NET Core features
            // so that it can retrieved while performing sign-in/sign-out operations.
            Context.Features.Set(new OpenIddictServerAspNetCoreFeature { Transaction = transaction });
        }

        var context = new ProcessRequestContext(transaction)
        {
            CancellationToken = Context.RequestAborted
        };

        await _dispatcher.DispatchAsync(context);

        if (context.IsRequestHandled)
        {
            return true;
        }
        else if (context.IsRequestSkipped)
        {
            return false;
        }
        else if (context.IsRejected)
        {
            var notification = new ProcessErrorContext(transaction)
            {
                CancellationToken = Context.RequestAborted,
                Error = context.Error ?? Errors.InvalidRequest,
                ErrorDescription = context.ErrorDescription,
                ErrorUri = context.ErrorUri,
                Response = new OpenIddictResponse()
            };

            await _dispatcher.DispatchAsync(notification);

            if (notification.IsRequestHandled)
            {
                return true;
            }
            else if (notification.IsRequestSkipped)
            {
                return false;
            }

            throw new InvalidOperationException(_stringLocalizerFactory.Create(typeof(AbpOpenIddictResource))["ID0111"]);
        }

        return false;
    }

    /// <inheritdoc/>
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var transaction = Context.Features.Get<OpenIddictServerAspNetCoreFeature>()?.Transaction ??
            throw new InvalidOperationException(_stringLocalizerFactory.Create(typeof(AbpOpenIddictResource))["ID0112"]);

        // Note: in many cases, the authentication token was already validated by the time this action is called
        // (generally later in the pipeline, when using the pass-through mode). To avoid having to re-validate it,
        // the authentication context is resolved from the transaction. If it's not available, a new one is created.
        var context = transaction.GetProperty<ProcessAuthenticationContext>(typeof(ProcessAuthenticationContext).FullName!);
        if (context is null)
        {
            await _dispatcher.DispatchAsync(context = new ProcessAuthenticationContext(transaction)
            {
                CancellationToken = Context.RequestAborted
            });

            // Store the context object in the transaction so it can be later retrieved by handlers
            // that want to access the authentication result without triggering a new authentication flow.
            transaction.SetProperty(typeof(ProcessAuthenticationContext).FullName!, context);
        }

        if (context.IsRequestHandled || context.IsRequestSkipped)
        {
            return AuthenticateResult.NoResult();
        }
        else if (context.IsRejected)
        {
            // Note: the missing_token error is special-cased to indicate to ASP.NET Core
            // that no authentication result could be produced due to the lack of token.
            // This also helps reducing the logging noise when no token is specified.
            if (string.Equals(context.Error, Errors.MissingToken, StringComparison.Ordinal))
            {
                return AuthenticateResult.NoResult();
            }

            var properties = new AuthenticationProperties(new Dictionary<string, string>
            {
                [Properties.Error] = context.Error,
                [Properties.ErrorDescription] = context.ErrorDescription,
                [Properties.ErrorUri] = context.ErrorUri
            });

            return AuthenticateResult.Fail(_stringLocalizerFactory.Create(typeof(AbpOpenIddictResource))["ID0113"], properties);
        }
        else
        {
            // A single main claims-based principal instance can be attached to an authentication ticket.
            // To return the most appropriate one, the principal is selected based on the endpoint type.
            // Independently of the selected main principal, all principals resolved from validated tokens
            // are attached to the authentication properties bag so they can be accessed from user code.
            var principal = context.EndpointType switch
            {
                OpenIddictServerEndpointType.Authorization or OpenIddictServerEndpointType.Logout
                    => context.IdentityTokenPrincipal,

                OpenIddictServerEndpointType.Introspection or OpenIddictServerEndpointType.Revocation
                    => context.AccessTokenPrincipal ??
                       context.RefreshTokenPrincipal ??
                       context.IdentityTokenPrincipal ??
                       context.AuthorizationCodePrincipal ??
                       context.DeviceCodePrincipal ??
                       context.UserCodePrincipal,

                OpenIddictServerEndpointType.Token when context.Request.IsAuthorizationCodeGrantType()
                    => context.AuthorizationCodePrincipal,
                OpenIddictServerEndpointType.Token when context.Request.IsDeviceCodeGrantType()
                    => context.DeviceCodePrincipal,
                OpenIddictServerEndpointType.Token when context.Request.IsRefreshTokenGrantType()
                    => context.RefreshTokenPrincipal,

                OpenIddictServerEndpointType.Userinfo => context.AccessTokenPrincipal,

                OpenIddictServerEndpointType.Verification => context.UserCodePrincipal,

                _ => null
            };

            var properties = new AuthenticationProperties
            {
                ExpiresUtc = principal?.GetExpirationDate(),
                IssuedUtc = principal?.GetCreationDate()
            };

            foreach (var property in context.Properties)
            {
                properties.Items[property.Key] = property.Value;
            }

            List<AuthenticationToken> tokens = null;

            // Attach the tokens to allow any ASP.NET Core component (e.g a controller)
            // to retrieve them (e.g to make an API request to another application).
            if (!string.IsNullOrEmpty(context.AccessToken))
            {
                tokens = new(capacity: 1);
                tokens.Add(new AuthenticationToken
                {
                    Name = Tokens.AccessToken,
                    Value = context.AccessToken
                });
            }

            if (!string.IsNullOrEmpty(context.AuthorizationCode))
            {
                tokens = new(capacity: 1);
                tokens.Add(new AuthenticationToken
                {
                    Name = Tokens.AuthorizationCode,
                    Value = context.AuthorizationCode
                });
            }

            if (!string.IsNullOrEmpty(context.ClientAssertion))
            {
                tokens = new(capacity: 1);
                tokens.Add(new AuthenticationToken
                {
                    Name = Tokens.ClientAssertion,
                    Value = context.ClientAssertion
                });
            }

            if (!string.IsNullOrEmpty(context.DeviceCode))
            {
                tokens = new(capacity: 1);
                tokens.Add(new AuthenticationToken
                {
                    Name = Tokens.DeviceCode,
                    Value = context.DeviceCode
                });
            }

            if (!string.IsNullOrEmpty(context.IdentityToken))
            {
                tokens = new(capacity: 1);
                tokens.Add(new AuthenticationToken
                {
                    Name = Tokens.IdentityToken,
                    Value = context.IdentityToken
                });
            }

            if (!string.IsNullOrEmpty(context.RefreshToken))
            {
                tokens = new(capacity: 1);
                tokens.Add(new AuthenticationToken
                {
                    Name = Tokens.RefreshToken,
                    Value = context.RefreshToken
                });
            }

            if (!string.IsNullOrEmpty(context.UserCode))
            {
                tokens = new(capacity: 1);
                tokens.Add(new AuthenticationToken
                {
                    Name = Tokens.UserCode,
                    Value = context.UserCode
                });
            }

            if (context.AccessTokenPrincipal is not null)
            {
                properties.SetParameter(Properties.AccessTokenPrincipal, context.AccessTokenPrincipal);
            }

            if (context.AuthorizationCodePrincipal is not null)
            {
                properties.SetParameter(Properties.AuthorizationCodePrincipal, context.AuthorizationCodePrincipal);
            }

            if (context.ClientAssertionPrincipal is not null)
            {
                properties.SetParameter(Properties.ClientAssertionPrincipal, context.ClientAssertionPrincipal);
            }

            if (context.DeviceCodePrincipal is not null)
            {
                properties.SetParameter(Properties.DeviceCodePrincipal, context.DeviceCodePrincipal);
            }

            if (context.IdentityTokenPrincipal is not null)
            {
                properties.SetParameter(Properties.IdentityTokenPrincipal, context.IdentityTokenPrincipal);
            }

            if (context.RefreshTokenPrincipal is not null)
            {
                properties.SetParameter(Properties.RefreshTokenPrincipal, context.RefreshTokenPrincipal);
            }

            if (context.UserCodePrincipal is not null)
            {
                properties.SetParameter(Properties.UserCodePrincipal, context.UserCodePrincipal);
            }

            if (tokens is { Count: > 0 })
            {
                properties.StoreTokens(tokens);
            }

            return AuthenticateResult.Success(new AuthenticationTicket(
                principal ?? new ClaimsPrincipal(new ClaimsIdentity()),
                properties,
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme));
        }
    }

    /// <inheritdoc/>
    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var transaction = Context.Features.Get<OpenIddictServerAspNetCoreFeature>()?.Transaction ??
            throw new InvalidOperationException(_stringLocalizerFactory.Create(typeof(AbpOpenIddictResource))["ID0112"]);

        transaction.Properties[typeof(AuthenticationProperties).FullName!] = properties ?? new AuthenticationProperties();

        var context = new ProcessChallengeContext(transaction)
        {
            CancellationToken = Context.RequestAborted,
            Response = new OpenIddictResponse()
        };

        await _dispatcher.DispatchAsync(context);

        if (context.IsRequestHandled || context.IsRequestSkipped)
        {
            return;
        }
        else if (context.IsRejected)
        {
            var notification = new ProcessErrorContext(transaction)
            {
                CancellationToken = Context.RequestAborted,
                Error = context.Error ?? Errors.InvalidRequest,
                ErrorDescription = context.ErrorDescription,
                ErrorUri = context.ErrorUri,
                Response = new OpenIddictResponse()
            };

            await _dispatcher.DispatchAsync(notification);

            if (notification.IsRequestHandled || context.IsRequestSkipped)
            {
                return;
            }

            throw new InvalidOperationException(_stringLocalizerFactory.Create(typeof(AbpOpenIddictResource))["ID0111"]);
        }
    }

    /// <inheritdoc/>
    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        => HandleChallengeAsync(properties);

    /// <inheritdoc/>
    public async Task SignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
    {
        ArgumentNullException.ThrowIfNull(user);

        var transaction = Context.Features.Get<OpenIddictServerAspNetCoreFeature>()?.Transaction ??
            throw new InvalidOperationException(_stringLocalizerFactory.Create(typeof(AbpOpenIddictResource))["ID0112"]);

        transaction.Properties[typeof(AuthenticationProperties).FullName!] = properties ?? new AuthenticationProperties();

        var context = new ProcessSignInContext(transaction)
        {
            CancellationToken = Context.RequestAborted,
            Principal = user,
            Response = new OpenIddictResponse()
        };

        await _dispatcher.DispatchAsync(context);

        if (context.IsRequestHandled || context.IsRequestSkipped)
        {
            return;
        }
        else if (context.IsRejected)
        {
            var notification = new ProcessErrorContext(transaction)
            {
                Error = context.Error ?? Errors.InvalidRequest,
                ErrorDescription = context.ErrorDescription,
                ErrorUri = context.ErrorUri,
                Response = new OpenIddictResponse()
            };

            await _dispatcher.DispatchAsync(notification);

            if (notification.IsRequestHandled || context.IsRequestSkipped)
            {
                return;
            }

            throw new InvalidOperationException(_stringLocalizerFactory.Create(typeof(AbpOpenIddictResource))["ID0111"]);
        }
    }

    /// <inheritdoc/>
    public async Task SignOutAsync(AuthenticationProperties properties)
    {
        var transaction = Context.Features.Get<OpenIddictServerAspNetCoreFeature>()?.Transaction ??
            throw new InvalidOperationException(_stringLocalizerFactory.Create(typeof(AbpOpenIddictResource))["ID0112"]);

        var context = new ProcessSignOutContext(transaction)
        {
            CancellationToken = Context.RequestAborted,
            Response = new OpenIddictResponse()
        };

        transaction.Properties[typeof(AuthenticationProperties).FullName!] = properties ?? new AuthenticationProperties();

        await _dispatcher.DispatchAsync(context);

        if (context.IsRequestHandled || context.IsRequestSkipped)
        {
            return;
        }
        else if (context.IsRejected)
        {
            var notification = new ProcessErrorContext(transaction)
            {
                CancellationToken = Context.RequestAborted,
                Error = context.Error ?? Errors.InvalidRequest,
                ErrorDescription = context.ErrorDescription,
                ErrorUri = context.ErrorUri,
                Response = new OpenIddictResponse()
            };

            await _dispatcher.DispatchAsync(notification);

            if (notification.IsRequestHandled || context.IsRequestSkipped)
            {
                return;
            }

            throw new InvalidOperationException(_stringLocalizerFactory.Create(typeof(AbpOpenIddictResource))["ID0111"]);
        }
    }
}
