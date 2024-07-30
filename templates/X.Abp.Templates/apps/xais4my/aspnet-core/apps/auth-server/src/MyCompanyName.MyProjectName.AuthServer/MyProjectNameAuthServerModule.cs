using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

using IdentityServer4.Extensions;

using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

using MyCompanyName.MyProjectName.AdministrationService.EntityFrameworkCore;
using MyCompanyName.MyProjectName.IdentityService.EntityFramework;
using MyCompanyName.MyProjectName.SaasService.EntityFramework;
using MyCompanyName.MyProjectName.Shared.Hosting.AspNetCore;

using Prometheus;

using StackExchange.Redis;

using Volo.Abp;
using Volo.Abp.AspNetCore.Authentication.JwtBearer;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.Auditing;
using Volo.Abp.BackgroundJobs.RabbitMQ;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Emailing;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Account;
using X.Abp.Account.Public.Web.ExternalProviders;
using X.Abp.Account.Web;
using X.Abp.AspNetCore.Mvc.UI.Theme.Lepton;
using X.Abp.IdentityServer;

namespace MyCompanyName.MyProjectName.AuthServer;

[DependsOn(
    typeof(AbpAspNetCoreAuthenticationJwtBearerModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpEventBusRabbitMqModule),
    typeof(AbpBackgroundJobsRabbitMqModule),
    typeof(AbpAspNetCoreMvcUiLeptonThemeModule),
    typeof(AbpAccountPublicWebIdentityServerModule),
    typeof(AbpAccountPublicApplicationModule),
    typeof(AbpAccountPublicHttpApiModule),
    typeof(AdministrationServiceEntityFrameworkCoreModule),
    typeof(IdentityServiceEntityFrameworkCoreModule),
    typeof(SaasServiceEntityFrameworkCoreModule),
    typeof(MyProjectNameSharedHostingAspNetCoreModule),
    typeof(MyProjectNameSharedLocalizationModule)
)]
public class MyProjectNameAuthServerModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        if (!hostingEnvironment.IsDevelopment())
        {
            var configuration = context.Services.GetConfiguration();

            PreConfigure<AbpIdentityServerBuilderOptions>(options =>
            {
                options.AddDeveloperSigningCredential = false;
            });

            PreConfigure<IIdentityServerBuilder>(builder =>
            {
                builder.AddSigningCredential(GetSigningCertificate(hostingEnvironment, configuration));
            });
        }
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //You can disable this setting in production to avoid any potential security risks.
        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();
        ConfigureSwagger(context, configuration);

        context.Services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["AuthServer:Authority"];
                options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                options.Audience = "AccountService";
            });

        ConfigureExternalProviders(context);

        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = true;
        });

        Configure<AbpAuditingOptions>(options =>
        {
            options.ApplicationName = "AuthServer";
        });

        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
            options.RedirectAllowedUrls.AddRange(configuration["App:RedirectAllowedUrls"].Split(','));
        });

        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = "MyProjectName:";
        });

        var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("MyProjectName");
        var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
        dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "MyProjectName-Protection-Keys");

        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(
                        configuration["App:CorsOrigins"]
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.Trim().RemovePostFix("/"))
                            .ToArray()
                    )
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

#if DEBUG
        context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
#endif

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<MyProjectNameSharedLocalizationModule>(Path.Combine(
                    hostingEnvironment.ContentRootPath,
                    $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}shared{Path.DirectorySeparatorChar}MyCompanyName.MyProjectName.Shared.Localization"));
            });
        }
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();

        app.Use(async (ctx, next) =>
        {
            if (ctx.Request.Headers.ContainsKey("from-ingress"))
            {
                ctx.SetIdentityServerOrigin(configuration["App:SelfUrl"]);
            }

            await next();
        });

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
        }

        app.UseCorrelationId();
        app.UseAbpSecurityHeaders();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseHttpMetrics();
        app.UseAuthentication();
        app.UseJwtTokenMiddleware();
        app.UseMultiTenancy();
        app.UseAbpSerilogEnrichers();
        app.UseUnitOfWork();
        app.UseIdentityServer();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Account Service API");
            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            options.OAuthClientSecret(configuration["AuthServer:SwaggerClientSecret"]);
        });
        app.UseAuditing();
        app.UseConfiguredEndpoints(endpoints =>
        {
            endpoints.MapMetrics();
        });
    }

    private void ConfigureExternalProviders(ServiceConfigurationContext context)
    {
        context.Services.AddAuthentication()
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

    private X509Certificate2 GetSigningCertificate(IWebHostEnvironment hostingEnv, IConfiguration configuration)
    {
        var fileName = "authserver.pfx";
        var passPhrase = "2D7AA457-5D33-48D6-936F-C48E5EF468ED";
        var file = Path.Combine(hostingEnv.ContentRootPath, fileName);

        if (!File.Exists(file))
        {
            throw new FileNotFoundException($"Signing Certificate couldn't found: {file}");
        }

        return new X509Certificate2(file, passPhrase);
    }

    private void ConfigureSwagger(ServiceConfigurationContext context, IConfiguration configuration)
    {
        SwaggerConfigurationHelper.ConfigureWithAuth(
            context: context,
            authority: configuration["AuthServer:Authority"],
            scopes: new Dictionary<string, string> {
                /* Requested scopes for authorization code request and descriptions for swagger UI only */
                { "AccountService", "Account Service API" }
            },
            apiTitle: "Account Service API"
        );
    }
}
