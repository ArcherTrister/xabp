// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

namespace X.Abp.Account.Public.Web.ProfileManagement;

public class ProfileManagementPageCreationContext
{
    public IServiceProvider ServiceProvider { get; }

    public ICollection<ProfileManagementPageGroup> Groups { get; }

    public ProfileManagementPageCreationContext(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;

        Groups = new List<ProfileManagementPageGroup>();
    }
}
