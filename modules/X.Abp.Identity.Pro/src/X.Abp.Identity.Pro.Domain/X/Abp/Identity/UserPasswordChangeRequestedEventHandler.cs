// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace X.Abp.Identity;

public class UserPasswordChangeRequestedEventHandler :
    IDistributedEventHandler<UserPasswordChangeRequestedEto>,
    ITransientDependency
{
  public ILogger<UserPasswordChangeRequestedEventHandler> Logger { get; set; }

  protected IIdentityUserRepository UserRepository { get; }

  protected IdentityUserManager IdentityUserManager { get; }

  public UserPasswordChangeRequestedEventHandler(
    IIdentityUserRepository userRepository,
    IdentityUserManager identityUserManager)
  {
    Logger = NullLogger<UserPasswordChangeRequestedEventHandler>.Instance;
    UserRepository = userRepository;
    IdentityUserManager = identityUserManager;
  }

  public virtual async Task HandleEventAsync(UserPasswordChangeRequestedEto eventData)
  {
    if (!eventData.Password.IsNullOrEmpty())
    {
      var user = await UserRepository.FindByTenantIdAndUserNameAsync(eventData.UserName, eventData.TenantId);
      if (user != null)
      {
        var source = await CheckUserPasswordAsync(user, eventData.Password);
        if (source.Count != 0)
        {
          Logger.LogError("User password change failed: {userName}, reason: {reason}", eventData.UserName, string.Join(";", source.Select(e => e.Code)));
        }
        else
        {
          (await IdentityUserManager.RemovePasswordAsync(user)).CheckIdentityErrors();
          (await IdentityUserManager.AddPasswordAsync(user, eventData.Password)).CheckIdentityErrors();
          Logger.LogInformation("User password changed: {userName}", eventData.UserName);
        }
      }
    }
  }

  private async Task<List<IdentityError>> CheckUserPasswordAsync(IdentityUser identityUser, string password)
  {
    var errors = new List<IdentityError>();
    foreach (var passwordValidator in IdentityUserManager.PasswordValidators)
    {
      var identityResult = await passwordValidator.ValidateAsync(IdentityUserManager, identityUser, password);
      if (!identityResult.Succeeded && identityResult.Errors.Any())
      {
        errors.AddRange(identityResult.Errors);
      }
    }

    return errors;
  }
}
