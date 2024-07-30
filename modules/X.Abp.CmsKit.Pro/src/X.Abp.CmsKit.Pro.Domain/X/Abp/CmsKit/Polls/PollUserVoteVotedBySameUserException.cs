// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp;

namespace X.Abp.CmsKit.Polls;

[Serializable]
public class PollUserVoteVotedBySameUserException : BusinessException
{
    public PollUserVoteVotedBySameUserException(Guid userId, Guid pollId, Guid pollOptionId)
      : base(CmsKitProErrorCodes.Poll.PollUserVoteVotedBySameUser)
    {
        WithData("UserId", userId);
        WithData("PollId", pollId);
        WithData("PollOptionId", pollOptionId);
    }
}
