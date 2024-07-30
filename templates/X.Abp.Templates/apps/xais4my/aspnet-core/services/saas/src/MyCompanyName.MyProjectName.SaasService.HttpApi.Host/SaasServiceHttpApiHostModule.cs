using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyCompanyName.MyProjectName.SaasService.Application;
using MyCompanyName.MyProjectName.SaasService.DbMigrations;
using MyCompanyName.MyProjectName.SaasService.EntityFramework;
using MyCompanyName.MyProjectName.Shared.Hosting.AspNetCore;
using MyCompanyName.MyProjectName.Shared.Hosting.Microservices;
using Prometheus;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace MyCompanyName.MyProjectName.SaasService;

[DependsOn(
    typeof(MyProjectNameSharedHostingMicroservicesModule),
    typeof(SaasServiceEntityFrameworkCoreModule),
    typeof(SaasServiceApplicationModule),
    typeof(SaasServiceHttpApiModule)
)]
public class SaasServiceHttpApiHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Enable if you need these
        // var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        context.Services.AddAbpDbContext<SaasServiceDbContext>();
        JwtBearerConfigurationHelper.Configure(context, "SaasService");
        SwaggerConfigurationHelper.ConfigureWithAuth(
            context: context,
            authority: configuration["AuthServer:Authority"],
            scopes: new
                Dictionary<string, string> /* Requested scopes for authorization code request and descriptions for swagger UI only */ {
                    { "SaasService", "Saas Service API" }
                },
            apiTitle: "Saas Service API"
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
        app.UseAbpRequestLocalization();
        app.UseAbpSecurityHeaders();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseHttpMetrics();
        app.UseAuthentication();
        app.UseAbpClaimsMap();
        app.UseMultiTenancy();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Saas Service API");
            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            options.OAuthClientSecret(configuration["AuthServer:SwaggerClientSecret"]);
        });
        app.UseAbpSerilogEnrichers();
        app.UseAuditing();
        app.UseUnitOfWork();
        app.UseConfiguredEndpoints(endpoints =>
        {
            endpoints.MapMetrics();
        });
    }

    public async override Task OnPostApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        using (var scope = context.ServiceProvider.CreateScope())
        {
            await scope.ServiceProvider
                .GetRequiredService<SaasServiceDatabaseMigrationChecker>()
                .CheckAndApplyDatabaseMigrationsAsync();
        }
    }
}
