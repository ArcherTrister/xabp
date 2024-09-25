// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MyCompanyName.MyProjectName.EntityFrameworkCore;
using MyCompanyName.MyProjectName.HealthChecks;
using MyCompanyName.MyProjectName.MultiTenancy;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using IdentityServer4.Configuration;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;

using Volo.Abp;
using Volo.Abp.AspNetCore.ExceptionHandling;
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
using Volo.Abp.Guids;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Account;
using X.Abp.Account.Public.Web;
using X.Abp.Account.Public.Web.ExternalProviders;
using X.Abp.Account.Public.Web.Impersonation;
using X.Abp.Account.Web;
using X.Abp.Identity.Permissions;
using X.Abp.IdentityServer;
using X.Abp.Saas.Permissions;

namespace MyCompanyName.MyProjectName
{
    [DependsOn(
        typeof(MyProjectNameApplicationModule),
        typeof(MyProjectNameEntityFrameworkCoreModule),
        typeof(MyProjectNameHttpApiModule),
        typeof(AbpAutofacModule),
        typeof(AbpAspNetCoreMultiTenancyModule),
        typeof(AbpAspNetCoreMvcUiBasicThemeModule),
        typeof(AbpAccountPublicWebImpersonationModule),
        typeof(AbpAccountPublicWebIdentityServerModule),
        typeof(AbpSwashbuckleModule),
        typeof(AbpAspNetCoreSerilogModule))]
    public class MyProjectNameHttpApiHostModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();
            var configuration = context.Services.GetConfiguration();

            PreConfigure<AuthorizationOptions>(options =>
            {
                options.AddPolicy("TwoFactorEnabled",
                    x => x.RequireClaim("amr", "mfa"));

                // options.AddPolicy("RequireMfa", policyIsAdminRequirement =>
                // {
                //     policyIsAdminRequirement.Requirements.Add(new RequireMfa());
                // });
            });

            if (hostingEnvironment.IsDevelopment())
            {
                PreConfigure<AbpIdentityServerBuilderOptions>(options =>
                {
                    options.AddDeveloperSigningCredential = true;
                });
            }
            else
            {
                PreConfigure<AbpIdentityServerBuilderOptions>(options =>
                {
                    options.AddDeveloperSigningCredential = false;
                });

                // IIS 应用程序池--->高级设置--->加载用户配置文件设置为True
                string password = configuration["Certificates:Password"];

                // Debug.Assert(!string.IsNullOrEmpty(password), "Certificates:Password is missing from appsettings");
                string certificate = Path.Combine(hostingEnvironment.ContentRootPath, configuration["Certificates:CerPath"]);

                PreConfigure<IIdentityServerBuilder>(identityServerBuilder =>
                {
                    identityServerBuilder.AddSigningCredential(new X509Certificate2(certificate, password));
                });
            }
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

            // Configure<CookieAuthenticationOptions>(IdentityConstants.TwoFactorRememberMeScheme, options =>
            // {
            //     options.ExpireTimeSpan = TimeSpan.FromDays(30); // override the timeout
            //     // options.Cookie.Name = "MyRememberMeCookieName"; // override the cookie name
            // });
            ConfigureAuthentication(context, configuration);
            ConfigureUrls(configuration);
            ConfigureBundles();
            ConfigureConventionalControllers();
            ConfigureImpersonation(context);
            ConfigureSwagger(context, configuration);
            ConfigureVirtualFileSystem(context);
            ConfigureCors(context, configuration);

            ConfigureHealthChecks(context, configuration);

            Configure<AbpAntiForgeryOptions>(options =>
            {
                // options.TokenCookie.Expiration = TimeSpan.FromDays(365);
                // options.AutoValidateIgnoredHttpMethods.Remove("GET");
                // options.AutoValidateFilter = type => !type.Namespace.StartsWith("MyProject.MyIgnoredNamespace");
                options.AutoValidate = false;
            });

            Configure<AbpExceptionHandlingOptions>(options =>
            {
                options.SendExceptionsDetailsToClients = true;
            });

            Configure<AbpSequentialGuidGeneratorOptions>(options =>
            {
                options.DefaultSequentialGuidType = SequentialGuidType.SequentialAsString;
            });

            /*
            Configure<AbpBlobStoringOptions>(options =>
            {
                options.Containers.ConfigureDefault(container =>
                {
                    container.UseFileSystem(fileSystem =>
                    {
                        fileSystem.BasePath = $"{hostingEnvironment.ContentRootPath}/default-files";
                    });
                });
                options.Containers.Configure<FileManagementContainer>(c =>
                {
                    c.UseFileSystem(fileSystem =>
                    {
                        fileSystem.BasePath = $"{hostingEnvironment.ContentRootPath}/files";
                    });
                });
            });

            Configure<AbpAuditingOptions>(options =>
            {
                options.EntityHistorySelectors.AddAllEntities();
                options.EntityHistorySelectors.Add(
                new NamedTypeSelector("MySelectorName",
                    type =>
                    {
                        if (typeof(IEntity).IsAssignableFrom(type))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }));
            });

            Configure<AbpSystemTextJsonSerializerOptions>(options =>
            {
                // If the type is used as a property of other types, you also need to add it.
                options.UnsupportedTypes.Add<CreateXXXDto>();
                options.UnsupportedTypes.Add<UpdateXXXDto>();
            });

            Configure<AbpClaimsServiceOptions>(options =>
            {
                options.RequestedClaims.Add(XXXClaim);
            });

            Configure<AbpDistributedCacheOptions>(options =>
            {
                options.KeyPrefix = "MyProjectName";
            });
            */
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

            var builder = context.Services.AddAuthentication()
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Account/AccessDenied";

                    // options.Cookie.Name = "CustomerPortal.Identity";
                    options.SlidingExpiration = true;
                    options.ExpireTimeSpan = TimeSpan.FromDays(10);

                    // options.Events = new CookieAuthenticationEvents
                    // {
                    //     //OnValidatePrincipal = async context =>
                    //     //{
                    //     //    Console.WriteLine();
                    //     //    await Task.CompletedTask;
                    //     //    //context.Response.ContentType = "application/json";
                    //     //    //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    //     //    //await context.Response.WriteAsync("");
                    //     //},
                    // };
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = configuration["AuthServer:Authority"];
                    options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);

                    // options.Audience = "MyProjectName";
                    options.Audience = configuration["AuthServer:Audience"];
                    options.TokenValidationParameters.ValidateIssuer = Convert.ToBoolean(configuration["AuthServer:ValidateIssuer"]);

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
                            await context.Response.WriteAsync("");
                        },
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

            ConfigureExternalProviders(builder);

            // context.Services.ForwardIdentityAuthenticationForBearer();
            context.Services.ConfigureApplicationCookie(options =>
            {
                options.ForwardDefaultSelector = ctx =>
                {
                    string authorization = ctx.Request.Headers.Authorization;
                    if (!authorization.IsNullOrWhiteSpace() && authorization.StartsWith("Bearer ", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return JwtBearerDefaults.AuthenticationScheme;
                    }

                    // If the request is for our hub...
                    var accessToken = ctx.Request.Query["access_token"];

                    if (!string.IsNullOrWhiteSpace(accessToken) &&
                        ctx.Request.Path.StartsWithSegments("/signalr-hubs"))
                    {
                        return JwtBearerDefaults.AuthenticationScheme;
                    }

                    return CookieAuthenticationDefaults.AuthenticationScheme;
                };
            });
        }

        private static void ConfigureHealthChecks(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddMyProjectNameHealthChecks(configuration["App:SelfUrl"]);
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
            });
        }

        private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();

            if (hostingEnvironment.IsDevelopment())
            {
                Configure<AbpVirtualFileSystemOptions>(options =>
                {
                    options.FileSets.ReplaceEmbeddedByPhysical<MyProjectNameDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}MyCompanyName.MyProjectName.Domain.Shared"));
                    options.FileSets.ReplaceEmbeddedByPhysical<MyProjectNameDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}MyCompanyName.MyProjectName.Domain"));
                    options.FileSets.ReplaceEmbeddedByPhysical<MyProjectNameApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}MyCompanyName.MyProjectName.Application.Contracts"));
                    options.FileSets.ReplaceEmbeddedByPhysical<MyProjectNameApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}MyCompanyName.MyProjectName.Application"));
                });
            }
        }

        private void ConfigureConventionalControllers()
        {
            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.Create(typeof(MyProjectNameApplicationModule).Assembly);
            });
        }

        private static void ConfigureSwagger(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddAbpSwaggerGenWithOAuth(
                configuration["AuthServer:Authority"],
                new Dictionary<string, string>
                {
                    { "MyProjectName", "MyProjectName API" }
                },
                options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "MyProjectName API", Version = "v1" });
                    options.DocInclusionPredicate((docName, description) => true);
                    options.CustomSchemaIds(type => type.FullName);

                    Directory.GetFiles(AppContext.BaseDirectory, "*.xml").ToList().ForEach(file =>
                    {
                        options.IncludeXmlComments(file, true);
                    });

                    options.ShowEnumDescription();
                });
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

        private void ConfigureExternalProviders(AuthenticationBuilder builder)
        {
            builder
                .AddGoogle(GoogleDefaults.AuthenticationScheme, _ => { })
                .WithDynamicOptions<GoogleOptions, GoogleHandler>(
                    GoogleDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.WithProperty(x => x.ClientId);
                        options.WithProperty(x => x.ClientSecret, isSecret: true);
                    }
                )
                .AddMicrosoftAccount(MicrosoftAccountDefaults.AuthenticationScheme, options =>
                {
                    //Personal Microsoft accounts as an example.
                    options.AuthorizationEndpoint = "https://login.microsoftonline.com/consumers/oauth2/v2.0/authorize";
                    options.TokenEndpoint = "https://login.microsoftonline.com/consumers/oauth2/v2.0/token";
                })
                .WithDynamicOptions<MicrosoftAccountOptions, MicrosoftAccountHandler>(
                    MicrosoftAccountDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.WithProperty(x => x.ClientId);
                        options.WithProperty(x => x.ClientSecret, isSecret: true);
                    }
                )
                .AddTwitter(TwitterDefaults.AuthenticationScheme, options => options.RetrieveUserDetails = true)
                .WithDynamicOptions<TwitterOptions, TwitterHandler>(
                    TwitterDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.WithProperty(x => x.ConsumerKey);
                        options.WithProperty(x => x.ConsumerSecret, isSecret: true);
                    }
                );
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
            IApplicationBuilder app = context.GetApplicationBuilder();
            IWebHostEnvironment env = context.GetEnvironment();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAbpRequestLocalization();

            if (!env.IsDevelopment())
            {
                app.UseErrorPage();
            }

            app.UseCookiePolicy();
            app.UseCorrelationId();

            var fordwardedHeaderOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };
            fordwardedHeaderOptions.KnownNetworks.Clear();
            fordwardedHeaderOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(fordwardedHeaderOptions);

            app.UseAbpSecurityHeaders();

            app.UseStaticFiles();

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
            app.UseAuthorization();
            app.UseSwagger();
            app.UseAbpSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "MyProjectName API");

                var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
                options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);

                // options.OAuthClientSecret(configuration["AuthServer:SwaggerClientSecret"]);
                // options.OAuthScopes("MyProjectName");
            });
            app.UseAuditing();
            app.UseAbpSerilogEnrichers();
            app.UseConfiguredEndpoints();
        }
    }
}
