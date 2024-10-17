// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.CmsKit.Admin.Newsletters;

public class ImportNewslettersFromFileOutput
{
    public int AllCount { get; set; }

    public int SucceededCount { get; set; }

    public int FailedCount { get; set; }

    public string InvalidNewslettersDownloadToken { get; set; }

    public bool IsAllSucceeded => AllCount == SucceededCount;
}
