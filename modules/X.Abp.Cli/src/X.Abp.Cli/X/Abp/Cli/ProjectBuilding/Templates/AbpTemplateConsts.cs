// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Cli.ProjectBuilding.Templates;
public class AbpTemplateConsts
{
    public const string MicroService = "xams";
    public const string MultiLayer = "xaml";
    public const string MultiLayerSeparateAuthServer = "xamlsas";
    public const string MultiLayerSeparatedTenantSchema = "xamlsts";
    public const string SingleLayer = "xasl";

    public static string[] NewTemplates =
    [
        MicroService, MultiLayer, MultiLayerSeparateAuthServer, MultiLayerSeparatedTenantSchema, SingleLayer
    ];

    public class IdentityServer
    {
        public const string Default = "xais4";
        public const string SeparatedTenantSchema = "xais4sts";
        public const string SeparatedAuthServer = "xais4sas";
        public const string SeparatedAuthServerSeparatedTenantSchema = "xais4sassts";
        public const string Microservices = "xais4m";
    }

    public class OpenIddict
    {
        public const string Default = "xaod";
        public const string SeparatedTenantSchema = "xaodsts";
        public const string SeparatedAuthServer = "xaodsas";
        public const string SeparatedAuthServerSeparatedTenantSchema = "xaodsassts";
        public const string Microservices = "xaodm";
    }
}
