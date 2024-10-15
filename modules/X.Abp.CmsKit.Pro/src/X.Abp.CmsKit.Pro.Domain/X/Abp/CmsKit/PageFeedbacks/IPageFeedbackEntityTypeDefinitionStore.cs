// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;
using Volo.CmsKit;

namespace X.Abp.CmsKit.PageFeedbacks
{
    public interface IPageFeedbackEntityTypeDefinitionStore : IEntityTypeDefinitionStore<PageFeedbackEntityTypeDefinition>, ITransientDependency
    {
        Task<PageFeedbackEntityTypeDefinitions> GetPageFeedbackEntityTypeDefinitionsAsync();
    }
}
