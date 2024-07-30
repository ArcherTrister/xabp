// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.Domain.Repositories;

namespace X.Abp.Forms.Questions;

public interface IQuestionRepository : IBasicRepository<QuestionBase, Guid>
{
    Task<List<QuestionBase>> GetListByFormIdAsync(
        Guid formId,
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        string filter = null,
        bool includeDetails = true,
        CancellationToken cancellationToken = default);

    Task<QuestionWithChoices> GetWithChoices(Guid id, CancellationToken cancellationToken = default);

    Task<List<QuestionWithAnswers>> GetListWithAnswersByFormResponseId(Guid formResponseId,
        CancellationToken cancellationToken = default);

    Task<List<QuestionWithAnswers>> GetListWithAnswersByFormId(Guid formId,
        CancellationToken cancellationToken = default);

    Task ClearQuestionChoicesAsync(Guid itemId, CancellationToken cancellationToken = default);
}
