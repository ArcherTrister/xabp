// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Users;

namespace X.Abp.Chat.Users;

public class ChatUserSynchronizer :
    IDistributedEventHandler<EntityUpdatedEto<UserEto>>,
    ITransientDependency
{
    protected IChatUserRepository UserRepository { get; }

    protected IChatUserLookupService UserLookupService { get; }

    public ChatUserSynchronizer(
        IChatUserRepository userRepository,
        IChatUserLookupService userLookupService)
    {
        UserRepository = userRepository;
        UserLookupService = userLookupService;
    }

    public virtual async Task HandleEventAsync(EntityUpdatedEto<UserEto> eventData)
    {
        var user = await UserRepository.FindAsync(eventData.Entity.Id);
        if (user == null)
        {
            user = await UserLookupService.FindByIdAsync(eventData.Entity.Id);
            if (user == null)
            {
                return;
            }
        }

        if (user.Update(eventData.Entity))
        {
            await UserRepository.UpdateAsync(user);
        }
    }
}
