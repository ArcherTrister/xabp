// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Linq;
using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace X.Abp.Identity.Integration
{
    public class IdentityUserIntegrationService : IdentityAppServiceBase, IIdentityUserIntegrationService
    {
        protected IIdentityUserRepository UserRepository { get; }

        protected IdentityUserRepositoryExternalUserLookupServiceProvider UserLookupServiceProvider { get; }

        public IdentityUserIntegrationService(
          IIdentityUserRepository userRepository,
          IdentityUserRepositoryExternalUserLookupServiceProvider userLookupServiceProvider)
        {
            UserRepository = userRepository;
            UserLookupServiceProvider = userLookupServiceProvider;
        }

        public async Task<string[]> GetRoleNamesAsync(Guid id) => (await UserRepository.GetRoleNamesAsync(id)).ToArray();

        public async Task<UserData> FindByIdAsync(Guid id)
        {
            IUserData userData = await UserLookupServiceProvider.FindByIdAsync(id);
            return userData != null ? new UserData(userData) : null;
        }

        public async Task<UserData> FindByUserNameAsync(string userName)
        {
            IUserData userData = await UserLookupServiceProvider.FindByUserNameAsync(userName);
            return userData != null ? new UserData(userData) : null;
        }

        public async Task<ListResultDto<UserData>> SearchAsync(UserLookupSearchInputDto input)
        {
            return new ListResultDto<UserData>((await UserLookupServiceProvider.SearchAsync(input.Sorting, input.Filter, input.MaxResultCount, input.SkipCount)).Select(u => new UserData(u)).ToList());
        }

        public async Task<long> GetCountAsync(UserLookupCountInputDto input)
        {
            return await UserLookupServiceProvider.GetCountAsync(input.Filter);
        }
    }
}
