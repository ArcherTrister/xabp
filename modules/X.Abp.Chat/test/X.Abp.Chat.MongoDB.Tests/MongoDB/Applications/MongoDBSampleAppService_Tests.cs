using X.Abp.Chat.Samples;

using Xunit;

namespace X.Abp.Chat.MongoDB.Applications;

[Collection(MongoTestCollection.Name)]
public class MongoDBSampleAppService_Tests : SampleAppService_Tests<ChatMongoDbTestModule>
{

}
