using X.Abp.Chat.Samples;

using Xunit;

namespace X.Abp.Chat.MongoDB.Domains;

[Collection(MongoTestCollection.Name)]
public class MongoDBSampleDomain_Tests : SampleManager_Tests<ChatMongoDbTestModule>
{

}
