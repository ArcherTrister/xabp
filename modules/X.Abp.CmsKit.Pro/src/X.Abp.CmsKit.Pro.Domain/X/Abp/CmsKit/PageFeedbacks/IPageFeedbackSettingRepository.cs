// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.Domain.Repositories;

namespace X.Abp.CmsKit.PageFeedbacks
{
    public interface IPageFeedbackSettingRepository : IBasicRepository<PageFeedbackSetting, Guid>
    {
        Task<List<PageFeedbackSetting>> GetListByEntityTypesAsync(
          List<string> entityTypes,
          CancellationToken cancellationToken = default);

        Task<PageFeedbackSetting> FindByEntityTypeAsync(
          string entityType,
          CancellationToken cancellationToken = default);

        Task DeleteOldSettingsAsync(
          List<string> existingEntityTypes,
          CancellationToken cancellationToken = default);
    }
}
