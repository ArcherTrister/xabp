// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;

using Volo.Abp.Cli.ProjectModification;

namespace X.Abp.Cli.ProjectModification;
internal sealed class InstallModules
{
    // TODO: 静态模块信息
    public static List<ModuleWithMastersInfo> Init => new()
    {
        // AuditLogging
        new ModuleWithMastersInfo
        {
            Name = "X.Abp.AuditLogging",
            DisplayName = "AuditLogging",
            DocumentationLinks = "https://commercial.abp.io/modules/Volo.AuditLogging.Ui",

            // EfCoreConfigureMethodName = "X.Abp.AuditLogging.EntityFrameworkCore:ConfigureAuditLogging",
            MasterModuleInfos = new List<ModuleWithMastersInfo>(),
            NpmPackages = new List<NpmPackageInfo>(),
            NugetPackages = new List<NugetPackageInfo>
            {
                new NugetPackageInfo
                {
                    Name = "X.Abp.AuditLogging.Application",
                    ModuleClass = "X.Abp.AuditLogging.AbpAuditLoggingApplicationModule",
                    Target = NuGetPackageTarget.Application,
                    TieredTarget = NuGetPackageTarget.Application
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.AuditLogging.Application.Contracts",
                    ModuleClass = "X.Abp.AuditLogging.AbpAuditLoggingApplicationContractsModule",
                    Target = NuGetPackageTarget.ApplicationContracts,
                    TieredTarget = NuGetPackageTarget.ApplicationContracts
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.AuditLogging.HttpApi",
                    ModuleClass = "X.Abp.AuditLogging.AbpAuditLoggingHttpApiModule",
                    Target = NuGetPackageTarget.HttpApi,
                    TieredTarget = NuGetPackageTarget.HttpApi
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.AuditLogging.HttpApi.Client",
                    ModuleClass = "X.Abp.AuditLogging.AbpAuditLoggingHttpApiClientModule",
                    Target = NuGetPackageTarget.HttpApiClient,
                    TieredTarget = NuGetPackageTarget.HttpApiClient
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.AuditLogging.Web",
                    ModuleClass = "X.Abp.AuditLogging.AbpAuditLoggingWebModule",
                    Target = NuGetPackageTarget.Web,
                    TieredTarget = NuGetPackageTarget.Web
                },
            }
        },

        // BasicData
        new ModuleWithMastersInfo
        {
            Name = "X.Abp.BasicData",
            DisplayName = "BasicData",
            DocumentationLinks = "https://github.com/ArcherTrister/xabp/tree/main/modules/X.Abp.BasicData/README.md",

            EfCoreConfigureMethodName = "X.Abp.BasicData.EntityFrameworkCore:ConfigureBasicData",
            MasterModuleInfos = new List<ModuleWithMastersInfo>(),
            NpmPackages = new List<NpmPackageInfo>(),
            NugetPackages = new List<NugetPackageInfo>
            {
                new NugetPackageInfo
                {
                    Name = "X.Abp.BasicData.Application",
                    ModuleClass = "X.Abp.BasicData.AbpAuditLoggingApplicationModule",
                    Target = NuGetPackageTarget.Application,
                    TieredTarget = NuGetPackageTarget.Application
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.BasicData.Application.Contracts",
                    ModuleClass = "X.Abp.BasicData.AbpAuditLoggingApplicationContractsModule",
                    Target = NuGetPackageTarget.ApplicationContracts,
                    TieredTarget = NuGetPackageTarget.ApplicationContracts
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.BasicData.HttpApi",
                    ModuleClass = "X.Abp.BasicData.AbpAuditLoggingHttpApiModule",
                    Target = NuGetPackageTarget.HttpApi,
                    TieredTarget = NuGetPackageTarget.HttpApi
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.BasicData.HttpApi.Client",
                    ModuleClass = "X.Abp.BasicData.AbpAuditLoggingHttpApiClientModule",
                    Target = NuGetPackageTarget.HttpApiClient,
                    TieredTarget = NuGetPackageTarget.HttpApiClient
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.BasicData.Web",
                    ModuleClass = "X.Abp.BasicData.AbpAuditLoggingWebModule",
                    Target = NuGetPackageTarget.Web,
                    TieredTarget = NuGetPackageTarget.Web
                },
            }
        },

        // Chat
        new ModuleWithMastersInfo
        {
            Name = "X.Abp.Chat",
            DisplayName = "Chat",
            DocumentationLinks = "https://docs.abp.io/en/commercial/latest/modules/chat",
            EfCoreConfigureMethodName = "X.Abp.Chat.EntityFrameworkCore:ConfigureChat",
            MasterModuleInfos = new List<ModuleWithMastersInfo>(),
            NpmPackages = new List<NpmPackageInfo>(),
            NugetPackages = new List<NugetPackageInfo>
            {
                new NugetPackageInfo
                {
                    Name = "X.Abp.Chat.Application",
                    ModuleClass = "X.Abp.Chat.AbpChatApplicationModule",
                    Target = NuGetPackageTarget.Application,
                    TieredTarget = NuGetPackageTarget.Application
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Chat.Application.Contracts",
                    ModuleClass = "X.Abp.Chat.AbpChatApplicationContractsModule",
                    Target = NuGetPackageTarget.ApplicationContracts,
                    TieredTarget = NuGetPackageTarget.ApplicationContracts
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Chat.Domain",
                    ModuleClass = "X.Abp.Chat.AbpChatDomainModule",
                    Target = NuGetPackageTarget.Domain,
                    TieredTarget = NuGetPackageTarget.Domain
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Chat.Domain.Shared",
                    ModuleClass = "X.Abp.Chat.AbpChatDomainSharedModule",
                    Target = NuGetPackageTarget.DomainShared,
                    TieredTarget = NuGetPackageTarget.DomainShared
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Chat.EntityFrameworkCore",
                    ModuleClass = "X.Abp.Chat.EntityFrameworkCore.AbpChatEntityFrameworkCoreModule",
                    Target = NuGetPackageTarget.EntityFrameworkCore,
                    TieredTarget = NuGetPackageTarget.EntityFrameworkCore
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Chat.HttpApi",
                    ModuleClass = "X.Abp.Chat.AbpChatHttpApiModule",
                    Target = NuGetPackageTarget.HttpApi,
                    TieredTarget = NuGetPackageTarget.HttpApi
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Chat.HttpApi.Client",
                    ModuleClass = "X.Abp.Chat.AbpChatHttpApiClientModule",
                    Target = NuGetPackageTarget.HttpApiClient,
                    TieredTarget = NuGetPackageTarget.HttpApiClient
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Chat.SignalR",
                    ModuleClass = "X.Abp.Chat.AbpChatSignalRModule",
                    Target = NuGetPackageTarget.SignalR,
                    TieredTarget = NuGetPackageTarget.SignalR
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Chat.Web",
                    ModuleClass = "X.Abp.Chat.AbpChatWebModule",
                    Target = NuGetPackageTarget.Web,
                    TieredTarget = NuGetPackageTarget.Web
                },
            }
        },

        // CmsKit Pro
        new ModuleWithMastersInfo
         {
            Name = "X.Abp.CmsKit.Pro",
            DisplayName = "Cms Kit Pro",
            DocumentationLinks = "https://docs.abp.io/en/commercial/latest/modules/cms-kit/index",
            EfCoreConfigureMethodName = "X.Abp.CmsKit.EntityFrameworkCore:ConfigureCmsKitPro",
            MasterModuleInfos = new List<ModuleWithMastersInfo>(),
            NpmPackages = new List<NpmPackageInfo>(),
            NugetPackages = new List<NugetPackageInfo>
            {
                new NugetPackageInfo
                {
                    Name = "X.Abp.CmsKit.Pro.Application",
                    ModuleClass = "X.Abp.CmsKit.AbpCmsKitProApplicationModule",
                    Target = NuGetPackageTarget.Application,
                    TieredTarget = NuGetPackageTarget.Application
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.CmsKit.Pro.Application.Contracts",
                    ModuleClass = "X.Abp.CmsKit.AbpCmsKitProApplicationContractsModule",
                    Target = NuGetPackageTarget.ApplicationContracts,
                    TieredTarget = NuGetPackageTarget.ApplicationContracts
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.CmsKit.Pro.Domain",
                    ModuleClass = "X.Abp.CmsKit.AbpCmsKitProDomainModule",
                    Target = NuGetPackageTarget.Domain,
                    TieredTarget = NuGetPackageTarget.Domain
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.CmsKit.Pro.Domain.Shared",
                    ModuleClass = "X.Abp.CmsKit.AbpCmsKitProDomainSharedModule",
                    Target = NuGetPackageTarget.DomainShared,
                    TieredTarget = NuGetPackageTarget.DomainShared
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.CmsKit.Pro.EntityFrameworkCore",
                    ModuleClass = "X.Abp.CmsKit.EntityFrameworkCore.AbpCmsKitProEntityFrameworkCoreModule",
                    Target = NuGetPackageTarget.EntityFrameworkCore,
                    TieredTarget = NuGetPackageTarget.EntityFrameworkCore
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.CmsKit.Pro.HttpApi",
                    ModuleClass = "X.Abp.CmsKit.AbpCmsKitProHttpApiModule",
                    Target = NuGetPackageTarget.HttpApi,
                    TieredTarget = NuGetPackageTarget.HttpApi
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.CmsKit.Pro.HttpApi.Client",
                    ModuleClass = "X.Abp.CmsKit.AbpCmsKitProHttpApiClientModule",
                    Target = NuGetPackageTarget.HttpApiClient,
                    TieredTarget = NuGetPackageTarget.HttpApiClient
                },

                // new NugetPackageInfo
                // {
                //     Name = "X.Abp.CmsKit.Pro.Web",
                //     ModuleClass = "X.Abp.CmsKit.Pro.AbpCmsKitProWebModule",
                //     Target = NuGetPackageTarget.Web
                // },
            }
         },

        // FileManagement
        new ModuleWithMastersInfo
        {
            Name = "X.Abp.FileManagement",
            DisplayName = "FileManagement",
            DocumentationLinks = "https://docs.abp.io/en/commercial/latest/modules/file-management",
            EfCoreConfigureMethodName = "X.Abp.FileManagement.EntityFrameworkCore:ConfigureFileManagement",
            MasterModuleInfos = new List<ModuleWithMastersInfo>(),
            NpmPackages = new List<NpmPackageInfo>(),
            NugetPackages = new List<NugetPackageInfo>
            {
                new NugetPackageInfo
                {
                    Name = "X.Abp.FileManagement.Application",
                    ModuleClass = "X.Abp.FileManagement.AbpFileManagementApplicationModule",
                    Target = NuGetPackageTarget.Application,
                    TieredTarget = NuGetPackageTarget.Application
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.FileManagement.Application.Contracts",
                    ModuleClass = "X.Abp.FileManagement.AbpFileManagementApplicationContractsModule",
                    Target = NuGetPackageTarget.ApplicationContracts,
                    TieredTarget = NuGetPackageTarget.ApplicationContracts
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.FileManagement.Domain",
                    ModuleClass = "X.Abp.FileManagement.AbpFileManagementDomainModule",
                    Target = NuGetPackageTarget.Domain,
                    TieredTarget = NuGetPackageTarget.Domain
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.FileManagement.Domain.Shared",
                    ModuleClass = "X.Abp.FileManagement.AbpFileManagementDomainSharedModule",
                    Target = NuGetPackageTarget.DomainShared,
                    TieredTarget = NuGetPackageTarget.DomainShared
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.FileManagement.EntityFrameworkCore",
                    ModuleClass = "X.Abp.FileManagement.EntityFrameworkCore.AbpFileManagementEntityFrameworkCoreModule",
                    Target = NuGetPackageTarget.EntityFrameworkCore,
                    TieredTarget = NuGetPackageTarget.EntityFrameworkCore
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.FileManagement.HttpApi",
                    ModuleClass = "X.Abp.FileManagement.AbpFileManagementHttpApiModule",
                    Target = NuGetPackageTarget.HttpApi,
                    TieredTarget = NuGetPackageTarget.HttpApi
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.FileManagement.HttpApi.Client",
                    ModuleClass = "X.Abp.FileManagement.AbpFileManagementHttpApiClientModule",
                    Target = NuGetPackageTarget.HttpApiClient,
                    TieredTarget = NuGetPackageTarget.HttpApiClient
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.FileManagement.Web",
                    ModuleClass = "X.Abp.FileManagement.AbpFileManagementWebModule",
                    Target = NuGetPackageTarget.Web,
                    TieredTarget = NuGetPackageTarget.Web
                },
            }
        },

        // Forms
        new ModuleWithMastersInfo
        {
            Name = "X.Abp.Forms",
            DisplayName = "Forms",
            DocumentationLinks = "https://docs.abp.io/en/commercial/latest/modules/forms",
            EfCoreConfigureMethodName = "X.Abp.Forms.EntityFrameworkCore:ConfigureForms",
            MasterModuleInfos = new List<ModuleWithMastersInfo>(),
            NpmPackages = new List<NpmPackageInfo>(),
            NugetPackages = new List<NugetPackageInfo>
            {
                new NugetPackageInfo
                {
                    Name = "X.Abp.Forms.Application",
                    ModuleClass = "X.Abp.Forms.AbpFormsApplicationModule",
                    Target = NuGetPackageTarget.Application,
                    TieredTarget = NuGetPackageTarget.Application
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Forms.Application.Contracts",
                    ModuleClass = "X.Abp.Forms.AbpFormsApplicationContractsModule",
                    Target = NuGetPackageTarget.ApplicationContracts,
                    TieredTarget = NuGetPackageTarget.ApplicationContracts
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Forms.Domain",
                    ModuleClass = "X.Abp.Forms.AbpFormsDomainModule",
                    Target = NuGetPackageTarget.Domain,
                    TieredTarget = NuGetPackageTarget.Domain
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Forms.Domain.Shared",
                    ModuleClass = "X.Abp.Forms.AbpFormsDomainSharedModule",
                    Target = NuGetPackageTarget.DomainShared,
                    TieredTarget = NuGetPackageTarget.DomainShared
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Forms.EntityFrameworkCore",
                    ModuleClass = "X.Abp.Forms.EntityFrameworkCore.AbpFormsEntityFrameworkCoreModule",
                    Target = NuGetPackageTarget.EntityFrameworkCore,
                    TieredTarget = NuGetPackageTarget.EntityFrameworkCore
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Forms.HttpApi",
                    ModuleClass = "X.Abp.Forms.AbpFormsHttpApiModule",
                    Target = NuGetPackageTarget.HttpApi,
                    TieredTarget = NuGetPackageTarget.HttpApi
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Forms.HttpApi.Client",
                    ModuleClass = "X.Abp.Forms.AbpFormsHttpApiClientModule",
                    Target = NuGetPackageTarget.HttpApiClient,
                    TieredTarget = NuGetPackageTarget.HttpApiClient
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Forms.Web",
                    ModuleClass = "X.Abp.Forms.AbpFormsWebModule",
                    Target = NuGetPackageTarget.Web,
                    TieredTarget = NuGetPackageTarget.Web
                },
            }
        },

        // Iot
        new ModuleWithMastersInfo
        {
            Name = "X.Abp.Iot",
            DisplayName = "Iot",
            DocumentationLinks = "https://github.com/ArcherTrister/xabp/tree/main/modules/X.Abp.Iot/README.md",
            EfCoreConfigureMethodName = "X.Abp.Iot.EntityFrameworkCore:ConfigureIot",
            MasterModuleInfos = new List<ModuleWithMastersInfo>(),
            NpmPackages = new List<NpmPackageInfo>(),
            NugetPackages = new List<NugetPackageInfo>
            {
                new NugetPackageInfo
                {
                    Name = "X.Abp.Iot.Application",
                    ModuleClass = "X.Abp.Iot.AbpIotApplicationModule",
                    Target = NuGetPackageTarget.Application,
                    TieredTarget = NuGetPackageTarget.Application
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Iot.Application.Contracts",
                    ModuleClass = "X.Abp.Iot.AbpIotApplicationContractsModule",
                    Target = NuGetPackageTarget.ApplicationContracts,
                    TieredTarget = NuGetPackageTarget.ApplicationContracts
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Iot.Domain",
                    ModuleClass = "X.Abp.Iot.AbpIotDomainModule",
                    Target = NuGetPackageTarget.Domain,
                    TieredTarget = NuGetPackageTarget.Domain
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Iot.Domain.Shared",
                    ModuleClass = "X.Abp.Iot.AbpIotDomainSharedModule",
                    Target = NuGetPackageTarget.DomainShared,
                    TieredTarget = NuGetPackageTarget.DomainShared
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Iot.EntityFrameworkCore",
                    ModuleClass = "X.Abp.Iot.EntityFrameworkCore.AbpIotEntityFrameworkCoreModule",
                    Target = NuGetPackageTarget.EntityFrameworkCore,
                    TieredTarget = NuGetPackageTarget.EntityFrameworkCore
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Iot.HttpApi",
                    ModuleClass = "X.Abp.Iot.AbpIotHttpApiModule",
                    Target = NuGetPackageTarget.HttpApi,
                    TieredTarget = NuGetPackageTarget.HttpApi
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Iot.HttpApi.Client",
                    ModuleClass = "X.Abp.Iot.AbpIotHttpApiClientModule",
                    Target = NuGetPackageTarget.HttpApiClient,
                    TieredTarget = NuGetPackageTarget.HttpApiClient
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Iot.EmqxGrpcService",
                    ModuleClass = "X.Abp.Iot.AbpIotEmqxGrpcServiceModule",
                    Target = NuGetPackageTarget.SignalR,
                    TieredTarget = NuGetPackageTarget.SignalR
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Iot.Web",
                    ModuleClass = "X.Abp.Iot.AbpIotWebModule",
                    Target = NuGetPackageTarget.Web,
                    TieredTarget = NuGetPackageTarget.Web
                },
            }
        },

        // Notification
        new ModuleWithMastersInfo
        {
            Name = "X.Abp.Notification",
            DisplayName = "Notification",
            DocumentationLinks = "https://github.com/ArcherTrister/xabp/tree/main/modules/X.Abp.Notification/README.md",
            EfCoreConfigureMethodName = "X.Abp.Notification.EntityFrameworkCore:ConfigureNotification",
            MasterModuleInfos = new List<ModuleWithMastersInfo>(),
            NpmPackages = new List<NpmPackageInfo>(),
            NugetPackages = new List<NugetPackageInfo>
            {
                new NugetPackageInfo
                {
                    Name = "X.Abp.Notification.Application",
                    ModuleClass = "X.Abp.Notification.AbpNotificationApplicationModule",
                    Target = NuGetPackageTarget.Application,
                    TieredTarget = NuGetPackageTarget.Application
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Notification.Application.Contracts",
                    ModuleClass = "X.Abp.Notification.AbpNotificationApplicationContractsModule",
                    Target = NuGetPackageTarget.ApplicationContracts,
                    TieredTarget = NuGetPackageTarget.ApplicationContracts
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Notification.Domain",
                    ModuleClass = "X.Abp.Notification.AbpNotificationDomainModule",
                    Target = NuGetPackageTarget.Domain,
                    TieredTarget = NuGetPackageTarget.Domain
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Notification.Domain.Shared",
                    ModuleClass = "X.Abp.Notification.AbpNotificationDomainSharedModule",
                    Target = NuGetPackageTarget.DomainShared,
                    TieredTarget = NuGetPackageTarget.DomainShared
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Notification.EntityFrameworkCore",
                    ModuleClass = "X.Abp.Notification.EntityFrameworkCore.AbpNotificationEntityFrameworkCoreModule",
                    Target = NuGetPackageTarget.EntityFrameworkCore,
                    TieredTarget = NuGetPackageTarget.EntityFrameworkCore
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Notification.HttpApi",
                    ModuleClass = "X.Abp.Notification.AbpNotificationHttpApiModule",
                    Target = NuGetPackageTarget.HttpApi,
                    TieredTarget = NuGetPackageTarget.HttpApi
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Notification.HttpApi.Client",
                    ModuleClass = "X.Abp.Notification.AbpNotificationHttpApiClientModule",
                    Target = NuGetPackageTarget.HttpApiClient,
                    TieredTarget = NuGetPackageTarget.HttpApiClient
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Notification.Web",
                    ModuleClass = "X.Abp.Notification.AbpNotificationWebModule",
                    Target = NuGetPackageTarget.Web,
                    TieredTarget = NuGetPackageTarget.Web
                },
            }
        },

        // Payment
        new ModuleWithMastersInfo
        {
            Name = "X.Abp.Payment",
            DisplayName = "Payment",
            DocumentationLinks = "https://docs.abp.io/en/commercial/latest/modules/payment",
            EfCoreConfigureMethodName = "X.Abp.Payment.EntityFrameworkCore:ConfigurePayment",
            MasterModuleInfos = new List<ModuleWithMastersInfo>(),
            NpmPackages = new List<NpmPackageInfo>(),
            NugetPackages = new List<NugetPackageInfo>
            {
                new NugetPackageInfo
                {
                    Name = "X.Abp.Payment.Admin.Application",
                    ModuleClass = "X.Abp.Payment.Admin.AbpPaymentAdminApplicationModule",
                    Target = NuGetPackageTarget.Application,
                    TieredTarget = NuGetPackageTarget.Application
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Payment.Admin.Application.Contracts",
                    ModuleClass = "X.Abp.Payment.Admin.AbpPaymentAdminApplicationContractsModule",
                    Target = NuGetPackageTarget.ApplicationContracts,
                    TieredTarget = NuGetPackageTarget.ApplicationContracts
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Payment.Admin.HttpApi",
                    ModuleClass = "X.Abp.Payment.Admin.AbpPaymentAdminHttpApiModule",
                    Target = NuGetPackageTarget.HttpApi,
                    TieredTarget = NuGetPackageTarget.HttpApi
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Payment.Admin.HttpApi.Client",
                    ModuleClass = "X.Abp.Payment.Admin.AbpPaymentAdminHttpApiClientModule",
                    Target = NuGetPackageTarget.HttpApiClient,
                    TieredTarget = NuGetPackageTarget.HttpApiClient
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Payment.Admin.Web",
                    ModuleClass = "X.Abp.Payment.Admin.AbpPaymentAdminWebModule",
                    Target = NuGetPackageTarget.Web,
                    TieredTarget = NuGetPackageTarget.Web
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Payment.Application",
                    ModuleClass = "X.Abp.Payment.AbpPaymentApplicationModule",
                    Target = NuGetPackageTarget.Application,
                    TieredTarget = NuGetPackageTarget.Application
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Payment.Application.Contracts",
                    ModuleClass = "X.Abp.Payment.AbpPaymentApplicationContractsModule",
                    Target = NuGetPackageTarget.ApplicationContracts,
                    TieredTarget = NuGetPackageTarget.ApplicationContracts
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Payment.Domain",
                    ModuleClass = "X.Abp.Payment.AbpPaymentDomainModule",
                    Target = NuGetPackageTarget.Domain,
                    TieredTarget = NuGetPackageTarget.Domain
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Payment.Domain.Shared",
                    ModuleClass = "X.Abp.Payment.AbpPaymentDomainSharedModule",
                    Target = NuGetPackageTarget.DomainShared,
                    TieredTarget = NuGetPackageTarget.DomainShared
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Payment.EntityFrameworkCore",
                    ModuleClass = "X.Abp.Payment.EntityFrameworkCore.AbpPaymentEntityFrameworkCoreModule",
                    Target = NuGetPackageTarget.EntityFrameworkCore,
                    TieredTarget = NuGetPackageTarget.EntityFrameworkCore
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Payment.HttpApi",
                    ModuleClass = "X.Abp.Payment.AbpPaymentHttpApiModule",
                    Target = NuGetPackageTarget.HttpApi,
                    TieredTarget = NuGetPackageTarget.HttpApi
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Payment.HttpApi.Client",
                    ModuleClass = "X.Abp.Payment.AbpPaymentHttpApiClientModule",
                    Target = NuGetPackageTarget.HttpApiClient,
                    TieredTarget = NuGetPackageTarget.HttpApiClient
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Payment.Web",
                    ModuleClass = "X.Abp.Payment.AbpPaymentWebModule",
                    Target = NuGetPackageTarget.Web,
                    TieredTarget = NuGetPackageTarget.Web
                },
            }
        },

        // Quartz
        new ModuleWithMastersInfo
        {
            Name = "X.Abp.Quartz",
            DisplayName = "Quartz",
            DocumentationLinks = "https://github.com/ArcherTrister/xabp/tree/main/modules/X.Abp.Quartz/README.md",
            EfCoreConfigureMethodName = null,
            MasterModuleInfos = new List<ModuleWithMastersInfo>(),
            NpmPackages = new List<NpmPackageInfo>(),
            NugetPackages = new List<NugetPackageInfo>
            {
                new NugetPackageInfo
                {
                    Name = "X.Abp.Quartz.Application",
                    ModuleClass = "X.Abp.Quartz.AbpQuartzApplicationModule",
                    Target = NuGetPackageTarget.Application,
                    TieredTarget = NuGetPackageTarget.Application
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Quartz.Application.Contracts",
                    ModuleClass = "X.Abp.Quartz.AbpQuartzApplicationContractsModule",
                    Target = NuGetPackageTarget.ApplicationContracts,
                    TieredTarget = NuGetPackageTarget.ApplicationContracts
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Quartz.Domain",
                    ModuleClass = "X.Abp.Quartz.AbpQuartzDomainModule",
                    Target = NuGetPackageTarget.Domain
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Quartz.Domain.Shared",
                    ModuleClass = "X.Abp.Quartz.AbpQuartzDomainSharedModule",
                    Target = NuGetPackageTarget.DomainShared
                },

                // new NugetPackageInfo
                // {
                //     Name = "X.Abp.Quartz.EntityFrameworkCore",
                //     ModuleClass = "X.Abp.Quartz.EntityFrameworkCore.AbpQuartzEntityFrameworkCoreModule",
                //     Target = NuGetPackageTarget.EntityFrameworkCore
                // },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Quartz.HttpApi",
                    ModuleClass = "X.Abp.Quartz.AbpQuartzHttpApiModule",
                    Target = NuGetPackageTarget.HttpApi,
                    TieredTarget = NuGetPackageTarget.HttpApi
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Quartz.HttpApi.Client",
                    ModuleClass = "X.Abp.Quartz.AbpQuartzHttpApiClientModule",
                    Target = NuGetPackageTarget.HttpApiClient,
                    TieredTarget = NuGetPackageTarget.HttpApiClient
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Quartz.Web",
                    ModuleClass = "X.Abp.Quartz.AbpQuartzWebModule",
                    Target = NuGetPackageTarget.Web,
                    TieredTarget = NuGetPackageTarget.Web
                },
            }
        },

        // Saas
        new ModuleWithMastersInfo
        {
            Name = "X.Abp.Saas",
            DisplayName = "Saas",
            DocumentationLinks = "https://docs.abp.io/en/commercial/latest/modules/saas",
            EfCoreConfigureMethodName = "X.Abp.Saas.EntityFrameworkCore:ConfigureSaas",
            MasterModuleInfos = new List<ModuleWithMastersInfo>(),
            NpmPackages = new List<NpmPackageInfo>(),
            NugetPackages = new List<NugetPackageInfo>
            {
                new NugetPackageInfo
                {
                    Name = "X.Abp.Saas.Application",
                    ModuleClass = "X.Abp.Saas.AbpSaasApplicationModule",
                    Target = NuGetPackageTarget.Application,
                    TieredTarget = NuGetPackageTarget.Application
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Saas.Application.Contracts",
                    ModuleClass = "X.Abp.Saas.AbpSaasApplicationContractsModule",
                    Target = NuGetPackageTarget.ApplicationContracts,
                    TieredTarget = NuGetPackageTarget.ApplicationContracts
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Saas.Domain",
                    ModuleClass = "X.Abp.Saas.AbpSaasDomainModule",
                    Target = NuGetPackageTarget.Domain,
                    TieredTarget = NuGetPackageTarget.Domain
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Saas.Domain.Shared",
                    ModuleClass = "X.Abp.Saas.AbpSaasDomainSharedModule",
                    Target = NuGetPackageTarget.DomainShared,
                    TieredTarget = NuGetPackageTarget.DomainShared
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Saas.EntityFrameworkCore",
                    ModuleClass = "X.Abp.Saas.EntityFrameworkCore.AbpSaasEntityFrameworkCoreModule",
                    Target = NuGetPackageTarget.EntityFrameworkCore,
                    TieredTarget = NuGetPackageTarget.EntityFrameworkCore
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Saas.HttpApi",
                    ModuleClass = "X.Abp.Saas.AbpSaasHttpApiModule",
                    Target = NuGetPackageTarget.HttpApi,
                    TieredTarget = NuGetPackageTarget.HttpApi
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Saas.HttpApi.Client",
                    ModuleClass = "X.Abp.Saas.AbpSaasHttpApiClientModule",
                    Target = NuGetPackageTarget.HttpApiClient,
                    TieredTarget = NuGetPackageTarget.HttpApiClient
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.Saas.Web",
                    ModuleClass = "X.Abp.Saas.AbpSaasWebModule",
                    Target = NuGetPackageTarget.Web,
                    TieredTarget = NuGetPackageTarget.Web
                },
            }
        },

        // TextTemplateManagement
        new ModuleWithMastersInfo
        {
            Name = "X.Abp.TextTemplateManagement",
            DisplayName = "TextTemplateManagement",
            DocumentationLinks = "https://docs.abp.io/en/commercial/latest/modules/text-template-management",
            EfCoreConfigureMethodName = "X.Abp.TextTemplateManagement.EntityFrameworkCore:ConfigureTextTemplateManagement",
            MasterModuleInfos = new List<ModuleWithMastersInfo>(),
            NpmPackages = new List<NpmPackageInfo>(),
            NugetPackages = new List<NugetPackageInfo>
            {
                new NugetPackageInfo
                {
                    Name = "X.Abp.TextTemplateManagement.Application",
                    ModuleClass = "X.Abp.TextTemplateManagement.AbpTextTemplateManagementApplicationModule",
                    Target = NuGetPackageTarget.Application,
                    TieredTarget = NuGetPackageTarget.Application
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.TextTemplateManagement.Application.Contracts",
                    ModuleClass = "X.Abp.TextTemplateManagement.AbpTextTemplateManagementApplicationContractsModule",
                    Target = NuGetPackageTarget.ApplicationContracts,
                    TieredTarget = NuGetPackageTarget.ApplicationContracts
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.TextTemplateManagement.Domain",
                    ModuleClass = "X.Abp.TextTemplateManagement.AbpTextTemplateManagementDomainModule",
                    Target = NuGetPackageTarget.Domain,
                    TieredTarget = NuGetPackageTarget.Domain
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.TextTemplateManagement.Domain.Shared",
                    ModuleClass = "X.Abp.TextTemplateManagement.AbpTextTemplateManagementDomainSharedModule",
                    Target = NuGetPackageTarget.DomainShared,
                    TieredTarget = NuGetPackageTarget.DomainShared
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.TextTemplateManagement.EntityFrameworkCore",
                    ModuleClass = "X.Abp.TextTemplateManagement.EntityFrameworkCore.AbpTextTemplateManagementEntityFrameworkCoreModule",
                    Target = NuGetPackageTarget.EntityFrameworkCore,
                    TieredTarget = NuGetPackageTarget.EntityFrameworkCore
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.TextTemplateManagement.HttpApi",
                    ModuleClass = "X.Abp.TextTemplateManagement.AbpTextTemplateManagementHttpApiModule",
                    Target = NuGetPackageTarget.HttpApi,
                    TieredTarget = NuGetPackageTarget.HttpApi
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.TextTemplateManagement.HttpApi.Client",
                    ModuleClass = "X.Abp.TextTemplateManagement.AbpTextTemplateManagementHttpApiClientModule",
                    Target = NuGetPackageTarget.HttpApiClient,
                    TieredTarget = NuGetPackageTarget.HttpApiClient
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.TextTemplateManagement.Web",
                    ModuleClass = "X.Abp.TextTemplateManagement.AbpTextTemplateManagementWebModule",
                    Target = NuGetPackageTarget.Web
                },
            }
        },

        // VersionManagement
        new ModuleWithMastersInfo
        {
            Name = "X.Abp.VersionManagement",
            DisplayName = "VersionManagement",
            DocumentationLinks = "https://github.com/ArcherTrister/xabp/tree/main/modules/X.Abp.VersionManagement/README.md",
            EfCoreConfigureMethodName = "X.Abp.VersionManagement.EntityFrameworkCore:ConfigureVersionManagement",
            MasterModuleInfos = new List<ModuleWithMastersInfo>(),
            NpmPackages = new List<NpmPackageInfo>(),
            NugetPackages = new List<NugetPackageInfo>
            {
                new NugetPackageInfo
                {
                    Name = "X.Abp.VersionManagement.Application",
                    ModuleClass = "X.Abp.VersionManagement.AbpVersionManagementApplicationModule",
                    Target = NuGetPackageTarget.Application,
                    TieredTarget = NuGetPackageTarget.Application
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.VersionManagement.Application.Contracts",
                    ModuleClass = "X.Abp.VersionManagement.AbpVersionManagementApplicationContractsModule",
                    Target = NuGetPackageTarget.ApplicationContracts,
                    TieredTarget = NuGetPackageTarget.ApplicationContracts
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.VersionManagement.Domain",
                    ModuleClass = "X.Abp.VersionManagement.AbpVersionManagementDomainModule",
                    Target = NuGetPackageTarget.Domain,
                    TieredTarget = NuGetPackageTarget.Domain
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.VersionManagement.Domain.Shared",
                    ModuleClass = "X.Abp.VersionManagement.AbpVersionManagementDomainSharedModule",
                    Target = NuGetPackageTarget.DomainShared,
                    TieredTarget = NuGetPackageTarget.DomainShared
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.VersionManagement.EntityFrameworkCore",
                    ModuleClass = "X.Abp.VersionManagement.EntityFrameworkCore.AbpVersionManagementEntityFrameworkCoreModule",
                    Target = NuGetPackageTarget.EntityFrameworkCore,
                    TieredTarget = NuGetPackageTarget.EntityFrameworkCore
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.VersionManagement.HttpApi",
                    ModuleClass = "X.Abp.VersionManagement.AbpVersionManagementHttpApiModule",
                    Target = NuGetPackageTarget.HttpApi,
                    TieredTarget = NuGetPackageTarget.HttpApi
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.VersionManagement.HttpApi.Client",
                    ModuleClass = "X.Abp.VersionManagement.AbpVersionManagementHttpApiClientModule",
                    Target = NuGetPackageTarget.HttpApiClient,
                    TieredTarget = NuGetPackageTarget.HttpApiClient
                },
                new NugetPackageInfo
                {
                    Name = "X.Abp.VersionManagement.Web",
                    ModuleClass = "X.Abp.VersionManagement.AbpVersionManagementWebModule",
                    Target = NuGetPackageTarget.Web,
                    TieredTarget = NuGetPackageTarget.Web
                },
            }
        },
    };
}
