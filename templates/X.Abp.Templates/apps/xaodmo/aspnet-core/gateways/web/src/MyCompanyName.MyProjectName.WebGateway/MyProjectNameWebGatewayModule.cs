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
using Ocelot.Middleware;
using Volo.Abp;
using Volo.Abp.Modularity;

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
            authority: configuration["AuthServer:Authority"]!,
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
                        configuration["App:CorsOrigins"]?
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.Trim().RemovePostFix("/"))
                            .ToArray() ?? Array.Empty<string>()
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
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
            var routes = configuration.GetSection("Routes").Get<List<OcelotConfiguration>>()!;
            var routedServices = routes
                .GroupBy(t => t.ServiceKey)
                .Select(r => r.First())
                .Distinct();

            foreach (var config in routedServices.OrderBy(q => q.ServiceKey))
            {
                if (config.DownstreamHostAndPorts != null)
                {
                    var url = $"{config.DownstreamScheme}://{config.DownstreamHostAndPorts.FirstOrDefault()?.Host}:{config.DownstreamHostAndPorts.FirstOrDefault()?.Port}";
                    if (!env.IsDevelopment())
                    {
                        url = $"https://{config.DownstreamHostAndPorts.FirstOrDefault()?.Host}";
                    }

                    options.SwaggerEndpoint($"{url}/swagger/v1/swagger.json", $"{config.ServiceKey} API");
                }

                options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            }
        });
        app.UseAbpSerilogEnrichers();
        app.UseRewriter(new RewriteOptions()
            // Regex for "", "/" and "" (whitespace)
            .AddRedirect("^(|\\|\\s+)$", "/swagger"));
        app.UseOcelot().Wait();
    }
}
