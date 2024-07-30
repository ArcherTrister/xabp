using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyCompanyName.MyProjectName.Shared.Hosting.AspNetCore;
using MyCompanyName.MyProjectName.Shared.Hosting.Gateways;

using Volo.Abp;
using Volo.Abp.Modularity;

using Yarp.ReverseProxy.Configuration;

namespace MyCompanyName.MyProjectName.WebGateway;

[DependsOn(
    typeof(MyProjectNameSharedHostingGatewaysModule)
)]
public class MyProjectNameWebGatewayModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Enable if you need hosting environment
        // var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        SwaggerConfigurationHelper.ConfigureWithAuth(
            context: context,
            authority: configuration["AuthServer:Authority"],
            scopes: new
                Dictionary<string, string> /* Requested scopes for authorization code request and descriptions for swagger UI only */ {
                    { "AccountService", "Account Service API" },
                    { "IdentityService", "Identity Service API" },
                    { "AdministrationService", "Administration Service API" },
                    { "SaasService", "Saas Service API" },
                    { "ProductService", "Product Service API" }
                },
            apiTitle: "Web Gateway API"
        );

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
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseCors();
        app.UseRouting();
        app.UseAuthorization();
        app.UseCors();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
            var proxyConfig = app.ApplicationServices.GetRequiredService<IProxyConfigProvider>().GetConfig();
            foreach (var cluster in proxyConfig.Clusters)
            {
                options.SwaggerEndpoint($"/swagger-json/{cluster.ClusterId}/swagger/v1/swagger.json", $"{cluster.ClusterId} API");
            }

            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            options.OAuthScopes(
            "AdministrationService",
            "AccountService",
            "IdentityService",
            "SaasService",
            "ProductService"
            );
        });
        app.UseAbpSerilogEnrichers();
        app.UseRewriter(new RewriteOptions()
            // Regex for "", "/" and "" (whitespace)
            .AddRedirect("^(|\\|\\s+)$", "/swagger"));
        app.UseEndpoints(endpoints => endpoints.MapReverseProxy());
    }
}
