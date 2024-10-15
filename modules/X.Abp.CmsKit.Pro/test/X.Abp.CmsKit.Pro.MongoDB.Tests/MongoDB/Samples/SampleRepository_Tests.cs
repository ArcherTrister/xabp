using X.Abp.CmsKit.Pro.Samples;
using Xunit;

namespace X.Abp.CmsKit.Pro.MongoDB.Samples;

[Collection(MongoTestCollection.Name)]
public class SampleRepository_Tests : SampleRepository_Tests<CmsKitProMongoDbTestModule>
{
    /* Don't write custom repository tests here, instead write to
     * the base class.
     * One exception can be some specific tests related to MongoDB.
     */
}
