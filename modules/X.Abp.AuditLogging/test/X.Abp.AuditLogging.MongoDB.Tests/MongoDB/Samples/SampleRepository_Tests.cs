﻿using Xunit;
using Volo.Abp.AuditLogging.Samples;

namespace X.Abp.AuditLogging.MongoDB.Samples;

[Collection(MongoTestCollection.Name)]
public class SampleRepository_Tests : SampleRepository_Tests<AuditLoggingMongoDbTestModule>
{
    /* Don't write custom repository tests here, instead write to
     * the base class.
     * One exception can be some specific tests related to MongoDB.
     */
}
