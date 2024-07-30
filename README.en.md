# XAbp Framework

English | [Chinese](./README.md)

[Update description](./RELEASE.md)

<!-- ![build and test](https://img.shields.io/github/actions/workflow/status/ArcherTrister/xabp/build-and-test.yml?branch=dev&style=flat-square) -->

[![codecov](https://codecov.io/gh/ArcherTrister/xabp/branch/dev/graph/badge.svg?token=jUKLCxa6HF)](https://codecov.io/gh/ArcherTrister/xabp) [![NuGet](https://img.shields.io/nuget/v/X.Abp.Templates.svg?style=flat-square)](https://www.nuget.org/packages/X.Abp.Templates) [![GitHub Packages](https://img.shields.io/badge/style-v6.0.5%20alpha-yellow?style=flat-square&label=GitHub%20Packages)](https://www.nuget.org/packages/X.Abp.Templates) [![NuGet Download](https://img.shields.io/nuget/dt/X.Abp.Templates.svg?style=flat-square)](https://www.nuget.org/packages/X.Abp.Templates)

<!-- <a href="https://github.com/users/ArcherTrister/packages/nuget/package/X.Abp.Cli"><img src="https://github.githubassets.com/images/modules/site/packages/packages.svg" style="zoom:20%" alt="GitHub Packages"></a> -->

XAbp is a complete **infrastructure** based on **ABP Framework** to create **modern web applications** and **APIs** by following the software development **best practices** and the **latest technologies**.

### Quick Start

Install the XABP CLI:

```bash
> dotnet tool install -g X.Abp.Cli
```

Update the XABP CLI:

```bash
> dotnet tool update -g X.Abp.Cli --version 6.0.5-alpha --no-cache
```

Create a new solution:

```bash
> xabp create MyCompanyName.MyProjectName -o "D:\Project" -dbms sqlserver -cs "Server=127.0.0.1;Database=MyProjectName;User Id=sa;Password=123456"
```

-   MyCompanyName.MyProjectName
    【Project Name】(company Name + roject Name)
-   -t
    Template name
-   -o
    Specified the output directory
-   -dbms
    Specified database driver
-   -cs
    Specifies the database connection string

> See the [CLI documentation](https://github.com/ArcherTrister/xabp/blob/main/modules/X.Abp.Cli/README.md) for all available options.

::: tip
Specify the database driver requires the database connection string, if not specified, you need to manually change, the microservice template uses the SqlServer driver by default, currently not supported to specify the database driver, please manually change.
:::

### Application Modules

-   [**Account Pro**](https://commercial.abp.io/modules/Volo.Account.Pro): Login, register, forgot password, email activation, social logins and other account related functionalities.
-   [**AuditLogging Ui**](https://commercial.abp.io/modules/Volo.AuditLogging.Ui): Reporting the user audit logs and entity histories in details.
-   [**Chat**](https://commercial.abp.io/modules/Volo.Chat): Real time messaging between users.
-   [**CmsKit Pro**](https://commercial.abp.io/modules/Volo.CmsKit.Pro): Building blocks to create your own Content Management System.
-   [**File Management**](https://commercial.abp.io/modules/Volo.FileManagement): Upload, download and organize files in a hierarchical folder structure.
-   [**Forms**](https://commercial.abp.io/modules/Volo.Forms): Create forms and surveys.
-   [**Gdpr**](https://commercial.abp.io/modules/Volo.Gdpr): This module allows users to download and delete their personal data collected by the application.
-   [**Identity Pro**](https://commercial.abp.io/modules/Volo.Identity.Pro): User, role, claims and permission management.
-   [**Identityserver Pro**](https://commercial.abp.io/modules/Volo.Identityserver.Ui): Managing the identity server objects like clients, API resources, identity resources, secrets, application URLs, claims and more.
-   ~~[**Iot**](https://github.com/ArcherTrister/xabp/blob/main/modules/X.Abp.Iot/README.md): Iot services.~~
-   [**Language Management**](https://commercial.abp.io/modules/Volo.LanguageManagement): Add or remove languages and localize the application UI on the fly.
-   ~~[**LeptonTheme**](https://commercial.abp.io/modules/Volo.LeptonTheme): The official default theme of pro templates before v6.0.~~
-   ~~[**LeptonXTheme Pro**](https://commercial.abp.io/modules/Volo.Abp.LeptonTheme.Pro): Next generation of the Lepton Theme. The official theme of ABP Commercial.~~
-   [**Localization**](https://github.com/ArcherTrister/xabp/tree/main/modules/X.Abp.Localization/README.md): ABP culture localization in multiple formats.
-   [**Notification**](https://github.com/ArcherTrister/xabp/tree/main/modules/X.Abp.Notification/README.md): ABP notifications.
-   [**OpenIddict Pro**](https://commercial.abp.io/modules/Volo.OpenIddict.Pro): Managing the identity server objects like clients, API resources, identity resources, secrets, application URLs, claims and more.
-   [**Payment**](https://commercial.abp.io/modules/Volo.Payment): Provides integration for different payment gateways.
-   [**Quartz**](https://github.com/ArcherTrister/xabp/tree/main/modules/X.Abp.Quartz/README.md): Quartz Dashboard.
-   [**Saas**](https://commercial.abp.io/modules/Volo.Saas): Manage tenants, editions and features to create your multi-tenant / SaaS application.
-   [**TextTemplate Management**](https://commercial.abp.io/modules/Volo.TextTemplateManagement): Edit text/email templates on the user interface.
-   ~~[**Twilio SMS**](https://commercial.abp.io/modules/Volo.Abp.Sms.Twilio): Sends SMS over Twilio Cloud service.~~
-   [**Version Management**](https://github.com/ArcherTrister/xabp/tree/main/modules/X.Abp.VersionManagement/README.md): Managing the application version.

### Startup Templates

The Startup templates are pre-built Visual Studio solution templates. You can create your own solution based on these templates to **immediately start your development**.

[Templates](#templates)

## Support the XAbp

Love XAbp? **Please give a star** to this repository :star:

### 💖💖 If you find this framework useful, maybe you can buy me a coffee. 💖💖

Donate money by [PayPal](https://www.paypal.me/archertrister/) to my account [archertrister@outlook.com](https://www.paypal.me/archertrister/)

## Plan

### Cli

-   [ ] generate crud
-   [ ] install-module Account Pro Module
-   [x] install-module AuditLogging Module
-   [x] install-module Chat Module
-   [x] install-module CmsKit Pro Module
-   [x] install-module FileManagement Module
-   [x] install-module Forms Module
-   [ ] install-module Gdpr Module
-   [ ] install-module Identity Pro Module
-   [ ] install-module IdentityServer Pro Module
-   [x] install-module Iot Module
-   [ ] install-module LanguageManagement Module
-   [ ] install-module Localization Module
-   [x] install-module Notification Module
-   [ ] install-module OpenIddict Pro Module
-   [x] install-module Payment Module
-   [x] install-module Quartz Module
-   [x] install-module Saas Module
-   [x] install-module TextTemplateManagement Module
-   [x] install-module VersionManagement Module

### Module

-   [ ] Iot Module
-   [x] Payment Module

### Localization

-   [ ] All Modules

### External Login

-   [x] IdentityServer4

-   [ ] Openiddict

### Templates

-   [x] X.Abp.IdentityServer4
-   [x] X.Abp.Openiddict
-   [x] X.Abp.IdentityServer4.SeparatedTenantSchema
-   [x] X.Abp.Openiddict.SeparatedTenantSchema
-   [ ] X.Abp.IdentityServer4.SeparatedAuthServer
-   [ ] X.Abp.Openiddict.SeparatedAuthServer
-   [ ] X.Abp.IdentityServer4.SeparatedAuthServer.SeparatedTenantSchema
-   [ ] X.Abp.Openiddict.SeparatedAuthServer.SeparatedTenantSchema
-   [x] X.Abp.IdentityServer4.Microservices
-   [x] X.Abp.Openiddict.Microservices
-   [ ] vue
