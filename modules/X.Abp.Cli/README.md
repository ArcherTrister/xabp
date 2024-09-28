# XABP CLI

XABP CLI (命令行接口) 是一个命令行工具,用来执行基于 XABP 解决方案的一些常见操作.

## Installation

XABP CLI 是一个 [dotnet global tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools). 使用命令行窗口安装:

```bash
dotnet tool install -g X.Abp.Cli
dotnet tool install -g X.Abp.Cli --add-source ./
```

更新最新版本:

```bash
dotnet tool update -g X.Abp.Cli
dotnet tool update -g X.Abp.Cli --add-source ./
```

## 全局选项

虽然每个命令可能都有一组选项,但有些全局选项可以与任何命令一起使用:

-   `--skip-cli-version-check`: 跳过检查最新版本的 XABP CLI. 如果没有指定,它会检查最新版本,如果检查到 XABP CLI 的新版本,会显示一条警告消息.

## Commands

这里是所有可用的命令列表:

-   **`help`**: 展示 XABP CLI 的用法帮助信息.
-   ~~**`cli`**: 更新或删除 XABP CLI.~~
-   **`prompt`**: tarts with prompt mode.
-   **`new`**：生成基于 XABP 的[启动模板](https://abp.io/docs/zh-Hans/abp/latest/Startup-Templates/Index).
-   **`update`**：自动更新的 XABP 解决方案 XABP 相关的 NuGet 和 NPM 包.
-   **`clean`**: 删除当前目录下所有的 `BIN` 和 `OBJ` 子目录.
-   ~~**`add-package`**: 添加 XABP 包到项目.~~
-   ~~**`add-module`**: 添加[应用模块](https://docs.abp.io/en/abp/latest/Modules/Index)到解决方案.~~
-   **`generate-proxy`**: 生成客户端代理以使用 HTTP API 端点.
-   **`remove-proxy`**: 移除以前生成的客户端代理.
-   ~~**`switch-to-preview`**: 切换到 XABP 框架的最新预览版本。~~
-   ~~**`switch-to-nightly`**: 切换解决方案所有 XABP 相关包为[夜间构建](https://abp.io/docs/en/abp/latest/Nightly-Builds)版本.~~
-   ~~**`switch-to-stable`**: 切换解决方案所有 XABP 相关包为最新的稳定版本.~~
-   **`translate`**: 当源代码控制存储库中有多个 JSON[本地化](https://abp.io/docs/lastest/Localization)文件时,可简化翻译本地化文件的过程.
-   **`create-migration-and-run-migrator`**: 创建并运行迁移.
-   ~~**`login`**: 使用你在[abp.io](https://abp.io/)的用户名和密码在你的计算机上认证.~~
-   ~~**`login-info`**: 展示当前登录用户信息.~~
-   ~~**`logout`**: 在你的计算机注销认证.~~
-   ~~**`bundle`**: 为 XABP Blazor 和 MAUI Blazor 项目生成引用的脚本和样式.~~
-   **`install-libs`**: 为 MVC / Razor Pages 和 Blazor Server UI 类型安装 NPM 包.
-   **`create`**: 基于 **xabp** 启动模板生成新的解决方案.
-   **`generate-cert`**: 生成证书文件.
-   **`install-module`**: 添加[应用模块](https://github.com/ArcherTrister/xabp#%E5%BA%94%E7%94%A8%E7%A8%8B%E5%BA%8F%E6%A8%A1%E5%9D%97)到解决方案.

### help

展示 XABP CLI 的基本用法:

用法:

```bash
xabp help [command-name]
```

示例:

```bash
xabp help        # Shows a general help.
xabp help new    # Shows help about the "new" command.
```

### cli

更新或删除 XABP CLI

用法:

```bash
xabp cli [command-name]
```

示例:

```bash
xabp cli update
xabp cli update --preview
xabp cli update --version 5.0.0
xabp cli remove
```

### new

生成基于 XABP[启动模板](https://github.com/ArcherTrister/xabp#%E5%90%AF%E5%8A%A8%E6%A8%A1%E6%9D%BF)的新解决方案.

用法:

```bash
xabp new <解决方案名称> [options]
```

示例:

```bash
xabp new Acme.BookStore
```

-   Acme.BookStore 是解决方案的名称.
-   常见的命名方式类似于 _YourCompany.YourProject_. 不过你可以使用自己喜欢的方式,如 _YourProject_ (单级命名空间) 或 _YourCompany.YourProduct.YourModule_ (三级命名空间).

参阅[XABP CLI 创建新解决方案示例](https://abp.io/docs/latest/cli/new-command-samples)查看更多示例.

#### Options

-   `--template` 或者 `-t`: 指定模板. 默认的模板是 `app`,会生成 web 项目.可用的模板有:
    -   `app` (**default**): [应用程序模板](https://abp.io/docs/zh-Hans/abp/latest/Startup-Templates/Application). 其他选项:
        -   `--ui` 或者 `-u`: 指定 ui 框架.默认`mvc`框架.其他选项:
            -   `mvc`: ASP.NET Core MVC.此模板的其他选项:
                -   `--tiered`: 创建分层解决方案,Web 和 Http Api 层在物理上是分开的.如果未指定会创建一个分层的解决方案,此解决方案没有那么复杂,适合大多数场景.
            -   `angular`: Angular. 这个模板还有一些额外的选项:
                -   `--separate-auth-server`: 将 Identity Server 应用程序与 API host 应用程序分开. 如果未指定,则服务器端将只有一个端点.
            -   `blazor`: Blazor. 这个模板还有一些额外的选项:
                -   `--separate-auth-server`: 将 Identity Server 应用程序与 API host 应用程序分开. 如果未指定,则服务器端将只有一个端点.
            -   `none`: 无 UI. 这个模板还有一些额外的选项:
                -   `--separate-auth-server`: 将 Identity Server 应用程序与 API host 应用程序分开. 如果未指定,则服务器端将只有一个端点.
        -   `--mobile` 或者 `-m`: 指定移动应用程序框架. 如果未指定,则不会创建任何移动应用程序,其他选项:
            -   `none`: 不包含移动应用程序.
            -   `react-native`: React Native.
        -   `--database-provider` 或者 `-d`: 指定数据库提供程序.默认是 `ef`.其他选项:
            -   `ef`: Entity Framework Core.
            -   `mongodb`: MongoDB.
    -   `module`: [Module template](https://abp.io/docs/zh-Hans/abp/latest/Startup-Templates/Module). 其他选项:
        -   `--no-ui`: 不包含 UI.仅创建服务模块(也称为微服务 - 没有 UI).
    -   **`console`**: [Console template](https://abp.io/docs/zh-Hans/abp/latest/Startup-Templates/Console).
    -   **`maui`**: [Maui template](https://abp.io/docs/zh-Hans/abp/latest/Startup-Templates/MAUI).
    -   **`app-nolayers`**: 应用程序单层模板
    -   `--ui` 或者 `-u`: 指定 ui 框架.默认`mvc`框架.其他选项:
        -   `mvc`: ASP.NET Core MVC.
        -   `angular`: Angular.
        -   `blazor`: Blazor UI.
        -   `blazor-server`: Blazor Server.
        -   `none`: 不包含 UI.
    -   `--database-provider` 或 `-d`: 或者 `-d`: 指定数据库提供程序.默认是 `ef`.其他选项:
        -   `ef`: Entity Framework Core.
        -   `mongodb`: MongoDB.
-   `--output-folder` 或者 `-o`: 指定输出文件夹,默认是当前目录.
-   `--version` 或者 `-v`: 指定 XABP 和模板的版本.它可以是 [release tag](https://github.com/ArcherTrister/xabp/releases) 或者 [branch name](https://github.com/ArcherTrister/xabp/branches). 如果没有指定,则使用最新版本.大多数情况下,你会希望使用最新的版本.
-   `--preview`: 使用最新的预览版本.
-   `--template-source` 或者 `-ts`: 指定自定义模板源用于生成项目,可以使用本地源和网络源(例如 `D:\local-templat` 或 `https://.../my-template-file.zip`).
-   `--create-solution-folder` 或者 `-csf`: 指定项目是在输出文件夹中的新文件夹中还是直接在输出文件夹中.
-   `--connection-string` 或者 `-cs`: 重写所有 `appsettings.json` 文件的默认连接字符串. 默认连接字符串是 `Server=localhost;Database=MyProjectName;Trusted_Connection=True`. 默认的数据库提供程序是 `SQL Server`. 如果你使用 EF Core 但需要更改 DBMS,可以按[这里所述](https://abp.io/docs/zh-Hans/abp/latest/Entity-Framework-Core-Other-DBMS)进行更改(创建解决方案之后).
-   `--local-framework-ref --abp-path`: 使用对项目的本地引用,而不是替换为 NuGet 包引用.

### update

更新所有 XABP 相关的包可能会很繁琐,框架和模块都有很多包. 此命令自动将解决方案或项目中所有 XABP 相关的包更新到最新版本.

用法:

```bash
xabp update [options]
```

-   如果你的文件夹中有.sln 文件,运行命令会将解决方案中所有项目 XABP 相关的包更新到最新版本.
-   如果你的文件夹中有.csproj 文件,运行命令会将项目中所有 XABP 相关的包更新到最新版本.

#### Options

-   `--npm`: 仅更新 NPM 包
-   `--nuget`: 仅更新的 NuGet 包
-   `--solution-path` 或 `-sp`: 指定解决方案路径/目录. 默认使用当前目录
-   `--solution-name` 或 `-sn`: 指定解决方案名称. 默认在目录中搜索`*.sln`文件.
-   `--check-all`: 分别检查每个包的新版本. 默认是 `false`.
-   `--version` or `-v`: 指定用于升级的版本. 如果没有指定,则使用最新版本.

### clean

删除当前目录下所有的 `BIN` 和 `OBJ` 子目录.

用法:

```bash
xabp clean
```

### add-package

通过以下方式将 XABP 包添加到项目中

-   添加 nuget 包做为项目的依赖项目.
-   添加 `[DependsOn(...)]` attribute 到项目的模块类 (请参阅 [模块开发文档](https://abp.io/docs/zh-Hans/abp/latest/Module-Development-Basics)).

> 需要注意的是添加的模块可能需要额外的配置,通常会在包的文档中指出.

用法:

```bash
xabp add-package <包名> [options]
```

示例:

```
xabp add-package Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic
```

-   示例中将`Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic`包添加到项目中.

#### Options

-   `--project` 或 `-p`: 指定项目 (.csproj) 路径. 如果未指定,Cli 会尝试在当前目录查找.csproj 文件.
-   `--with-source-code`: 下载包的源码到你的解决方案文件夹，而不是 NuGet/NPM 软件包.
-   `--add-to-solution-file`: 添加下载/创建的包添加到解决方案文件中,你在 IDE 中打开解决方案时也会看到包的项目. (仅当 `--with-source-code` 为 `True` 时可用.)

> 当前只有基本主题包([MVC](https://docs.abp.io/zh-Hans/abp/latest/UI/AspNetCore/Basic-Theme) 和 [Blazor](https://docs.abp.io/zh-Hans/abp/latest/UI/Blazor/Basic-Theme)) 可以下载.
>
> -   Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic
> -   Volo.Abp.AspNetCore.Components.WebAssembly.BasicTheme
> -   Volo.Abp.AspNetCore.Components.Web.BasicTheme
> -   Volo.Abp.AspNetCore.Components.Server.BasicTheme

### add-module

通过查找模块的所有包,查找解决方案中的相关项目,并将每个包添加到解决方案中的相应项目,从而将[多包应用程序模块](Modules/Index)添加到解决方案中.

> 由于分层,不同的数据库提供程序选项或其他原因,业务模块通常由多个包组成. 使用`add-module`命令可以大大简化向模块添加模块的过程. 但是每个模块可能需要一些其他配置,这些配置通常在相关模块的文档中指出.

用法:

```bash
xabp add-module <模块名称> [options]
```

示例:

```bash
xabp add-module Volo.Blogging
```

-   示例中将 Volo.Blogging 模块添加到解决方案中.

#### Options

-   `--solution` 或 `-s`: 指定解决方案 (.sln) 路径. 如果未指定,CLI 会尝试在当前目录中寻找.sln 文件.
-   `--skip-db-migrations`: 对于 EF Core 数据库提供程序,它会自动添加新代码的第一次迁移 (`Add-Migration`) 并且在需要时更新数据库 (`Update-Database`). 指定此选项可跳过此操作.
-   `-sp` 或 `--startup-project`: 启动项目的项目文件夹的相对路径. 默认值是当前文件夹.
-   `--with-source-code`: 添加模块的源代码,而不是 NuGet/NPM 软件包.
-   `--add-to-solution-file`: 添加下载/创建的模块添加到解决方案文件中,你在 IDE 中打开解决方案时也会看到模块的项目. (仅当 `--with-source-code` 为 `True` 时可用.)

### generate-proxy

为你的 HTTP API 生成 Angular, C# 或 JavaScript 服务代理,简化从客户端使用服务的成本. 在运行此命令之前,你的 host 必须启动正在运行.

用法:

```bash
xabp generate-proxy -t <client-type> [options]
```

示例:

```bash
xabp generate-proxy -t ng
xabp generate-proxy -t js -url https://localhost:44302/
xabp generate-proxy -t csharp -url https://localhost:44302/
```

#### Options

-   `--type` 或 `-t`: 客户端类型的名称. 可用的客户端有:
    -   `csharp`: C#, 工作在 `*.HttpApi.Client` 项目目录. 此客户端有一些可选选项:
        -   `--without-contracts`: 取消生成应用程序服务接口,类,枚举和 DTO.
        -   `--folder`: 放置生成的 CSharp 代码的文件夹名称. 默认值: `ClientProxies`.
    -   `ng`: Angular. 此客户端有一些可选选项:
        -   `--api-name` 或 `-a`: 在 `/src/environments/environment.ts` 中定义的 API 端点名称。. 默认值: `default`.
        -   `--source` 或 `-s`: 指定解析根名称空间和 API 定义 URL 的 Angular 项目名称. 默认值: `defaultProject`
        -   `--target` 或 `-t`: 指定放置生成的代码的 Angular 项目名称. 默认值: `defaultProject`.
        -   `--prompt` 或 `-p`: 在命令行提示符下询问选项(未指定的选项).
    -   `js`: JavaScript. 工作在 `*.Web` 项目目录. 此客户端有一些可选选项:
        -   `--output` or `-o`: 放置生成的 JavaScript 代码的文件夹名称.
-   `--module` 或 `-m`: 指定要为其生成代理的后端模块的名称. 默认值: `app`.
-   `--working-directory` or `-wd`: 执行目录. 用于 `csharp` 和 `js` 客户端类型.
-   `--url` or `-u`: API 定义的 URL. 用于 `csharp` 和 `js` 客户端类型.

> 参阅 [Angular 服务代理文档](https://abp.io/docs/zh-Hans/abp/latest/UI/Angular/Service-Proxies) 了解更多.

### remove-proxy

从 Angular, CSharp 或 JavaScript 应用程序中删除以前生成的代理代码. 在运行此命令之前,你的 host 必须启动正在运行.

这在你之前为多个模块生成代理并且需要删除其中一个模块时特别有用.

用法:

```bash
xabp remove-proxy -t <client-type> [options]
```

示例:

```bash
xabp remove-proxy -t ng
xabp remove-proxy -t js -m identity -o Pages/Identity/client-proxies.js
xabp remove-proxy -t csharp --folder MyProxies/InnerFolder
```

#### Options

-   `--type` 或 `-t`: 客户端类型的名称. 可用的客户端有:
    -   `csharp`: C#, 工作在 `*.HttpApi.Client` 项目目录. 此客户端有一些可选选项:
        -   `--folder`: 放置生成的 CSharp 代码的文件夹名称. 默认值: `ClientProxies`.
    -   `ng`: Angular. 此客户端有一些可选选项:
        -   `--api-name` 或 `-a`: 在 `/src/environments/environment.ts` 中定义的 API 端点名称。. 默认值: `default`.
        -   `--source` 或 `-s`: 指定解析根名称空间和 API 定义 URL 的 Angular 项目名称. 默认值: `defaultProject`
        -   `--target` 或 `-t`: 指定放置生成的代码的 Angular 项目名称. 默认值: `defaultProject`.
        -   `--prompt` 或 `-p`: 在命令行提示符下询问选项(未指定的选项).
    -   `js`: JavaScript. 工作在 `*.Web` 项目目录. 此客户端有一些可选选项:
        -   `--output` or `-o`: 放置生成的 JavaScript 代码的文件夹名称.
-   `--module` 或 `-m`: 指定要为其生成代理的后端模块的名称. 默认值: `app`.
-   `--working-directory` or `-wd`: 执行目录. 用于 `csharp` 和 `js` 客户端类型.
-   `--url` or `-u`: API 定义的 URL. 用于 `csharp` 和 `js` 客户端类型.

> 参阅 [Angular 服务代理文档](https://abp.io/docs/en/abp/latest/UI/Angular/Service-Proxies) 了解更多.

### switch-to-preview

你可以使用此命令将项目切换到 XABP 框架的最新预览版本.

用法:

```bash
xabp switch-to-preview [options]
```

#### Options

-   `--solution-directory` 或 `-sd`: 指定目录. 解决方案应该在该目录或其子目录中. 如果未指定默认为当前目录.

### switch-to-nightly

想要切换到 XABP 框架的最新[每晚构建](https://abp.io/docs/en/abp/latest/Nightly-Builds)预览版可以使用此命令.

用法:

```bash
xabp switch-to-nightly [options]
```

#### Options

`--solution-directory` 或 `-sd`: 指定目录. 解决方案应该在该目录或其子目录中. 如果未指定默认为当前目录.

### switch-to-stable

如果你使用的是 XABP 框架预览包(包括每晚构建),可以使用此命令切换回最新的稳定版本.

用法:

```bash
xabp switch-to-stable [options]
```

#### Options

`--solution-directory` 或 `-sd`: 指定目录. 解决方案应该在该目录或其子目录中. 如果未指定默认为当前目录.

### translate

源代码控制存储库中有多个 JSON[本地化](https://abp.io/docs/lastest/Localization)文件时,用于简化翻译[本地化](https://abp.io/docs/lastest/Localization)文件的过程.

-   该命令将基于参考文化创建一个统一的 json 文件
-   它搜索当前目录和所有子目录中的所有本地化"JSON"文件(递归). 然后创建一个包含所有需要翻译的条目的文件(默认情况下名为 "abp-translation.json").
-   翻译了此文件中的条目后,你就可以使用 `--apply` 命令将更改应用于原始本地化文件.

> 该命令的主要目的是翻译 XABP 框架本地化文件(因为[xabp 仓库](https://github.com/ArcherTrister/xabp)包括数十个要在不同目录中转换的本地化文件).

#### 创建翻译文件

第一步是创建统一的翻译文件:

```bash
xabp translate -c <culture> [options]
```

示例:

```bash
xabp translate -c de
```

该命令为 `de-DE` (德语)文化创建了统一的翻译文件.

##### 附加选项

-   `--reference-culture` 或 `-r`: 默认值 `en`. 指定参考文化.
-   `--output` 或 `-o`: 输出文件名. 默认值 `abp-translation.json`.
-   `--all-values` 或 `-all`: 包括所有要翻译的键. 默认情况下,统一翻译文件仅包含目标文化的缺失文本. 如果你可能需要修改之前已经翻译的值,请指定此参数.

#### 应用更改

翻译完统一翻译文件中的条目后,你可以使用 `--apply` 参数将更改应用于原始本地化文件:

```bash
xabp translate --apply  # apply all changes
xabp translate -a       # shortcut for --apply
```

然后,检查源代码控制系统上的更改,以确保它已更改了正确的文件. 如果你翻译了 XABP 框架资源, 请发送 "Pull Request". 提前感谢你的贡献.

##### 附加选项

-   `--file` 或 `-f`: 默认值: `abp-translation.json`. 翻译文件(仅在之前使用过 `--output` 选项时使用).

### bundle

这个命令为 XABP Blazor WebAssembly 和 MAUI Blazor 项目生成引用的脚本和样式并且更新 **index.html** 文件, 它帮助开发者轻松的管理 XABP 模块的依赖. 为了使 `bundle` 命令工作, 它的**执行目录**或者传递 `--working-directory` 参数的目录必须包含 Blazor 或 MAUI Blazor 项目文件(\*.csproj)

用法:

```bash
xabp bundle [options]
```

#### Options

-   `--working-directory` 或 `-wd`: 指定当前执行目录, 这个命令在当前目录不包含项目文件时非常有用.
-   `--force` 或 `-f`: 在生成引用之前强制构建项目.
-   `--project-type` 或 `-t`: 指定项目类型. 默认类型是 `webassembly`. 可用的类型有:
    -   `webassembly`
    -   `maui-blazor`

`bundle` command reads the `appsettings.json` file inside the Blazor and MAUI Blazor project for bundling options. For more details about managing style and script references in Blazor or MAUI Blazor apps, see [Managing Global Scripts & Styles](https://abp.io/docs/en/abp/latest/UI/Blazor/Global-Scripts-Styles)

### install-libs

为 MVC / Razor Pages 和 Blazor Server UI 类型安装 NPM 包, 它的 **执行目录** 或者传递的 `--working-directory` 目录必须包含一个项目文件(\*.csproj).

`install-libs` 命令读取 `abp.resourcemapping.js` 来管理包. 参阅[客户端包管理](https://abp.io/docs/zh-Hans/abp/latest/UI/AspNetCore/Client-Side-Package-Management)了解更多细节.

用法:

```bash
xabp install-libs [options]
```

#### Options

-   `--working-directory` 或 `-wd`: 指定工作目录, 当执行目录不包含项目文件时会很有用.

### create

生成基于 XABP[启动模板](https://abp.io/startup-templates?culture=zh-Hans)的新解决方案.

用法:

```bash
xabp create <解决方案名称> [options]
```

示例:

```bash
xabp create Acme.BookStore
```

-   Acme.BookStore 是解决方案的名称.
-   常见的命名方式类似于 _YourCompany.YourProject_. 不过你可以使用自己喜欢的方式,如 _YourProject_ (单级命名空间) 或 _YourCompany.YourProduct.YourModule_ (三级命名空间).

参阅[XABP CLI 创建新解决方案示例](https://abp.io/docs/latest/cli/new-command-samples)查看更多示例.

#### Options

-   `--template` 或者 `-t`: 指定模板. 默认的模板是 `xaml`,会生成 web 项目.可用的模板有:

    -   [x] ~~`xais4` : IdentityServer4 多层普通应用程序模板.~~
    -   [x] ~~`xaod` : OpenIddict 多层普通应用程序模板~~
    -   [x] `xais4sts` : IdentityServer4 租户架构分离应用程序模板.
    -   [x] `xaodsts` : OpenIddict 租户架构分离应用程序模板.
    -   [ ] `xais4sas` : IdentityServer4 授权服务分离应用程序模板.
    -   [ ] `xaodsas` : OpenIddict 授权服务分离应用程序模板.
    -   [ ] `xais4sassts` : IdentityServer4 SeparatedAuthServer and SeparatedTenantSchema 应用程序模板.
    -   [ ] `xaodsassts` : OpenIddict SeparatedAuthServer and SeparatedTenantSchema 应用程序模板.
    -   [x] `xais4mo` : IdentityServer4 Ocelot 微服务应用程序模板.
    -   [x] `xais4my` : IdentityServer4 Yarp 微服务应用程序模板.
    -   [x] `xaodmo` : OpenIddict Ocelot 微服务应用程序模板.
    -   [x] `xaodmy` : OpenIddict Yarp 微服务应用程序模板.

    ***

    -   [ ] `xams` : 微服务应用程序模板,需搭配 `-tt` 参数使用.
    -   [x] `xaml` (**default**): 多层普通应用程序模板,需搭配 `-tt` 参数使用.
    -   [x] `xamlsas` : 多层授权服务分离应用程序模板,需搭配 `-tt` 参数使用.
    -   [ ] `xamlsts` : 多层租户架构分离应用程序模板,需搭配 `-tt` 参数使用.
    -   [x] `xasl` : 单层应用程序模板,需搭配 `-tt` 参数使用.

-   `--template-type` 或者 `-tt`: 指定模板类型,默认是 IdentityServer4.
-   `--output-folder` 或者 `-o`: 指定输出文件夹,默认是当前目录.
-   `--version` 或者 `-v`: 指定 XABP 和模板的版本.它可以是 [release tag](https://github.com/ArcherTrister/xabp/releases) 或者 [branch name](https://github.com/ArcherTrister/xabp/branches). 如果没有指定,则使用最新版本.大多数情况下,你会希望使用最新的版本.
-   `--create-solution-folder` 或者 `-csf`: 指定项目是在输出文件夹中的新文件夹中还是直接在输出文件夹中.
-   `--connection-string` 或者 `-cs`: 重写所有 `appsettings.json` 文件的默认连接字符串. 默认连接字符串是 `Server=localhost;Database=MyProjectName;Trusted_Connection=True`. 默认的数据库提供程序是 `SQL Server`. 如果你使用 EF Core 但需要更改 DBMS,可以按[这里所述](https://abp.io/docs/zh-Hans/abp/latest/Entity-Framework-Core-Other-DBMS)进行更改(创建解决方案之后).
-   `--database-management-system` or `-dbms`: Sets the database management system. Default is **SQL Server**. Supported DBMS's:
    -   `SqlServer`
    -   `MySQL`
    -   `SQLite`
    -   `Oracle`
    -   `Oracle-Devart`
    -   `PostgreSQL`
        ::tip
        使用 mysql 需要先创建一个空的数据库再创建项目，否则会报错 `Unknown database`.
-   `--install-template` 或者 `-it`: 是否安装项目模板,默认 false.
-   `--include-vue` 或者 `-iv`: 是否包含 Vue 项目,默认 false.
-   `--enable-swagger-enum-filter` 或者 `-esef`: 是否启用 Swagger 枚举过滤器,默认 true.
-   `--install-template` 或者 `-it`: 是否安装项目模板,默认 false.

### generate-cert

为你的项目生成证书文件，一般用于生成 IdentityServer4 所需证书文件.

用法:

```bash
xabp generate-cert -t <cert-type> [options]
```

示例:

```bash
xabp generate-cert
xabp generate-cert -o MyCerts/InnerFolder
xabp generate-cert -o MyCerts/InnerFolder -n identityserver
xabp generate-cert -o MyCerts/InnerFolder -p 123456
xabp generate-cert -t rsa -o MyCerts/InnerFolder -p 123456
xabp generate-cert -t ecd -o MyCerts/InnerFolder -p 123456
xabp generate-cert -t ecd -o MyCerts/InnerFolder -p 123456 -y 10
xabp generate-cert -t ecd -o MyCerts/InnerFolder -p 123456 -d www.domain.com -y 10
```

#### Options

-   `--type` 或 `-t`: 证书类型.默认值: `rsa`. 可用的证书类型有:
    -   `rsa`: RSA. 此证书有一些可选选项:
        -   `--output` 或 `-o`: Folder name to place generated Cert file in. 默认值: ``.
        -   `--name` 或 `-n`: Generated Cert File name. 默认值: `idsrv4`.
        -   `--password` 或 `-p`: The cert file password. 默认值: `123456`
        -   `--dns-name` 或 `-d`: The cert file domain name. 默认值: `localhost`.
        -   `--years` 或 `-y`: The cert file valid duration. 默认值: `1`.
    -   `ecd`: ECD. 此证书有一些可选选项:
        -   `--output` 或 `-o`: Folder name to place generated Cert file in. 默认值: ``.
        -   `--name` 或 `-n`: Generated Cert File name. 默认值: `idsrv4`.
        -   `--password` 或 `-p`: The cert file password. 默认值: `123456`
        -   `--dns-name` 或 `-d`: The cert file domain name. 默认值: `localhost`.
        -   `--years` 或 `-y`: The cert file valid duration. 默认值: `1`.
-   `--working-directory` or `-wd`: 执行目录.

### install-module

通过查找模块的所有包,查找解决方案中的相关项目,并将每个包添加到解决方案中的相应项目,从而将[多包应用程序模块](https://github.com/ArcherTrister/xabp#%E5%BA%94%E7%94%A8%E7%A8%8B%E5%BA%8F%E6%A8%A1%E5%9D%97)添加到解决方案中.

> 由于分层,不同的数据库提供程序选项或其他原因,业务模块通常由多个包组成. 使用`install-module`命令可以大大简化向模块添加模块的过程. 但是每个模块可能需要一些其他配置,这些配置通常在相关模块的文档中指出.

用法:

```bash
xabp install-module <模块名称> [options]
```

示例:

```bash
xabp install-module X.Abp.Chat
```

-   示例中将 X.Chat 模块添加到解决方案中.

#### Options

-   `--solution` 或 `-s`: 指定解决方案 (.sln) 路径. 如果未指定,CLI 会尝试在当前目录中寻找.sln 文件.
-   `--skip-db-migrations`: 对于 EF Core 数据库提供程序,它会自动添加新代码的第一次迁移 (`Add-Migration`) 并且在需要时更新数据库 (`Update-Database`). 指定此选项可跳过此操作.
-   `-sp` 或 `--startup-project`: 启动项目的项目文件夹的相对路径. 默认值是当前文件夹.
-   ~~`--with-source-code`: 添加模块的源代码,而不是 NuGet/NPM 软件包.~~
-   ~~`--add-to-solution-file`: 添加下载/创建的模块添加到解决方案文件中,你在 IDE 中打开解决方案时也会看到模块的项目. (仅当 `--with-source-code` 为 `True` 时可用.)~~
