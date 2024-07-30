using Volo.Abp.TextTemplateManagement.Samples;
using Xunit;

namespace X.Abp.TextTemplateManagement.MongoDB.Samples;

[Collection(MongoTestCollection.Name)]
public class SampleRepository_Tests : SampleRepository_Tests<TextTemplateManagementMongoDbTestModule>
{
    /* Don't write custom repository tests here, instead write to
     * the base class.
     * One exception can be some specific tests related to MongoDB.
     */
}
