// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Volo.Abp.Domain.Entities;

namespace X.Abp.CmsKit.Polls;

public class PollManager : CmsKitProDomainServiceBase
{
    protected IPollRepository PollRepository { get; }

    protected IPollUserVoteRepository PollUserVoteRepository { get; }

    public PollManager(
      IPollRepository pollRepository,
      IPollUserVoteRepository pollUserVoteRepository)
    {
        PollRepository = pollRepository;
        PollUserVoteRepository = pollUserVoteRepository;
    }

    public async Task SubmitVoteAsync(Guid id, Guid userId, Guid[] pollOptionIds)
    {
        var keyValuePair = (await PollRepository.GetPollWithPollUserVotesAsync(id)).First();
        var key = keyValuePair.Key;
        var pollUserVotes = keyValuePair.Value;
        var insertPollUserVotes = new List<PollUserVote>();
        if (key.AllowMultipleVote)
        {
            foreach (var pollOptionId in pollOptionIds)
            {
                Create(userId, key, insertPollUserVotes, pollOptionId, pollUserVotes);
            }
        }
        else
        {
            var pollOptionId = pollOptionIds.Length == 1 ? pollOptionIds.First() : throw new PollAllowSingleVoteException(key.AllowMultipleVote);
            Create(userId, key, insertPollUserVotes, pollOptionId, pollUserVotes);
        }

        key.Increase();
        await PollUserVoteRepository.InsertManyAsync(insertPollUserVotes, false);
    }

    private void Create(
      Guid userId,
      Poll poll,
      List<PollUserVote> insertPollUserVotes,
      Guid pollOptionId,
      List<PollUserVote> pollUserVotes)
    {
        if (!poll.PollOptions.Any(p => p.Id == pollOptionId))
        {
            throw new EntityNotFoundException(typeof(Guid), pollOptionId);
        }

        if (pollUserVotes.Any(p => p.UserId == userId && p.PollId == poll.Id && p.PollOptionId == pollOptionId))
        {
            throw new PollUserVoteVotedBySameUserException(userId, poll.Id, pollOptionId);
        }

        insertPollUserVotes.Add(new PollUserVote(GuidGenerator.Create(), poll.Id, userId, pollOptionId, poll.TenantId));
        poll.PollOptions.First(p => p.Id == pollOptionId).Increase();
    }

    public async Task EnsureExistAsync(string code)
    {
        if (await PollRepository.FindByCodeAsync(code) != null)
        {
            throw new PollHasAlreadySameCodeException(code);
        }
    }
}
