// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

namespace X.Abp.CmsKit.Admin.PageFeedbacks;

[Serializable]
public class UpdatePageFeedbackSettingsInput
{
    public List<UpdatePageFeedbackSettingDto> Settings { get; set; }

    public UpdatePageFeedbackSettingsInput()
    {
        Settings = new List<UpdatePageFeedbackSettingDto>();
    }

    public UpdatePageFeedbackSettingsInput(List<UpdatePageFeedbackSettingDto> settings)
    {
        Settings = settings;
    }
}
