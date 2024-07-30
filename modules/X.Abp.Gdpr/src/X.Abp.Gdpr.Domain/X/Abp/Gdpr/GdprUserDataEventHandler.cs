// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Linq;
using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Gdpr;
using Volo.Abp.Guids;
using Volo.Abp.Json;
using Volo.Abp.Uow;

namespace X.Abp.Gdpr;

public class GdprUserDataEventHandler :
IDistributedEventHandler<GdprUserDataPreparedEto>,
IEventHandler,
ITransientDependency
{
    protected IGdprRequestRepository GdprRequestRepository { get; }

    protected IGuidGenerator GuidGenerator { get; }

    protected IUnitOfWorkManager UnitOfWorkManager { get; }

    protected IJsonSerializer JsonSerializer { get; }

    public GdprUserDataEventHandler(
      IGdprRequestRepository gdprRequestRepository,
      IGuidGenerator guidGenerator,
      IUnitOfWorkManager unitOfWorkManager,
      IJsonSerializer jsonSerializer)
    {
        GdprRequestRepository = gdprRequestRepository;
        GuidGenerator = guidGenerator;
        UnitOfWorkManager = unitOfWorkManager;
        JsonSerializer = jsonSerializer;
    }

    [UnitOfWork(true)]
    public virtual async Task HandleEventAsync(GdprUserDataPreparedEto eventData)
    {
        if (eventData.Data.Any())
        {
            var gdprRequest = await GdprRequestRepository.FindAsync(eventData.RequestId);
            if (gdprRequest != null)
            {
                var data = JsonSerializer.Serialize(eventData.Data);
                gdprRequest.AddData(GuidGenerator.Create(), data, eventData.Provider);
                await GdprRequestRepository.UpdateAsync(gdprRequest);
            }
        }
    }
}
