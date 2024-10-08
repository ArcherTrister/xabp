﻿// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Users;

namespace X.Abp.Identity;

[Dependency(TryRegister = true)]
public class HttpClientExternalUserLookupServiceProvider : IExternalUserLookupServiceProvider, ITransientDependency
{
  protected IIdentityUserLookupAppService UserLookupAppService { get; }

  public HttpClientExternalUserLookupServiceProvider(IIdentityUserLookupAppService userLookupAppService)
  {
    UserLookupAppService = userLookupAppService;
  }

  public virtual async Task<IUserData> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    return await UserLookupAppService.FindByIdAsync(id);
  }

  public virtual async Task<IUserData> FindByUserNameAsync(string userName, CancellationToken cancellationToken = default)
  {
    return await UserLookupAppService.FindByUserNameAsync(userName);
  }

  public virtual async Task<List<IUserData>> SearchAsync(
      string sorting = null,
      string filter = null,
      int maxResultCount = int.MaxValue,
      int skipCount = 0,
      CancellationToken cancellationToken = default)
  {
    var result = await UserLookupAppService.SearchAsync(
        new UserLookupSearchInputDto
        {
          Sorting = sorting,
          Filter = filter,
          MaxResultCount = maxResultCount,
          SkipCount = skipCount
        }
    );

    return result.Items.Cast<IUserData>().ToList();
  }

  public virtual async Task<long> GetCountAsync(
      string filter = null,
      CancellationToken cancellationToken = default)
  {
    return await UserLookupAppService
        .GetCountAsync(
            new UserLookupCountInputDto
            {
              Filter = filter
            }
        );
  }
}
