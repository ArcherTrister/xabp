// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Data;
using Volo.Abp.Identity;
using Volo.Abp.Identity.MongoDB;

namespace X.Abp.Identity.MongoDB;

[ConnectionStringName(AbpIdentityDbProperties.ConnectionStringName)]
public interface IIdentityProMongoDbContext : IAbpIdentityMongoDbContext
{
    /* Define mongo collections here. Example:
     * IMongoCollection<Question> Questions { get; }
     */
}
