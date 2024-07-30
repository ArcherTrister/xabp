FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# It's important to keep lines from here down to "COPY . ." identical in all Dockerfiles
# to take advantage of Docker's build cache, to speed up local container builds
COPY "MyCompanyName.MyProjectName.sln" "MyCompanyName.MyProjectName.sln"

# Applications
COPY "apps/auth-server/src/MyCompanyName.MyProjectName.AuthServer/MyCompanyName.MyProjectName.AuthServer.csproj" "apps/auth-server/src/MyCompanyName.MyProjectName.AuthServer/MyCompanyName.MyProjectName.AuthServer.csproj"
COPY "apps/public-web/src/MyCompanyName.MyProjectName.PublicWeb/MyCompanyName.MyProjectName.PublicWeb.csproj" "apps/public-web/src/MyCompanyName.MyProjectName.PublicWeb/MyCompanyName.MyProjectName.PublicWeb.csproj"
COPY *"apps/blazor/src/MyCompanyName.MyProjectName.Blazor/MyCompanyName.MyProjectName.Blazor.csproj" "apps/blazor/src/MyCompanyName.MyProjectName.Blazor/MyCompanyName.MyProjectName.Blazor.csproj"
COPY *"apps/blazor/src/MyCompanyName.MyProjectName.Blazor.Server/MyCompanyName.MyProjectName.Blazor.Server.csproj" "apps/blazor/src/MyCompanyName.MyProjectName.Blazor.Server/MyCompanyName.MyProjectName.Blazor.Server.csproj"
COPY *"apps/web/src/MyCompanyName.MyProjectName.Web/MyCompanyName.MyProjectName.Web.csproj" "apps/web/src/MyCompanyName.MyProjectName.Web/MyCompanyName.MyProjectName.Web.csproj"

# Shared Projects
COPY "shared/MyCompanyName.MyProjectName.DbMigrator/MyCompanyName.MyProjectName.DbMigrator.csproj" "shared/MyCompanyName.MyProjectName.DbMigrator/MyCompanyName.MyProjectName.DbMigrator.csproj"
COPY "shared/MyCompanyName.MyProjectName.Shared.Hosting/MyCompanyName.MyProjectName.Shared.Hosting.csproj" "shared/MyCompanyName.MyProjectName.Shared.Hosting/MyCompanyName.MyProjectName.Shared.Hosting.csproj"
COPY "shared/MyCompanyName.MyProjectName.Shared.Hosting.Gateways/MyCompanyName.MyProjectName.Shared.Hosting.Gateways.csproj" "shared/MyCompanyName.MyProjectName.Shared.Hosting.Gateways/MyCompanyName.MyProjectName.Shared.Hosting.Gateways.csproj"
COPY "shared/MyCompanyName.MyProjectName.Shared.Hosting.Microservices/MyCompanyName.MyProjectName.Shared.Hosting.Microservices.csproj" "shared/MyCompanyName.MyProjectName.Shared.Hosting.Microservices/MyCompanyName.MyProjectName.Shared.Hosting.Microservices.csproj"
COPY "shared/MyCompanyName.MyProjectName.Shared.Hosting.AspNetCore/MyCompanyName.MyProjectName.Shared.Hosting.AspNetCore.csproj" "shared/MyCompanyName.MyProjectName.Shared.Hosting.AspNetCore/MyCompanyName.MyProjectName.Shared.Hosting.AspNetCore.csproj"
COPY "shared/MyCompanyName.MyProjectName.Shared.Hosting/MyCompanyName.MyProjectName.Shared.Hosting.csproj" "shared/MyCompanyName.MyProjectName.Shared.Hosting/MyCompanyName.MyProjectName.Shared.Hosting.csproj"
COPY "shared/MyCompanyName.MyProjectName.Shared.Localization/MyCompanyName.MyProjectName.Shared.Localization.csproj" "shared/MyCompanyName.MyProjectName.Shared.Localization/MyCompanyName.MyProjectName.Shared.Localization.csproj"

# Microservices
COPY "services/administration/src/MyCompanyName.MyProjectName.AdministrationService.HttpApi.Host/MyCompanyName.MyProjectName.AdministrationService.HttpApi.Host.csproj" "services/administration/src/MyCompanyName.MyProjectName.AdministrationService.HttpApi.Host/MyCompanyName.MyProjectName.AdministrationService.HttpApi.Host.csproj"
COPY "services/identity/src/MyCompanyName.MyProjectName.IdentityService.HttpApi.Host/MyCompanyName.MyProjectName.IdentityService.HttpApi.Host.csproj" "services/identity/src/MyCompanyName.MyProjectName.IdentityService.HttpApi.Host/MyCompanyName.MyProjectName.IdentityService.HttpApi.Host.csproj"
COPY "services/saas/src/MyCompanyName.MyProjectName.SaasService.HttpApi.Host/MyCompanyName.MyProjectName.SaasService.HttpApi.Host.csproj" "services/saas/src/MyCompanyName.MyProjectName.SaasService.HttpApi.Host/MyCompanyName.MyProjectName.SaasService.HttpApi.Host.csproj"
COPY "services/product/src/MyCompanyName.MyProjectName.ProductService.HttpApi.Host/MyCompanyName.MyProjectName.ProductService.HttpApi.Host.csproj" "services/product/src/MyCompanyName.MyProjectName.ProductService.HttpApi.Host/MyCompanyName.MyProjectName.ProductService.HttpApi.Host.csproj"
# Gateways
COPY "gateways/web/src/MyCompanyName.MyProjectName.WebGateway/MyCompanyName.MyProjectName.WebGateway.csproj" "gateways/web/src/MyCompanyName.MyProjectName.WebGateway/MyCompanyName.MyProjectName.WebGateway.csproj"
COPY "gateways/web-public/src/MyCompanyName.MyProjectName.PublicWebGateway/MyCompanyName.MyProjectName.PublicWebGateway.csproj" "gateways/web-public/src/MyCompanyName.MyProjectName.PublicWebGateway/MyCompanyName.MyProjectName.PublicWebGateway.csproj"

COPY "NuGet.Config" "NuGet.Config"

RUN dotnet restore "MyCompanyName.MyProjectName.sln"

COPY . .
WORKDIR "/src/apps/public-web/src/MyCompanyName.MyProjectName.PublicWeb"
RUN dotnet publish -c Release -o /app
# Should be used after .net6 is out of preview for better performance
# RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MyCompanyName.MyProjectName.PublicWeb.dll"]
