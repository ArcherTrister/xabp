// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using Quartz.Impl.AdoJobStore;

using Volo.Abp.DependencyInjection;

using Volo.Abp.Quartz;

namespace X.Abp.Quartz;

public abstract class ObjectsInstallerBase : IObjectsInstaller, ISingletonDependency
{
    public abstract Task Initialize(AbpQuartzOptions options, AbpQuartzInstallScriptOptions installScriptOptions);

    protected virtual string GetInstallScript(Assembly scriptAssembly, string scriptResourceName, string dataBaseName, string tablePrefix, bool enableHeavyMigrations)
    {
        var script = GetStringResource(scriptAssembly, scriptResourceName);

        script = script.Replace("$(TablePrefix)", !string.IsNullOrWhiteSpace(tablePrefix) ? tablePrefix : AdoConstants.DefaultTablePrefix)
            .ReplaceFirst("$(QuartzSchema)", dataBaseName);

        if (!enableHeavyMigrations)
        {
            script = script.Replace("--SET @DISABLE_HEAVY_MIGRATIONS = 1;", "SET @DISABLE_HEAVY_MIGRATIONS = 1;");
        }

        return script;
    }

    /// <summary>
    /// 嵌入式资源转字符串
    /// </summary>
    /// <param name="assembly">assembly</param>
    /// <param name="resourceName">资源名称【全称】</param>
    protected virtual string GetStringResource(Assembly assembly, string resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new InvalidOperationException(
                $"Requested resource `{resourceName}` was not found in the assembly `{assembly}`.");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
