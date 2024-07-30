// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Diagnostics;

using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

using X.Abp.Quartz.Localization;

namespace X.Abp.Quartz.Web.Pages.Quartz;

/* Inherit your PageModel classes from this class.
 */
public abstract class QuartzPageModel : AbpPageModel
{
    public Stopwatch GenerationTime { get; }

    protected QuartzPageModel()
    {
        LocalizationResourceType = typeof(QuartzResource);
        ObjectMapperContext = typeof(AbpQuartzWebModule);
        GenerationTime = Stopwatch.StartNew();
    }

    ///// <summary>
    ///// 获取初始化时间
    ///// </summary>
    // protected virtual Stopwatch GetGenerationTime()
    // {
    //     return GenerationTime;
    // }
}
