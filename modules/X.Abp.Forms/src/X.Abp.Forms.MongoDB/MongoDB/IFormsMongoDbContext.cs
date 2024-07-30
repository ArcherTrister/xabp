using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace X.Abp.Forms.MongoDB;

[ConnectionStringName(FormsDbProperties.ConnectionStringName)]
public interface IFormsMongoDbContext : IAbpMongoDbContext
{
    /* Define mongo collections here. Example:
     * IMongoCollection<Question> Questions { get; }
     */
}
