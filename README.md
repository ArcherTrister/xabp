# XAbp Framework

[English](./README.en.md) | 简体中文

[更新说明](./RELEASE.md)

<!-- ![build and test](https://img.shields.io/github/actions/workflow/status/ArcherTrister/xabp/build-and-test.yml?branch=dev&style=flat-square) -->

[![codecov](https://codecov.io/gh/ArcherTrister/xabp/branch/dev/graph/badge.svg?token=jUKLCxa6HF)](https://codecov.io/gh/ArcherTrister/xabp) [![NuGet](https://img.shields.io/nuget/v/X.Abp.Templates.svg?style=flat-square)](https://www.nuget.org/packages/X.Abp.Templates) [![GitHub Packages](https://img.shields.io/badge/style-v6.0.5%20alpha-yellow?style=flat-square&label=GitHub%20Packages)](https://www.nuget.org/packages/X.Abp.Templates) [![NuGet Download](https://img.shields.io/nuget/dt/X.Abp.Templates.svg?style=flat-square)](https://www.nuget.org/packages/X.Abp.Templates)

<!-- <a href="https://github.com/users/ArcherTrister/packages/nuget/package/X.Abp.Cli"><img src="https://github.githubassets.com/images/modules/site/packages/packages.svg" style="zoom:20%" alt="GitHub Packages"></a> -->

XAbp 是一个基于**ABP Framework**框架的完整**基础架构**。通过遵循软件开发**最佳实践**和**最新技术**，创建**现代 web 应用**和**接口**。

### 快速启动项目

安装 XABP CLI:

```bash
> dotnet tool install -g X.Abp.Cli

# 安装指定版本
> dotnet tool install -g X.Abp.Cli --version x.x.x
```

更新 XABP CLI:

```bash
> dotnet tool update -g X.Abp.Cli --version x.x.x --no-cache
```

使用 cli 创建一个项目:

```bash
> xabp create MyCompanyName.MyProjectName -t xasl -tt IdentityServer4 -o "D:\Project" -dbms mysql -cs "Server=localhost;Port=3306;Database=QingBookStore;Uid=root;Pwd=123456;"
```

-   MyCompanyName.MyProjectName
    (公司名称+项目名称)【项目名称】
-   -t
    模板名称 【[模板列表](#模板)】
-   -tt
    模板类型(IdentityServer4 OpenIddict)【默认：IdentityServer4】
-   -o
    指定输出目录
-   -dbms
    指定数据库驱动(MySQL SQLServer SQLite Oracle OracleDevart PostgreSQL)【默认：MySQL】
-   -cs
    指定数据库连接字符串

> See the [CLI documentation](https://github.com/ArcherTrister/xabp/blob/main/modules/X.Abp.Cli/README.md) for all available options.

::: tip
<font style="background:#FF6600">指定数据库驱动需指定数据库连接字符串，如不指定则需手动更改，微服务模板默认使用 SqlServer 驱动，暂不支持指定数据库驱动，请手动更换。</font>
:::

### 应用程序模块

-   [**Account Pro**](https://commercial.abp.io/modules/Volo.Account.Pro): 登录，注册，忘记密码，电子邮件激活，社交登录和其他帐户相关功能.
-   [**AuditLogging Ui**](https://commercial.abp.io/modules/Volo.AuditLogging.Ui): 详细报告用户审核日志和实体历史记录.
-   [**Chat**](https://commercial.abp.io/modules/Volo.Chat): 用户之间的实时消息传递.
-   [**CmsKit Pro**](https://commercial.abp.io/modules/Volo.CmsKit.Pro): 用于创建自己的内容管理系统的构建块.
-   [**File Management**](https://commercial.abp.io/modules/Volo.FileManagement): 以分层文件夹结构上传、下载和组织文件.
-   [**Forms**](https://commercial.abp.io/modules/Volo.Forms): 创建表单和调查.
-   [**Gdpr**](https://commercial.abp.io/modules/Volo.Gdpr): 此模块允许用户下载和删除应用程序收集的个人数据.
-   [**Identity Pro**](https://commercial.abp.io/modules/Volo.Identity.Pro): 用户、角色、声明和权限管理.
-   [**Identityserver Pro**](https://commercial.abp.io/modules/Volo.Identityserver.Ui): 管理身份服务器对象，如客户端、API 资源、身份资源、机密、应用程序 URL、声明等.
-   ~~[**Iot**](https://github.com/ArcherTrister/xabp/blob/main/modules/X.Abp.Iot/README.md): 物联网服务.~~
-   [**Language Management**](https://commercial.abp.io/modules/Volo.LanguageManagement): 添加或删除语言并动态本地化应用程序 UI.
-   ~~[**LeptonTheme**](https://commercial.abp.io/modules/Volo.LeptonTheme): v6.0 之前专业模板的官方默认主题.~~
-   ~~[**LeptonXTheme Pro**](https://commercial.abp.io/modules/Volo.Abp.LeptonTheme.Pro): 下一代轻子主题。ABP 商业的官方主题.~~
-   [**Localization**](https://github.com/ArcherTrister/xabp/tree/main/modules/X.Abp.Localization/README.md): ABP 多种格式的区域性本地化.
-   [**Notification**](https://github.com/ArcherTrister/xabp/tree/main/modules/X.Abp.Notification/README.md): ABP 通知模块.
-   [**OpenIddict Pro**](https://commercial.abp.io/modules/Volo.OpenIddict.Pro): 管理身份服务器对象，如客户端、API 资源、身份资源、机密、应用程序 URL、声明等.
-   [**Payment**](https://commercial.abp.io/modules/Volo.Payment): 为不同的支付网关提供集成.
-   [**Quartz**](https://github.com/ArcherTrister/xabp/tree/main/modules/X.Abp.Quartz/README.md): Quartz 仪表板.
-   [**Saas**](https://commercial.abp.io/modules/Volo.Saas): 管理租户、版本和功能以创建 多租户/SaaS 应用程序.
-   [**TextTemplate Management**](https://commercial.abp.io/modules/Volo.TextTemplateManagement): 在用户界面上编辑文本/电子邮件模板.
-   ~~[**Twilio SMS**](https://commercial.abp.io/modules/Volo.Abp.Sms.Twilio): 通过 Twilio 云服务发送短信.~~
-   [**Version Management**](https://github.com/ArcherTrister/xabp/tree/main/modules/X.Abp.VersionManagement/README.md): 应用程序版本管理.

### 启动模板

启动模板是预构建的 Visual Studio 解决方案模板。您可以基于这些模板创建自己的解决方案，**立即开始您的开发**.

[模板列表](#模板)

## 支持 XAbp

喜欢 XAbp? 请给这个仓库一个星 :star:

### 💖💖 If you find this framework useful, maybe you can buy me a coffee. 💖💖

<p align="center">
  <img alt="pay" src="https://cdn.jsdelivr.net/gh/ArcherTrister/ArcherTrister@master/assets/pay.jpg" onerror="this.src='https://gcore.jsdelivr.net/gh/ArcherTrister/ArcherTrister@main/assets/pay.jpg'" />
</p>

## 计划

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
-   [ ] install-module LeptonTheme Module
-   [ ] install-module Localization Module
-   [x] install-module Notification Module
-   [ ] install-module OpenIddict Pro Module
-   [x] install-module Payment Module
-   [x] install-module Quartz Module
-   [x] install-module Saas Module
-   [x] install-module TextTemplateManagement Module
-   [x] install-module VersionManagement Module

### 模块

-   [ ] Iot Module
-   [x] Payment Module

### 本地化

-   [ ] All Modules

### 扩展登录

-   [x] IdentityServer4

-   [ ] Openiddict

### 模板

-   [x] ~~X.Abp.IdentityServer4(xais4)~~
-   [x] ~~X.Abp.Openiddict(xaod)~~
-   [x] X.Abp.IdentityServer4.SeparatedTenantSchema(xais4sts)
-   [x] X.Abp.Openiddict.SeparatedTenantSchema(xaodsts)
-   [ ] ~~X.Abp.IdentityServer4.SeparatedAuthServer~~
-   [ ] ~~X.Abp.Openiddict.SeparatedAuthServer~~
-   [ ] ~~X.Abp.IdentityServer4.SeparatedAuthServer.SeparatedTenantSchema~~
-   [ ] ~~X.Abp.Openiddict.SeparatedAuthServer.SeparatedTenantSchema~~
-   [x] X.Abp.IdentityServer4.Microservices(xais4mo or xais4my)
-   [x] X.Abp.Openiddict.Microservices(xaodmo or xaodmy)
-   [ ] vue

---

-   [ ] X.Abp.MicroService(xams)
-   [x] X.Abp.MultiLayer(xaml)
-   [x] X.Abp.MultiLayer.SeparateAuthServer(xamlsas)
-   [ ] X.Abp.MultiLayer.SeparatedTenantSchema(xamlsts)
-   [x] X.Abp.SingleLayer(xasl)
