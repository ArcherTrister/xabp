// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

using AbpVnext.Pro.EntityFrameworkCore;
using AbpVnext.Pro.MultiTenancy;

using AspNet.Security.OAuth.GitHub;

using IdentityServer4.Configuration;

using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;

using Volo.Abp;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.BlobStoring;
using Volo.Abp.IdentityServer;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Swashbuckle;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Account;
using X.Abp.Account.Public.Web;
using X.Abp.Account.Public.Web.Impersonation;
using X.Abp.Account.Web;
using X.Abp.Identity.Permissions;
using X.Abp.Saas.Permissions;

namespace AbpVnext.Pro;

[DependsOn(
    typeof(ProHttpApiModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(ProApplicationModule),
    typeof(ProEntityFrameworkCoreModule),
    typeof(AbpAspNetCoreMvcUiBasicThemeModule),
    typeof(AbpAccountPublicWebImpersonationModule),
    typeof(AbpAccountPublicWebIdentityServerModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpAspNetCoreSerilogModule))]

// [DependsOn(typeof(IotEmqxGrpcServiceModule))]
// [DependsOn(typeof(QuartzInstallScriptSqlServerModule))]
// [DependsOn(typeof(QuartzInstallScriptMySqlModule))]
// [DependsOn(typeof(QuartzWebModule))]
// [DependsOn(typeof(AbpBackgroundJobsQuartzModule))]
public class IdentityServerProHttpApiHostModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        //PreConfigure<IdentityBuilder>(identityBuilder =>
        //{
        //    // identityBuilder.AddSignInManager<CustomSignInManager>();
        //    identityBuilder.AddClaimsPrincipalFactory<CustomUserClaimsPrincipalFactory>();
        //});

        PreConfigure<AuthorizationOptions>(options =>
        {
            options.AddPolicy("TwoFactorEnabled",
                x => x.RequireClaim("amr", "mfa"));

            // options.AddPolicy("RequireMfa", policyIsAdminRequirement =>
            // {
            //    policyIsAdminRequirement.Requirements.Add(new RequireMfa());
            // });
        });

        if (hostingEnvironment.IsDevelopment())
        {
            PreConfigure<AbpIdentityServerBuilderOptions>(options =>
            {
                options.AddDeveloperSigningCredential = true;
                //options.UpdateAbpClaimTypes = false;
            });

            // PreConfigure<IdentityServerOptions>(options =>
            // {
            //    // EnableJwtRequestUri = false
            //    // options.Endpoints = new EndpointsOptions { EnableTokenEndpoint = false };
            // });

            //PreConfigure<IIdentityServerBuilder>(identityServerBuilder =>
            //{
            //    //identityServerBuilder.AddProfileService<CustomProfileService>();
            //    // builder.AddExtensionGrantValidator<WeChatMiniProgramGrantValidator>();
            //    // builder.AddExtensionGrantValidator<PhoneVerifyCodeGrantValidator>();
            //    // builder.AddExtensionGrantValidator<ThirdPartyProviderGrantValidator>();
            //    // builder.AddExtensionGrantValidator<BindThirdPartyProviderGrantValidator>();
            //    // //builder.AddPersistedGrantStore<CustomPersistedGrantStore>();
            //    // //builder.AddResponseGenerators
            //    // //CustomTokenValidator
            //    // identityServerBuilder.AddEndpoint<CustomTokenEndpoint>(EndpointNames.Token, ProtocolRoutePaths.Token.EnsureLeadingSlash());
            //});
        }
        else
        {
            PreConfigure<AbpIdentityServerBuilderOptions>(options =>
            {
                options.AddDeveloperSigningCredential = false;
                //options.UpdateAbpClaimTypes = false;
            });

            // IIS 应用程序池--->高级设置--->加载用户配置文件设置为True
            var password = configuration["Certificates:Password"];

            // Debug.Assert(!string.IsNullOrEmpty(password), "Certificates:Password is missing from appsettings");
            var certificate = Path.Combine(hostingEnvironment.ContentRootPath, configuration["Certificates:CerPath"]);

            PreConfigure<IIdentityServerBuilder>(identityServerBuilder =>
            {
                // builder.AddExtensionGrantValidator<WeChatMiniProgramGrantValidator>();
                // builder.AddExtensionGrantValidator<PhoneVerifyCodeGrantValidator>();
                // builder.AddExtensionGrantValidator<ThirdPartyProviderGrantValidator>();
                // builder.AddExtensionGrantValidator<BindThirdPartyProviderGrantValidator>();
                // //builder.AddPersistedGrantStore<CustomPersistedGrantStore>();
                // //builder.AddResponseGenerators
                // //CustomTokenValidator
                identityServerBuilder.AddSigningCredential(new X509Certificate2(certificate, password));
            });
        }

        // PreConfigure<ISignalRServerBuilder>(builder =>
        //        {
        //            builder.AddHubOptions<ChatHub>(options =>
        //            {
        // #if DEBUG
        //                options.EnableDetailedErrors = true;
        // #endif
        //                options.KeepAliveInterval = TimeSpan.FromSeconds(1.5);
        //                options.ClientTimeoutInterval = TimeSpan.FromSeconds(3);
        //            });
        //            //.AddJsonProtocol(options =>
        //            // {
        //            //     //options.PayloadSerializerOptions.IgnoreNullValues = true;
        //            //     options.PayloadSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        //            // });
        //        });

        // PreConfigure<AbpQuartzOptions>(options =>
        // {
        //    //options.StartSchedulerFactory = fac =>
        //    //{
        //    //    LiveLogPlugin liveLogPlugin = new LiveLogPlugin();
        //    //    fac.ListenerManager.AddSchedulerListener(liveLogPlugin);
        //    //    return Task.FromResult(fac);
        //    //};

        // options.Configurator = configure =>
        //    {
        //        configure.UsePersistentStore(storeOptions =>
        //        {
        //            storeOptions.UseProperties = true;
        //            //storeOptions
        //            storeOptions.UseJsonSerializer();
        //            //storeOptions.UseMySql(configuration.GetConnectionString("Quartz"));
        //            //storeOptions.UseMySql(mysql =>
        //            //{
        //            //    //mysql.TablePrefix = "qqqq";
        //            //    mysql.ConnectionString = configuration.GetConnectionString("Quartz");
        //            //});
        //            storeOptions.UseSqlServer(configuration.GetConnectionString("Quartz"));
        //            storeOptions.UseClustering(c =>
        //            {
        //                c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
        //                c.CheckinInterval = TimeSpan.FromSeconds(10);
        //            });
        //            //storeOptions.UseInMemoryExecutionHistoryPlugin();
        //            //storeOptions.UseMySqlExecutionHistoryPlugin();
        //            storeOptions.UseSqlServerExecutionHistoryPlugin();
        //        });
        //        //configure.AddSchedulerListener();
        //    };
        // });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        if (hostingEnvironment.IsDevelopment())
        {
            // You can disable this setting in production to avoid any potential security risks.
            IdentityModelEventSource.ShowPII = true;
        }

        context.Services.AddSameSiteCookiePolicy();

        /*
        Configure<AuthenticationSchemeOptions>(options =>
        {
            Console.WriteLine("1111111111111111111111111111111111111111111111111111");

            options.ForwardDefaultSelector = ctx =>
            {
                string authorization = ctx.Request.Headers.Authorization;
                if (!authorization.IsNullOrWhiteSpace() && authorization.StartsWith("Bearer ", StringComparison.InvariantCultureIgnoreCase))
                {
                    return JwtBearerDefaults.AuthenticationScheme;
                }

                // string xRequestedWith = ctx.Request.Headers.XRequestedWith;
                // if (!xRequestedWith.IsNullOrWhiteSpace() && xRequestedWith.Equals("XMLHttpRequest", StringComparison.OrdinalIgnoreCase))
                // {
                //    return JwtBearerDefaults.AuthenticationScheme;
                // }
                var accessToken = ctx.Request.Query["access_token"];

                // If the request is for our hub...
                if (!string.IsNullOrWhiteSpace(accessToken) &&
                    ctx.Request.Path.StartsWithSegments("/signalr-hubs"))
                {
                    return JwtBearerDefaults.AuthenticationScheme;
                }

                // return (ctx.Request.Path.StartsWithSegments("/api")) ? JwtBearerDefaults.AuthenticationScheme : null;
                // return null;
                return CookieAuthenticationDefaults.AuthenticationScheme;
            };
        });
        */

        context.Services.Configure<IISServerOptions>(options =>
        {
            options.MaxRequestBodySize = int.MaxValue;
        });
        context.Services.Configure<FormOptions>(options =>
        {
            options.ValueLengthLimit = int.MaxValue;
            options.MultipartBodyLengthLimit = int.MaxValue;
            options.MultipartHeadersLengthLimit = int.MaxValue;
        });
        context.Services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = int.MaxValue;
        });

        // Configure<CookieAuthenticationOptions>
        //    (IdentityConstants.TwoFactorRememberMeScheme, options =>
        // {
        //    options.ExpireTimeSpan = TimeSpan.FromDays(30); //override the timeout
        //    //options.Cookie.Name = "MyRememberMeCookieName"; //override the cookie name
        // });

        // TODO: AbpClaimsPrincipalFactory IUserClaimsPrincipalFactory
        // context.Services.AddScoped<IUserClaimsPrincipalFactory<IdentityUser>, AdditionalUserClaimsPrincipalFactory>();
        ConfigureAuthentication(context, configuration);
        ConfigureUrls(configuration);
        ConfigureBundles();
        ConfigureConventionalControllers();
        ConfigureImpersonation(context);
        ConfigureSwagger(context, configuration);

        // ConfigureLocalization();
        ConfigureVirtualFileSystem(context);
        ConfigureCors(context, configuration);
        ConfigureExternalProviders(context);
        // ConfigureHealthChecks(context, configuration);

        Configure<AbpAntiForgeryOptions>(options =>
        {
            // options.TokenCookie.Expiration = TimeSpan.FromDays(365);
            // options.AutoValidateIgnoredHttpMethods.Remove("GET");
            // options.AutoValidateFilter =
            //    type => !type.Namespace.StartsWith("MyProject.MyIgnoredNamespace");
            options.AutoValidate = false;
        });

        Configure<AbpBlobStoringOptions>(options =>
        {
            // options.Containers.ConfigureDefault(container =>
            // {
            //    container.UseFileSystem(fileSystem =>
            //    {
            //        fileSystem.BasePath = "/files";
            //    });
            // });

            // VersionManagement
            // options.Containers.Configure<ApplicationFileContainer>(container =>
            // {
            //    container.UseFileSystem(fileSystem =>
            //    {
            //        fileSystem.BasePath = hostingEnvironment.ContentRootPath + "/files";
            //    });
            // });
        });

        // Configure<AbpDistributedCacheOptions>(options =>
        // {
        //    options.KeyPrefix = "KeyPrefix";
        // });

        //context.Services.AddSingleton<IDistributedLockProvider>(sp =>
        //{
        //    var connection = ConnectionMultiplexer
        //        .Connect(configuration["Redis:Configuration"]);
        //    return new
        //        RedisDistributedSynchronizationProvider(connection.GetDatabase());
        //});
        //LocalAbpDistributedLock

        // Configure<AbpDbContextOptions>(options =>
        // {
        //    options.Configure(opts =>
        //    {
        //        opts.DbContextOptions.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        //    });
        // });

        // Configure<AbpNewtonsoftJsonSerializerOptions>(options =>
        // {
        //    var c = options.Converters;
        //    options.Converters.Clear();
        // });

        // Configure<AbpClockOptions>(options => options.Kind = DateTimeKind.Utc);
        // https://github.com/abpframework/abp/issues/13404
        // Configure<AbpJsonOptions>(options => options.DefaultDateTimeFormat = "yyyy-MM-dd HH:mm:ss");
        // Configure<AbpSystemTextJsonSerializerOptions>(options =>
        // {
        //    //If the type is used as a property of other types, you also need to add it.
        //    options.UnsupportedTypes.Add<PagedIdentityUsersInput>();
        // });

        // Configure<AbpClaimsServiceOptions>(options =>
        // {
        //    options.RequestedClaims.Add(XXXClaim);
        // });

        // Configure<AbpAspNetCoreMultiTenancyOptions>(options =>
        // {
        //    // options.KeyPrefix = "KeyPrefix";
        //    options.MultiTenancyMiddlewareErrorPageBuilder = async (context, exception) =>
        //    {
        //        var isCookieAuthentication = false;
        //        var tenantResolveResult = context.RequestServices.GetRequiredService<ITenantResolveResultAccessor>().Result;
        //        if (tenantResolveResult != null)
        //        {
        //            if (tenantResolveResult.AppliedResolvers.Count == 1 && tenantResolveResult.AppliedResolvers.Contains(CurrentUserTenantResolveContributor.ContributorName))
        //            {
        //                var authenticationType = context.User.Identity?.AuthenticationType;
        //                if (authenticationType != null)
        //                {
        //                    var scheme = await context.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>().GetHandlerAsync(context, authenticationType);
        //                    if (scheme is CookieAuthenticationHandler cookieAuthenticationHandler)
        //                    {
        //                        // Try to delete the authentication's cookie if it does not exist or is inactive.
        //                        await cookieAuthenticationHandler.SignOutAsync(null);
        //                        isCookieAuthentication = true;
        //                    }
        //                }
        //            }

        // var options = context.RequestServices.GetRequiredService<IOptions<AbpAspNetCoreMultiTenancyOptions>>().Value;
        //            if (tenantResolveResult.AppliedResolvers.Contains(CookieTenantResolveContributor.ContributorName) ||
        //                context.Request.Cookies.ContainsKey(options.TenantKey))
        //            {
        //                // Try to delete the tenant's cookie if it does not exist or is inactive.
        //                AbpMultiTenancyCookieHelper.SetTenantCookie(context, null, options.TenantKey);
        //            }
        //        }

        // context.Response.Headers.Add("Abp-Tenant-Resolve-Error", HtmlEncoder.Default.Encode(exception.Message));
        //        if (isCookieAuthentication && context.Request.Method.Equals("Get", StringComparison.OrdinalIgnoreCase) && !context.Request.IsAjax())
        //        {
        //            context.Response.Redirect(context.Request.GetEncodedUrl());
        //        }
        //        else if (context.Request.IsAjax())
        //        {
        //            var error = new RemoteServiceErrorResponse(new RemoteServiceErrorInfo(exception.Message, exception is BusinessException businessException ? businessException.Details : string.Empty));

        // var jsonSerializerOptions = context.RequestServices.GetRequiredService<IOptions<JsonOptions>>().Value.SerializerOptions;

        // AbpVnext.Pro.ResponseContentTypeHelper.ResolveContentTypeAndEncoding(
        //                null,
        //                context.Response.ContentType,
        //                (new MediaTypeHeaderValue("application/json")
        //                {
        //                    Encoding = Encoding.UTF8
        //                }.ToString(), Encoding.UTF8),
        //                MediaType.GetEncoding,
        //                out var resolvedContentType,
        //                out var resolvedContentTypeEncoding);

        // context.Response.ContentType = resolvedContentType;
        //            context.Response.StatusCode = (int)HttpStatusCode.NotFound;

        // var responseStream = context.Response.Body;
        //            if (resolvedContentTypeEncoding.CodePage == Encoding.UTF8.CodePage)
        //            {
        //                try
        //                {
        //                    await JsonSerializer.SerializeAsync(responseStream, error, error.GetType(), jsonSerializerOptions, context.RequestAborted);
        //                    await responseStream.FlushAsync(context.RequestAborted);
        //                }
        //                catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested) { }
        //            }
        //            else
        //            {
        //                var transcodingStream = Encoding.CreateTranscodingStream(context.Response.Body, resolvedContentTypeEncoding, Encoding.UTF8, leaveOpen: true);
        //                ExceptionDispatchInfo exceptionDispatchInfo = null;
        //                try
        //                {
        //                    await JsonSerializer.SerializeAsync(transcodingStream, error, error.GetType(), jsonSerializerOptions, context.RequestAborted);
        //                    await transcodingStream.FlushAsync(context.RequestAborted);
        //                }
        //                catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested) { }
        //                catch (Exception ex)
        //                {
        //                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(ex);
        //                }
        //                finally
        //                {
        //                    try
        //                    {
        //                        await transcodingStream.DisposeAsync();
        //                    }
        //                    catch when (exceptionDispatchInfo != null)
        //                    {
        //                    }
        //                    exceptionDispatchInfo?.Throw();
        //                }
        //            }
        //        }
        //        else
        //        {
        //            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        //            context.Response.ContentType = "text/html";

        // var message = exception.Message;
        //            var details = exception is BusinessException businessException ? businessException.Details : string.Empty;

        // await context.Response.WriteAsync($"<html lang=\"{HtmlEncoder.Default.Encode(CultureInfo.CurrentCulture.Name)}\"><body>\r\n");
        //            await context.Response.WriteAsync($"<h3>{HtmlEncoder.Default.Encode(message)}</h3>{HtmlEncoder.Default.Encode(details)}<br>\r\n");
        //            await context.Response.WriteAsync("</body></html>\r\n");

        // // Note the 500 spaces are to work around an IE 'feature'
        //            await context.Response.WriteAsync(new string(' ', 500));
        //        }

        // return true;
        //    };
        // });

        // context.Services.Replace(
        //    ServiceDescriptor.Transient<IUserIdProvider, CustomSignalRUserIdProvider>()
        // );

        // context.Services.Replace(
        //    ServiceDescriptor.Transient<ITenantConfigurationProvider, CustomTenantConfigurationProvider>()
        // );
        //            context.Services.Replace(
        //    ServiceDescriptor.Transient<ITokenCreationService, CustomDefaultTokenCreationService>()
        // );
        // context.Services.Replace(ServiceDescriptor.Transient<ITokenService, CustomDefaultTokenService>());
        //        context.Services.Replace(
        //ServiceDescriptor.Transient<IUserInfoResponseGenerator, CustomUserInfoResponseGenerator>());

        //        context.Services.Replace(
        //ServiceDescriptor.Transient<IClaimsService, CustomDefaultClaimsService>());

        //CustomTokenResponseGenerator : ITokenResponseGenerator
        //context.Services.Replace(ServiceDescriptor.Transient<ITokenResponseGenerator, CustomTokenResponseGenerator>());

        //            // CustomTokenEndpoint : IEndpointHandler
        //            // CustomDefaultUserSession : IUserSession
        //            context.Services.Replace(
        // ServiceDescriptor.Transient<IUserSession, CustomDefaultUserSession>()
        // );

        // context.Services.Replace(ServiceDescriptor.Transient<IApiDescriptionModelProvider, CustomApiDescriptionModelProvider>());
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
        if (Convert.ToBoolean(configuration["AuthServer:SetSelfAsIssuer"]))
        {
            Configure<IdentityServerOptions>(options => { options.IssuerUri = configuration["App:SelfUrl"]; });
        }
        else
        {
            Configure<IdentityServerOptions>(options => { options.IssuerUri = configuration["AuthServer:Authority"]; });
        }

        context.Services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.Authority = configuration["AuthServer:Authority"];
                    options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                    options.Audience = "Pro";
                    options.MapInboundClaims = false;
                    options.BackchannelHttpHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback =
                            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                });

        /*
        // context.Services.AddAuthentication()
        context.Services
            //.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

            .AddAuthentication(options =>
        {
            // Console.WriteLine(IdentityConstants.ApplicationScheme);
            options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            // options.DefaultScheme = "Bearer_OR_Cookies";
            // options.DefaultChallengeScheme = "Bearer_OR_Cookies";
            // options.DefaultAuthenticateScheme = "Bearer_OR_Cookies";
            // options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            // options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            // options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            // options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            // options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            // options.
        })
            // .AddPolicyScheme("Bearer_OR_Cookies", "Bearer_OR_Cookies", options =>
            // {
            //    options.ForwardDefaultSelector = ctx =>
            //    {
            //        string authorization = ctx.Request.Headers.Authorization;
            //        if (!authorization.IsNullOrWhiteSpace() && authorization.StartsWith("Bearer ", StringComparison.InvariantCultureIgnoreCase))
            //        {
            //            return JwtBearerDefaults.AuthenticationScheme;
            //        }

            // //string xRequestedWith = ctx.Request.Headers.XRequestedWith;
            //        //if (!xRequestedWith.IsNullOrWhiteSpace() && xRequestedWith.Equals("XMLHttpRequest", StringComparison.OrdinalIgnoreCase))
            //        //{
            //        //    return JwtBearerDefaults.AuthenticationScheme;
            //        //}

            // var accessToken = ctx.Request.Query["access_token"];
            //        // If the request is for our hub...
            //        if (!string.IsNullOrWhiteSpace(accessToken) &&
            //            ctx.Request.Path.StartsWithSegments("/signalr-hubs"))
            //        {
            //            return JwtBearerDefaults.AuthenticationScheme;
            //        }
            //        //return (ctx.Request.Path.StartsWithSegments("/api")) ? JwtBearerDefaults.AuthenticationScheme : null;
            //        //return null;
            //        return CookieAuthenticationDefaults.AuthenticationScheme;
            //    };
            // })
            // .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";

                // options.Cookie.Name = "CustomerPortal.Identity";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(10); // Account.Login overrides this default value

                // options.Events = new CookieAuthenticationEvents
                // {
                //    //OnValidatePrincipal = async context =>
                //    //{
                //    //    Console.WriteLine();
                //    //    await Task.CompletedTask;
                //    //    //context.Response.ContentType = "application/json";
                //    //    //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                //    //    //await context.Response.WriteAsync("");
                //    //},

                // };
            })
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["AuthServer:Authority"];
                options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                options.Audience = "Pro";

                // options.Audience = configuration["AuthServer:Audience"];
                // TODO: ValidateIssuer
                options.TokenValidationParameters.ValidateIssuer = true;
                options.TokenValidationParameters.ValidateAudience = true;

                // See https://github.com/abpframework/abp/issues/18895
                options.MapInboundClaims = false;

                // options.TokenValidationParameters
                // options.SecurityTokenValidators.Clear();
                // options.SecurityTokenValidators.Add(new CustomSecurityTokenValidator(DateTime.Parse(configuration["App:TokenExpirationTime"])));
                options.BackchannelHttpHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync(string.Empty);
                    },

                    // OnAuthenticationFailed = async (ctx) =>
                    // {
                    //    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    //    await ctx.Response.WriteAsync(ctx.Exception.Message);
                    // },
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        if (!string.IsNullOrWhiteSpace(accessToken) &&
                            context.Request.Path.StartsWithSegments("/signalr-hubs"))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });
        */

        // .AddOpenIdConnect(options =>
        // {
        //    options.SignInScheme =
        //        CookieAuthenticationDefaults.AuthenticationScheme;
        //    options.Authority = "<OpenID Connect server URL>";
        //    options.RequireHttpsMetadata = true;
        //    options.ClientId = "<OpenID Connect client ID>";
        //    options.ClientSecret = "<>";
        //    // Code with PKCE can also be used here
        //    options.ResponseType = "code id_token";
        //    options.Scope.Add("profile");
        //    options.Scope.Add("offline_access");
        //    options.SaveTokens = true;
        //    options.Events = new OpenIdConnectEvents
        //    {
        //        OnRedirectToIdentityProvider = context =>
        //        {
        //            context.ProtocolMessage.SetParameter("acr_values", "mfa");
        //            return Task.FromResult(0);
        //        }
        //    };
        // });

        // Configure<AuthorizationOptions>(options =>
        // {
        //    //TODO: Rename UpdatePolicy/DeletePolicy since it's candidate to conflicts with other modules!
        //    //options.AddPolicy("Bearer_OR_Cookies", policy => policy.Requirements.Add(CommonOperations.Update));
        //    //options.AddPolicy("Bearer_OR_Cookies", policy => policy.RequireAuthenticatedUser().Build());
        //    ////Bearer_OR_Cookies
        //    //options.AddPolicy("Bearer_OR_Cookies", policy => policy.AuthenticationSchemes.().Build());

        // options.AddPolicy("DenyAnonyUser", policy => policy.RequireAuthenticatedUser());

        // });

        // context.Services.AddAuthorization(options =>
        // {
        //    options.AddPolicy("DenyAnonyUser", policy => policy.RequireAuthenticatedUser());

        // //options.DefaultPolicy =
        //    //    new AuthorizationPolicyBuilder("B2C", "AAD")
        //    //        .RequireAuthenticatedUser()
        //    //        .Build();

        // options.DefaultPolicy = new AuthorizationPolicyBuilder()
        //        .RequireAuthenticatedUser()
        //        .AddAuthenticationSchemes("Bearer_OR_Cookies")
        //        .Build();
        // });

        context.Services.ForwardIdentityAuthenticationForBearer();

        /*
         * 注：此代码在nuget包中无效，会话不会被保存
        context.Services.ConfigureApplicationCookie(options =>
        {
            //options.Events.OnSignedIn += async context =>
            //{
            //    var currentUser = context.HttpContext.RequestServices.GetRequiredService<ICurrentUser>();
            //    if (currentUser != null)
            //    {
            //        Console.WriteLine(currentUser.FindSessionId());
            //    }
            //};

            //Func<CookieSignedInContext, Task> previousOnSignedIn = options.Events.OnSignedIn;
            ////options.Events.OnSignedIn = async cookieSignedInContext =>
            ////{
            ////    await previousOnSignedIn(cookieSignedInContext);
            ////};

            //options.Events.OnSignedIn = cookieSignedInContext =>
            //{
            //    return previousOnSignedIn(cookieSignedInContext);
            //};

            //Func<CookieSigningOutContext, Task> previousOnSigningOut = options.Events.OnSigningOut;
            ////options.Events.OnSigningOut = async cookieSigningOutContext =>
            ////{
            ////    await previousOnSigningOut(cookieSigningOutContext);
            ////};

            //options.Events.OnSigningOut = cookieSigningOutContext =>
            //{
            //    return previousOnSigningOut(cookieSigningOutContext);
            //};

            // Cookie settings
            // options.Cookie.Name = settings.App;
            // options.Cookie.HttpOnly = true;
            // options.ExpireTimeSpan = TimeSpan.FromSeconds(10);
            // options.LoginPath = "/Account/Login";
            // options.LogoutPath = "/Account/Logout";
            // options.Events = new CookieAuthenticationEvents()
            // {
            //    OnRedirectToLogin = context =>
            //    {
            //        //这里区分当访问/api 如果cookie过期那么 不重定向到login登录界面
            //        //if (context.Request.Path.Value.StartsWith("/api"))
            //        //{
            //        //    context.Response.Clear();
            //        //    context.Response.StatusCode = 401;
            //        //    return Task.FromResult(0);
            //        //}
            //        string authorization = context.Request.Headers.Authorization;
            //        if (!authorization.IsNullOrWhiteSpace() && authorization.StartsWith("Bearer ", StringComparison.InvariantCultureIgnoreCase))
            //        {
            //            // context.HandleResponse();
            //            context.Response.Clear();
            //            context.Response.ContentType = "application/json";
            //            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //            //await context.Response.WriteAsync("");
            //            return Task.CompletedTask;
            //        }

            // //string xRequestedWith = context.Request.Headers.XRequestedWith;
            //        //if (!xRequestedWith.IsNullOrWhiteSpace() && xRequestedWith.Equals("XMLHttpRequest", StringComparison.OrdinalIgnoreCase))
            //        //{
            //        //    context.Response.Clear();
            //        //    context.Response.ContentType = "application/json";
            //        //    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //        //    //await context.Response.WriteAsync("");
            //        //    return Task.CompletedTask;
            //        //}

            // var accessToken = context.Request.Query["access_token"];
            //        // If the request is for our hub...
            //        if (!string.IsNullOrWhiteSpace(accessToken) &&
            //            context.Request.Path.StartsWithSegments("/signalr-hubs"))
            //        {
            //            context.Response.Clear();
            //            context.Response.ContentType = "application/json";
            //            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //            //await context.Response.WriteAsync("");
            //            return Task.CompletedTask;
            //        }

            // context.Response.Redirect(context.RedirectUri);
            //        return Task.CompletedTask;
            //    }
            // };
            // options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            options.ForwardDefaultSelector = ctx =>
            {
                string authorization = ctx.Request.Headers.Authorization;
                if (!authorization.IsNullOrWhiteSpace() && authorization.StartsWith("Bearer ", StringComparison.InvariantCultureIgnoreCase))
                {
                    return JwtBearerDefaults.AuthenticationScheme;
                }

                // string xRequestedWith = ctx.Request.Headers.XRequestedWith;
                // if (!xRequestedWith.IsNullOrWhiteSpace() && xRequestedWith.Equals("XMLHttpRequest", StringComparison.OrdinalIgnoreCase))
                // {
                //    return JwtBearerDefaults.AuthenticationScheme;
                // }
                var accessToken = ctx.Request.Query["access_token"];

                // If the request is for our hub...
                if (!string.IsNullOrWhiteSpace(accessToken) &&
                    ctx.Request.Path.StartsWithSegments("/signalr-hubs"))
                {
                    return JwtBearerDefaults.AuthenticationScheme;
                }

                // return (ctx.Request.Path.StartsWithSegments("/api")) ? JwtBearerDefaults.AuthenticationScheme : null;
                // return null;
                return CookieAuthenticationDefaults.AuthenticationScheme;
            };
        });
        */

        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });

        /*
         context.Services.ConfigureApplicationCookie((options =>
{
    Func<CookieSignedInContext, Task> previousOnSignedIn = options.Events.OnSignedIn;
    options.Events.OnSignedIn = (async cookieSignedInContext =>
    {
        await previousOnSignedIn(cookieSignedInContext);
        ILogger<AbpAccountPublicWebModule> logger = cookieSignedInContext.HttpContext.RequestServices.GetRequiredService<ILogger<AbpAccountPublicWebModule>>();
        string sessionId = cookieSignedInContext.Principal?.FindSessionId();
        if (sessionId != null)
        {
            IdentitySessionManager identitySessionManager = cookieSignedInContext.HttpContext.RequestServices.GetRequiredService<IdentitySessionManager>();
            IWebClientInfoProvider webClientInfoProvider = cookieSignedInContext.HttpContext.RequestServices.GetRequiredService<IWebClientInfoProvider>();
            logger.LogDebug("Create" + sessionId);
            IdentitySession identitySession = await identitySessionManager.CreateAsync(sessionId, webClientInfoProvider.DeviceInfo, cookieSignedInContext.Principal.FindUserId().Value, cookieSignedInContext.Principal.FindTenantId(), cookieSignedInContext.Principal.FindClientId(), webClientInfoProvider.ClientIpAddress, true);
        }
        else
        {
            logger.LogError("Not find sessionId");
        }
    });

    Func<CookieSigningOutContext, Task> previousOnSigningOut = options.Events.OnSigningOut;
    options.Events.OnSigningOut = (async cookieSigningOutContext =>
    {
        await previousOnSigningOut(cookieSigningOutContext);
        ILogger<AbpAccountPublicWebModule> logger = cookieSigningOutContext.HttpContext.RequestServices.GetRequiredService<ILogger<AbpAccountPublicWebModule>>();
        string sessionId = cookieSigningOutContext.HttpContext.RequestServices.GetRequiredService<ICurrentUser>().FindSessionId();
        if (sessionId != null)
        {
            IdentitySessionManager identitySessionManager = cookieSigningOutContext.HttpContext.RequestServices.GetRequiredService<IdentitySessionManager>();
            logger.LogDebug("Revoke" + sessionId);
            await identitySessionManager.RevokeAsync(sessionId);
        }
        else
        {
            logger.LogError("Not find sessionId");
        }
    });
}));
 */
    }

    private static void ConfigureHealthChecks(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddProHealthChecks(configuration["App:SelfUrl"]);
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
            options.Applications["Angular"].RootUrl = configuration["App:AngularUrl"];
            options.Applications["Angular"].Urls[AccountUrlNames.PasswordReset] = "account/reset-password";
            options.Applications["Angular"].Urls[AccountUrlNames.EmailConfirmation] = "account/email-confirmation";
            options.RedirectAllowedUrls.AddRange(configuration["App:RedirectAllowedUrls"].Split(','));
        });
    }

    private void ConfigureBundles()
    {
        Configure<AbpBundlingOptions>(options =>
        {
            options.StyleBundles.Configure(
                BasicThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                });

            // options.ScriptBundles
            // .Configure(
            //    typeof(global::Pages.Abp.MultiTenancy.TenantSwitchModalModel).FullName,
            //    bundleConfig =>
            //    {
            //        bundleConfig.AddFiles("/Pages/Abp/MultiTenancy/tenant-switch.js");
            //    });
        });
    }

    private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<ProDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}AbpVnext.Pro.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<ProDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}AbpVnext.Pro.Domain"));
                options.FileSets.ReplaceEmbeddedByPhysical<ProApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}AbpVnext.Pro.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<ProApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}AbpVnext.Pro.Application"));
            });
        }
    }

    private void ConfigureConventionalControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(ProApplicationModule).Assembly);
            options.ExposeIntegrationServices = true;
        });
    }

    private static void ConfigureSwagger(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAbpSwaggerGenWithOAuth(
            configuration["AuthServer:Authority"],
            new Dictionary<string, string>
            {
                { "Pro", "IdentityServer Pro API" }
            },
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "IdentityServer Pro API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);

                Directory.GetFiles(AppContext.BaseDirectory, "*.xml").ToList().ForEach(file =>
                {
                    options.IncludeXmlComments(file, true);
                });

                options.ShowEnumDescription();
            });
    }

    private void ConfigureLocalization()
    {
        /*
        //Configure<AbpLocalizationCultureMapOptions>(options =>
        //{
        //    var zhHansCultureMapInfo = new CultureMapInfo
        //    {
        //        TargetCulture = "zh-Hans",
        //        SourceCultures = new string[] { "zh", "zh_CN", "zh_cn", "zh-CN", "zh-cn" }
        //    };
        //    // TODO: zh-Hant
        //    options.CulturesMaps.Add(zhHansCultureMapInfo);
        //    options.UiCulturesMaps.Add(zhHansCultureMapInfo);
        //});
        */
    }

    private static void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(
                        configuration["App:CorsOrigins"]
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.Trim().RemovePostFix("/"))
                            .ToArray())
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    private static void ConfigureExternalProviders(ServiceConfigurationContext context)
    {
        context.Services.AddAuthentication()
            .AddGitHub(GitHubAuthenticationDefaults.AuthenticationScheme, options =>
            {
                // options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.Scope.Add("user:email");
            })
            .WithDynamicOptions<GitHubAuthenticationOptions, GitHubAuthenticationHandler>(
            GitHubAuthenticationDefaults.AuthenticationScheme,
            options =>
            {
                options.WithProperty(x => x.ClientId);
                options.WithProperty(x => x.ClientSecret, isSecret: true);
            })
            .AddGoogle(GoogleDefaults.AuthenticationScheme, _ => { })
            .WithDynamicOptions<GoogleOptions, GoogleHandler>(
                GoogleDefaults.AuthenticationScheme,
                options =>
                {
                    options.WithProperty(x => x.ClientId);
                    options.WithProperty(x => x.ClientSecret, isSecret: true);
                })
            .AddMicrosoftAccount(MicrosoftAccountDefaults.AuthenticationScheme, options =>
            {
                // Personal Microsoft accounts as an example.
                options.AuthorizationEndpoint = "https://login.microsoftonline.com/consumers/oauth2/v2.0/authorize";
                options.TokenEndpoint = "https://login.microsoftonline.com/consumers/oauth2/v2.0/token";
            })
            .WithDynamicOptions<MicrosoftAccountOptions, MicrosoftAccountHandler>(
                MicrosoftAccountDefaults.AuthenticationScheme,
                options =>
                {
                    options.WithProperty(x => x.ClientId);
                    options.WithProperty(x => x.ClientSecret, isSecret: true);
                })
            .AddTwitter(TwitterDefaults.AuthenticationScheme, options => options.RetrieveUserDetails = true)
            .WithDynamicOptions<TwitterOptions, TwitterHandler>(
                TwitterDefaults.AuthenticationScheme,
                options =>
                {
                    options.WithProperty(x => x.ConsumerKey);
                    options.WithProperty(x => x.ConsumerSecret, isSecret: true);
                });
    }

    private static void ConfigureImpersonation(ServiceConfigurationContext context)
    {
        context.Services.Configure<AbpAccountOptions>(options =>
        {
            options.TenantAdminUserName = "admin";
            options.ImpersonationTenantPermission = AbpSaasPermissions.Tenants.Impersonation;
            options.ImpersonationUserPermission = AbpIdentityProPermissions.Users.Impersonation;
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // acme
        // app.Map("/.well-known/acme-challenge", config =>
        // {
        //    config.Run(async context =>
        //    {
        //        var token = context.Request.Path.ToString().TrimStart('/');
        //        await context.Response.WriteAsync($"{token}.{configuration["Certificates:AccountThumbprint"]}");
        //    });
        // });
        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
        }

        app.UseCookiePolicy();

        var fordwardedHeaderOptions = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        };
        fordwardedHeaderOptions.KnownNetworks.Clear();
        fordwardedHeaderOptions.KnownProxies.Clear();
        app.UseForwardedHeaders(fordwardedHeaderOptions);

        app.UseAbpSecurityHeaders();

        // DefaultFilesOptions defaultFilesOptions = new();
        // defaultFilesOptions.DefaultFileNames.Clear();
        // defaultFilesOptions.DefaultFileNames.Add("index.html");
        // app.UseDefaultFiles(defaultFilesOptions);
        app.UseStaticFiles();

        // 下载文件扩展
        // app.UseStaticFiles(new StaticFileOptions
        // {
        //    //FileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory()),
        //    //设置可以下载apk和nupkg类型的文件
        //    ContentTypeProvider = new FileExtensionContentTypeProvider(new Dictionary<string, string>
        //    {
        //        { ".apk", "application/vnd.android.package-archive" },
        //        { ".exe", "application/octet-stream" },
        //        { ".dmg", "application/octet-stream" }
        //    })
        // });
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseJwtTokenMiddleware();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseUnitOfWork();

        app.UseIdentityServer();
        app.UseDynamicClaims();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityServer Pro API");

            var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);

            // options.OAuthClientSecret(configuration["AuthServer:SwaggerClientSecret"]);
            // options.OAuthScopes("Pro");
        });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();

        app.UseConfiguredEndpoints();

        // app.UseConfiguredEndpoints(endpoints =>
        // {
        //    endpoints.MapGrpcService<HookProviderService>();

        // // endpoints.MapGrpcHealthChecksService();
        // });
    }
}
