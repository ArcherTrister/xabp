﻿// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

using X.Abp.CmsKit.Pro.MongoDB;

namespace X.Abp.CmsKit.Polls
{
    public class MongoPollUserVoteRepository : MongoDbRepository<ICmsKitProMongoDbContext, PollUserVote, Guid>, IPollUserVoteRepository
    {
        public MongoPollUserVoteRepository(IMongoDbContextProvider<ICmsKitProMongoDbContext> dbContextProvider)
          : base(dbContextProvider)
        {
        }
    }
}
