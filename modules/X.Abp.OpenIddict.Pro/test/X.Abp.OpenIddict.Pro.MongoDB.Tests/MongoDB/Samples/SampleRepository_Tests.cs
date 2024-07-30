using Volo.Abp.OpenIddict.Pro.Samples;
using Xunit;

namespace X.Abp.OpenIddict.Pro.MongoDB.Samples;

[Collection(MongoTestCollection.Name)]
public class SampleRepository_Tests : SampleRepository_Tests<ProMongoDbTestModule>
{
    /* Don't write custom repository tests here, instead write to
     * the base class.
     * One exception can be some specific tests related to MongoDB.
     */
}
