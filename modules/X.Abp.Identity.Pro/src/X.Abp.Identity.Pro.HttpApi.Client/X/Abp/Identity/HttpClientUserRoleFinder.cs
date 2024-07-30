// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Linq;
using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;

namespace X.Abp.Identity;

[Dependency(TryRegister = true)]
public class HttpClientUserRoleFinder : IUserRoleFinder, ITransientDependency
{
    protected IIdentityUserAppService UserAppService { get; }

    public HttpClientUserRoleFinder(IIdentityUserAppService userAppService)
    {
        UserAppService = userAppService;
    }

    public virtual async Task<string[]> GetRolesAsync(Guid userId)
    {
        var output = await UserAppService.GetRolesAsync(userId);
        return output.Items.Select(r => r.Name).ToArray();
    }

    public virtual async Task<string[]> GetRoleNamesAsync(Guid userId)
    {
        var output = await UserAppService.GetRolesAsync(userId);
        return output.Items.Select(r => r.Name).ToArray();
    }
}
