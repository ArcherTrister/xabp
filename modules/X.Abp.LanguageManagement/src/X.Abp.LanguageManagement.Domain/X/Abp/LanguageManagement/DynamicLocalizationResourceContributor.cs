// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using Volo.Abp.Localization;

namespace X.Abp.LanguageManagement;

public class DynamicLocalizationResourceContributor : ILocalizationResourceContributor
{
  public bool IsDynamic => true;

  protected LocalizationResourceBase Resource { get; private set; }

  protected IDynamicResourceLocalizer DynamicResourceLocalizer { get; set; }

  public void Initialize(LocalizationResourceInitializationContext context)
  {
    Resource = context.Resource;
    DynamicResourceLocalizer = context.ServiceProvider.GetRequiredService<IDynamicResourceLocalizer>();
  }

  public LocalizedString GetOrNull(string cultureName, string name)
  {
    return DynamicResourceLocalizer.GetOrNull(Resource, cultureName, name);
  }

  public void Fill(string cultureName, Dictionary<string, LocalizedString> dictionary)
  {
    DynamicResourceLocalizer.Fill(Resource, cultureName, dictionary);
  }

  public virtual Task FillAsync(string cultureName, Dictionary<string, LocalizedString> dictionary)
  {
    return DynamicResourceLocalizer.FillAsync(Resource, cultureName, dictionary);
  }

  public virtual async Task<IEnumerable<string>> GetSupportedCulturesAsync()
  {
    return await Task.FromResult(Array.Empty<string>());
  }
}
