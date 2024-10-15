using Volo.Abp.Data;
using Volo.Abp.MongoDB;
using Volo.Abp.OpenIddict;

namespace X.Abp.OpenIddict.MongoDB;

[ConnectionStringName(AbpOpenIddictDbProperties.ConnectionStringName)]
public class OpenIddictProMongoDbContext : AbpMongoDbContext, IOpenIddictProMongoDbContext
{
    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        modelBuilder.ConfigureOpenIddictPro();
    }
}
