// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.VersionManagement.AppEditions.Dtos;

public class CheckUpdateOutput
{
    /// <summary>
    /// 是否需要更新
    /// </summary>
    public bool IsNeedUpdate { get; set; }

    /// <summary>
    /// 强制更新
    /// </summary>
    public ForceUpdateDto ForceUpdate { get; set; }
}
