// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

using X.Abp.CmsKit.EntityFrameworkCore;

namespace X.Abp.CmsKit.Polls;

public class EfCorePollRepository :
EfCoreRepository<CmsKitProDbContext, Poll, Guid>,
IPollRepository
{
  public EfCorePollRepository(
    IDbContextProvider<CmsKitProDbContext> dbContextProvider)
    : base(dbContextProvider)
  {
  }

  public override async Task<IQueryable<Poll>> WithDetailsAsync()
  {
    return (await base.GetQueryableAsync()).IncludeDetails();
  }

  public virtual async Task<Dictionary<Poll, List<PollUserVote>>> GetPollWithPollUserVotesAsync(Guid id)
  {
    // TODO: DEAL WITH
    var dbContext = await GetDbContextAsync();
    var dataList =
        await (await WithDetailsAsync())
        .GroupJoin(dbContext.PollUserVotes,
        poll => poll.Id,
        pollUserVote => pollUserVote.PollId,
        (poll, pollUserVotes) => new
        {
          poll,
          pollUserVotes
        }).SelectMany(data => data.pollUserVotes.DefaultIfEmpty(),
    (data, pollUserVote) => new
    {
      TransparentIdentifier = data,
      pollUserVote
    }).Where(x => x.TransparentIdentifier.poll.Id == id)
    .Select(data => new
    {
      data.TransparentIdentifier.poll,
      data.pollUserVote
    }).ToListAsync();

    var dictionary = new Dictionary<Poll, List<PollUserVote>>();
    foreach (var data in dataList)
    {
      if (!dictionary.TryGetValue(data.poll, out var _))
      {
        dictionary.Add(data.poll, new List<PollUserVote>());
      }

      if (data.pollUserVote != null)
      {
        dictionary[data.poll].Add(data.pollUserVote);
      }
    }

    return dictionary;
  }

  public virtual async Task<List<Poll>> GetListByWidgetAsync(string widget, CancellationToken cancellationToken = default)
  {
    return string.IsNullOrWhiteSpace(widget)
        ? new List<Poll>()
        : await (await GetDbSetAsync()).Where(poll => poll.Widget == widget).ToListAsync(GetCancellationToken(cancellationToken));
  }

  public virtual async Task<Poll> FindByWidgetAsync(string widget, CancellationToken cancellationToken = default)
  {
    return string.IsNullOrWhiteSpace(widget)
        ? null
        : await (await GetDbSetAsync()).IncludeDetails().FirstOrDefaultAsync(poll => poll.Widget == widget, GetCancellationToken(cancellationToken));
  }

  public virtual async Task<Poll> FindByCodeAsync(string code, CancellationToken cancellationToken = default)
  {
    return string.IsNullOrWhiteSpace(code)
        ? null
        : await (await GetDbSetAsync()).IncludeDetails().FirstOrDefaultAsync(poll => poll.Code == code, GetCancellationToken(cancellationToken));
  }

  public virtual async Task<long> GetCountAsync(string filter = null, CancellationToken cancellationToken = default)
  {
    return await (await GetDbSetAsync()).WhereIf(!filter.IsNullOrWhiteSpace(), poll => poll.Question.Contains(filter) || poll.Name.Contains(filter)).LongCountAsync(GetCancellationToken(cancellationToken));
  }

  public virtual async Task<List<Poll>> GetListAsync(
    string filter = null,
    string sorting = null,
    int skipCount = 0,
    int maxResultCount = int.MaxValue,
    CancellationToken cancellationToken = default)
  {
    return await (await GetDbSetAsync()).WhereIf(!filter.IsNullOrWhiteSpace(), poll => poll.Question.Contains(filter) || poll.Name.Contains(filter))
        .OrderBy(sorting.IsNullOrEmpty() ? "CreationTime desc" : sorting)
        .PageBy(skipCount, maxResultCount)
        .ToListAsync(GetCancellationToken(cancellationToken));
  }
}
