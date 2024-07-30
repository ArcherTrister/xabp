// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;

using X.Abp.Account;
using X.Abp.Identity;

namespace AbpVnext.Pro.HttpApi.Client.ConsoleTestApp;

public class ClientDemoService : ITransientDependency
{
    private readonly IProfileAppService _profileAppService;
    private readonly IIdentityUserAppService _identityUserAppService;

    public ClientDemoService(
        IProfileAppService profileAppService,
        IIdentityUserAppService identityUserAppService)
    {
        _profileAppService = profileAppService;
        _identityUserAppService = identityUserAppService;
    }

    public async Task RunAsync()
    {
        var profileDto = await _profileAppService.GetAsync();
        Console.WriteLine($"UserName : {profileDto.UserName}");
        Console.WriteLine($"Email    : {profileDto.Email}");
        Console.WriteLine($"Name     : {profileDto.Name}");
        Console.WriteLine($"Surname  : {profileDto.Surname}");
        Console.WriteLine();

        var resultDto = await _identityUserAppService.GetListAsync(new GetIdentityUsersInput());
        Console.WriteLine($"Total users: {resultDto.TotalCount}");
        foreach (var identityUserDto in resultDto.Items)
        {
            Console.WriteLine($"- [{identityUserDto.Id}] {identityUserDto.Name}");
        }
    }
}
