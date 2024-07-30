// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Gdpr;
using Volo.Abp.Identity;
using Volo.Abp.Uow;

namespace X.Abp.Identity.Gdpr;

public class IdentityGdprEventHandler :
    IDistributedEventHandler<GdprUserDataDeletionRequestedEto>,
    IDistributedEventHandler<GdprUserDataRequestedEto>,
    ITransientDependency
{
    protected IdentityUserManager IdentityUserManager { get; }

    protected IIdentityUserRepository IdentityUserRepository { get; }

    protected IDistributedEventBus EventBus { get; }

    public IdentityGdprEventHandler(
        IdentityUserManager identityUserManager,
        IIdentityUserRepository identityUserRepository,
        IDistributedEventBus eventBus)
    {
        IdentityUserManager = identityUserManager;
        IdentityUserRepository = identityUserRepository;
        EventBus = eventBus;
    }

    [UnitOfWork]
    public virtual async Task HandleEventAsync(GdprUserDataDeletionRequestedEto eventData)
    {
        var identityUser = await IdentityUserManager.GetByIdAsync(eventData.UserId);
        identityUser.Name = string.Empty;
        identityUser.Surname = string.Empty;
        identityUser.SetIsActive(false);

        await IdentityUserManager.SetPhoneNumberAsync(identityUser, string.Empty);
        await IdentityUserManager.SetEmailAsync(identityUser, string.Empty);

        await IdentityUserRepository.UpdateAsync(identityUser);
        await IdentityUserRepository.DeleteAsync(identityUser);
    }

    public virtual async Task HandleEventAsync(GdprUserDataRequestedEto eventData)
    {
        var identityUser = await IdentityUserRepository.GetAsync(eventData.UserId);

        var gdprDataInfo = new GdprDataInfo
        {
            { "Username", identityUser.UserName },
            { "Name", identityUser.Name },
            { "Surname", identityUser.Surname },
            { "Email", identityUser.Email },
            { "Phone Number", identityUser.PhoneNumber }
        };

        var claims = identityUser.Claims;
        foreach (var identityUserClaim in claims)
        {
            gdprDataInfo.Add(identityUserClaim.ClaimType, identityUserClaim.ClaimValue);
        }

        await EventBus.PublishAsync(
            new GdprUserDataPreparedEto
            {
                RequestId = eventData.RequestId,
                Data = gdprDataInfo,
                Provider = GetType().FullName
            });
    }
}
