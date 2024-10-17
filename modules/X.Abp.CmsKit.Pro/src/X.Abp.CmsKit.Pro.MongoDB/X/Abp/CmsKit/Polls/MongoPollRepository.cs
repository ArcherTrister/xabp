// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

using X.Abp.CmsKit.Pro.MongoDB;

namespace X.Abp.CmsKit.Polls
{
    public class MongoPollRepository : MongoDbRepository<ICmsKitProMongoDbContext, Poll, Guid>, IPollRepository
    {
        public MongoPollRepository(IMongoDbContextProvider<ICmsKitProMongoDbContext> dbContextProvider)
          : base(dbContextProvider)
        {
        }

        public virtual async Task<Poll> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(cancellationToken, null))
                .FirstOrDefaultAsync(poll => poll.Id == id, GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetCountAsync(string filter = null, CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(cancellationToken, null))
                .WhereIf(!filter.IsNullOrWhiteSpace(), poll => poll.Question.Contains(filter) || poll.Name.Contains(filter))
                .As<IMongoQueryable<Poll>>()
                .LongCountAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<Poll>> GetListAsync(
          string filter = null,
          string sorting = null,
          int skipCount = 0,
          int maxResultCount = int.MaxValue,
          CancellationToken cancellationToken = default)
        {
            CancellationToken token = GetCancellationToken(cancellationToken);
            return await (await GetMongoQueryableAsync(token, null))
                .WhereIf(!filter.IsNullOrWhiteSpace(), poll => poll.Question.Contains(filter) || poll.Name.Contains(filter))
                .OrderBy(sorting.IsNullOrEmpty() ? "CreationTime desc" : sorting)
                      .As<IMongoQueryable<Poll>>()
                      .PageBy<Poll, IMongoQueryable<Poll>>(skipCount, maxResultCount)
                      .ToListAsync(token);
        }

        public virtual async Task<List<Poll>> GetListByWidgetAsync(
          string widget,
          CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(widget))
            {
                return new List<Poll>();
            }

            return await (await GetMongoQueryableAsync(cancellationToken, null))
                .Where(poll => poll.Widget == widget)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<Poll> FindByAvailableWidgetAsync(
          string widget,
          DateTime now,
          CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(widget))
            {
                return null;
            }

            return await (await GetMongoQueryableAsync(cancellationToken, null)).FirstOrDefaultAsync(poll => poll.Widget == widget && ((poll.StartDate <= now && (poll.EndDate.HasValue == false || poll.EndDate >= now)) || (poll.ResultShowingEndDate.HasValue && poll.ResultShowingEndDate >= now)), GetCancellationToken(cancellationToken));
        }

        public virtual async Task<Poll> FindByCodeAsync(
          string code,
          CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }

            return await (await GetMongoQueryableAsync(cancellationToken, null)).FirstOrDefaultAsync(poll => poll.Code == code, GetCancellationToken(cancellationToken));
        }

        public virtual async Task<Dictionary<Poll, List<PollUserVote>>> GetPollWithPollUserVotesAsync(Guid id)
        {
            var dbContext = await GetDbContextAsync();
            var queryable = (await GetMongoQueryableAsync(new CancellationToken(), null))
                .Where(poll => poll.Id == id)
                .GroupJoin(dbContext.PollUserVotes.AsQueryable(), poll => poll.Id, pollUserVote => pollUserVote.PollId, (poll, pollUserVotes) => new
                {
                    poll,
                    pollUserVotes
                });

            var dataList = await queryable.ToListAsync();
            Dictionary<Poll, List<PollUserVote>> dictionary = new Dictionary<Poll, List<PollUserVote>>();
            foreach (var data in dataList)
            {
                if (!dictionary.ContainsKey(data.poll))
                {
                    dictionary.Add(data.poll, new List<PollUserVote>());
                }

                if (data.pollUserVotes != null)
                {
                    dictionary[data.poll] = data.pollUserVotes.ToList();
                }
            }

            return dictionary;
        }

        public virtual async Task<PollWithUserVotes> GetPollWithPollUserVotesAsync(
          Guid id,
          Guid userId)
        {
            ICmsKitProMongoDbContext dbContext = await GetDbContextAsync();

            return await (await GetMongoQueryableAsync())
                .GroupJoin(dbContext.PollUserVotes, poll => poll.Id, pollUserVote => pollUserVote.PollId, (poll, pollUserVotes) => new
                {
                    Poll = poll,
                    UserVotes = pollUserVotes
                }).Where(data => data.Poll.Id == id && data.UserVotes.Any(uv => uv.UserId == userId))
            .Select(p => new PollWithUserVotes { Poll = p.Poll, UserVotes = p.UserVotes }).FirstAsync();
        }

        public virtual async Task<Poll> FindByDateRangeAndWidgetAsync(
          DateTime startDate,
          DateTime? endDate = null,
          DateTime? resultShowingEndDate = null,
          string widget = null,
          CancellationToken cancellationToken = default)
        {
            ExpressionStarter<Poll> expression = PredicateBuilder.New<Poll>();
            expression = expression.And(poll => poll.StartDate <= startDate && (!poll.EndDate.HasValue || poll.EndDate.Value >= startDate));
            if (endDate.HasValue)
            {
                expression = expression.Or(poll => poll.StartDate <= endDate && poll.EndDate >= endDate);
            }

            if (resultShowingEndDate.HasValue)
            {
                expression = expression.Or(poll => poll.StartDate <= resultShowingEndDate && (!poll.EndDate.HasValue || poll.EndDate.Value >= resultShowingEndDate));
            }

            if (!widget.IsNullOrWhiteSpace())
            {
                expression = expression.And(poll => poll.Widget == widget);
            }

            return await (await GetMongoQueryableAsync(GetCancellationToken(cancellationToken), null)).FirstOrDefaultAsync(expression, GetCancellationToken(cancellationToken));
        }
    }
}
