// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using Volo.Abp.DependencyInjection;

namespace X.Abp.CmsKit.Newsletters;

public class NewsletterPreferenceDefinitionStore :
ITransientDependency,
INewsletterPreferenceDefinitionStore
{
  protected NewsletterOptions NewsletterOptions { get; }

  public NewsletterPreferenceDefinitionStore(IOptions<NewsletterOptions> newsletterOptions)
  {
    NewsletterOptions = newsletterOptions.Value;
  }

  public virtual Task<List<NewsletterPreferenceDefinition>> GetNewslettersAsync()
  {
    return Task.FromResult(NewsletterOptions.Preferences.Values.ToList());
  }
}
