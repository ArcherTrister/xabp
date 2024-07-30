// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;

namespace X.Abp.CmsKit.Polls;

public class CmsKitPollingOptions
{
    public List<string> WidgetNames { get; }

    public CmsKitPollingOptions()
    {
        WidgetNames = new List<string>();
    }

    public void AddWidget(string name)
    {
        if (WidgetNames.Contains(name))
        {
            throw new PollOptionWidgetNameCannotBeSameException(name);
        }

        WidgetNames.Add(name);
    }
}
