// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.CmsKit;

namespace X.Abp.CmsKit.PageFeedbacks
{
    public class DefaultPageFeedbackEntityTypeDefinitionStore : IEntityTypeDefinitionStore<PageFeedbackEntityTypeDefinition>, ITransientDependency, IPageFeedbackEntityTypeDefinitionStore
    {
        protected virtual CmsKitPageFeedbackOptions Options { get; }

        public DefaultPageFeedbackEntityTypeDefinitionStore(IOptions<CmsKitPageFeedbackOptions> options) => Options = options.Value;

        public virtual Task<PageFeedbackEntityTypeDefinitions> GetPageFeedbackEntityTypeDefinitionsAsync() => Task.FromResult(Options.EntityTypes);

        public virtual Task<PageFeedbackEntityTypeDefinition> GetAsync(string entityType)
        {
            Check.NotNullOrWhiteSpace(entityType, nameof(entityType), PageFeedbackConst.MaxEntityTypeLength);
            return Task.FromResult(Options.EntityTypes.SingleOrDefault(x => x.EntityType == entityType) ?? throw new EntityCantHavePageFeedbackException(entityType));
        }

        public virtual Task<bool> IsDefinedAsync(string entityType)
        {
            Check.NotNullOrWhiteSpace(entityType, nameof(entityType), PageFeedbackConst.MaxEntityTypeLength);
            return Task.FromResult(Options.EntityTypes.Any(x => x.EntityType == entityType));
        }
    }
}
