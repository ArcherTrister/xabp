// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.RequestLocalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace X.Abp.Localization;
public class AbpCultureMapRequestCultureProvider : RequestCultureProvider
{
    public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var option = httpContext.RequestServices.GetRequiredService<IOptions<AbpLocalizationCultureMapOptions>>().Value;

        var mapCultures = new List<StringSegment>();
        var mapUiCultures = new List<StringSegment>();

        var requestLocalizationOptionsProvider = httpContext.RequestServices.GetRequiredService<IAbpRequestLocalizationOptionsProvider>();
        foreach (var provider in (await requestLocalizationOptionsProvider.GetLocalizationOptionsAsync()).RequestCultureProviders)
        {
            if (provider == this)
            {
                continue;
            }

            var providerCultureResult = await provider.DetermineProviderCultureResult(httpContext);
            if (providerCultureResult == null)
            {
                continue;
            }

            mapCultures.AddRange(providerCultureResult.Cultures.Where(x => x.HasValue)
                .Select(culture =>
                {
                    var map = option.CulturesMaps.FirstOrDefault(x =>
                        x.SourceCultures.Contains(culture.Value, StringComparer.OrdinalIgnoreCase));
                    return new StringSegment(map?.TargetCulture ?? culture.Value);
                }));

            mapUiCultures.AddRange(providerCultureResult.UICultures.Where(x => x.HasValue)
                .Select(culture =>
                {
                    var map = option.UiCulturesMaps.FirstOrDefault(x =>
                        x.SourceCultures.Contains(culture.Value, StringComparer.OrdinalIgnoreCase));
                    return new StringSegment(map?.TargetCulture ?? culture.Value);
                }));
        }

        return new ProviderCultureResult(mapCultures, mapUiCultures);
    }
}
