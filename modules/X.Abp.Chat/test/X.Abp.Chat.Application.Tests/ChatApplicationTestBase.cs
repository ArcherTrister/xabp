using Volo.Abp.Modularity;

namespace X.Abp.Chat;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class ChatApplicationTestBase<TStartupModule> : ChatTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
