// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.Domain.Repositories;

namespace X.Abp.CmsKit.Polls;

public interface IPollRepository : IBasicRepository<Poll, Guid>
{
    Task<List<Poll>> GetListAsync(
      string filter = null,
      string sorting = null,
      int skipCount = 0,
      int maxResultCount = int.MaxValue,
      CancellationToken cancellationToken = default);

    Task<List<Poll>> GetListByWidgetAsync(
      string widget,
      CancellationToken cancellationToken = default);

    // Task<Poll> FindByWidgetAsync(string widget, CancellationToken cancellationToken = default);
    Task<Poll> FindByAvailableWidgetAsync(string widget, DateTime now, CancellationToken cancellationToken = default);

    Task<Poll> FindByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(string filter = null, CancellationToken cancellationToken = default);

    Task<IQueryable<Poll>> WithDetailsAsync();

    Task<Dictionary<Poll, List<PollUserVote>>> GetPollWithPollUserVotesAsync(Guid id);

    Task<PollWithUserVotes> GetPollWithPollUserVotesAsync(Guid id, Guid userId);

    Task<Poll> FindByDateRangeAndWidgetAsync(
      DateTime startDate,
      DateTime? endDate = null,
      DateTime? resultShowingEndDate = null,
      string widget = null,
      CancellationToken cancellationToken = default);
}
