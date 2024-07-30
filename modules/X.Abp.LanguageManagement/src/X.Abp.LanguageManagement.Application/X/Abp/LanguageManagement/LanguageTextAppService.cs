// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Localization.External;

using X.Abp.LanguageManagement.Dto;
using X.Abp.LanguageManagement.Permissions;

namespace X.Abp.LanguageManagement;

[RequiresFeature(LanguageManagementFeatures.Enable)]
[Authorize(AbpLanguageManagementPermissions.LanguageTexts.Default)]
public class LanguageTextAppService : LanguageAppServiceBase, ILanguageTextAppService
{
    protected ILanguageTextRepository LanguageTextRepository { get; }

    protected IStringLocalizerFactory LocalizerFactory { get; }

    protected AbpLocalizationOptions AbpLocalizationOptions { get; }

    protected IExternalLocalizationStore ExternalLocalizationStore { get; }

    public LanguageTextAppService(
      ILanguageTextRepository languageTextRepository,
      IOptions<AbpLocalizationOptions> abpLocalizationOptions,
      IStringLocalizerFactory localizerFactory,
      IExternalLocalizationStore externalLocalizationStore)
    {
        LanguageTextRepository = languageTextRepository;
        LocalizerFactory = localizerFactory;
        ExternalLocalizationStore = externalLocalizationStore;
        AbpLocalizationOptions = abpLocalizationOptions.Value;
    }

    public virtual async Task<PagedResultDto<LanguageTextDto>> GetListAsync(
      GetLanguagesTextsInput input)
    {
        if (!CultureHelper.IsValidCultureCode(input.BaseCultureName) || !CultureHelper.IsValidCultureCode(input.TargetCultureName))
        {
            throw new AbpException("The selected culture is not valid! Make sure you enter a valid culture code.");
        }

        var languageTexts = new List<LanguageTextDto>();

        foreach (var resourceName in await GetResourceNamesAsync(input))
        {
            languageTexts.AddRange(await GetLocalizationsAsync(input, resourceName));
        }

#pragma warning disable CA2249 // 请考虑使用 "string.Contains" 而不是 "string.IndexOf"
        var filteredQuery = languageTexts
            .AsQueryable()
            .WhereIf(
                !input.Filter.IsNullOrWhiteSpace(),
                l =>
                    (l.Name != null && l.Name.IndexOf(input.Filter, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (l.BaseValue != null && l.BaseValue.IndexOf(input.Filter, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!input.GetOnlyEmptyValues && l.Value != null && l.Value.IndexOf(input.Filter, StringComparison.InvariantCultureIgnoreCase) >= 0));
#pragma warning restore CA2249 // 请考虑使用 "string.Contains" 而不是 "string.IndexOf"

        var languagesTextDtos = filteredQuery
            .OrderBy(input.Sorting.IsNullOrWhiteSpace() ? nameof(LanguageTextDto.Name) : input.Sorting)
            .PageBy(input)
            .ToList();

        return new PagedResultDto<LanguageTextDto>(
            filteredQuery.Count(),
            languagesTextDtos);
    }

    public virtual async Task<LanguageTextDto> GetAsync(
      string resourceName,
      string cultureName,
      string name,
      string baseCultureName)
    {
        if (!CultureHelper.IsValidCultureCode(cultureName) || !CultureHelper.IsValidCultureCode(baseCultureName))
        {
            throw new AbpException("The selected culture is not valid! Make sure you enter a valid culture code.");
        }

        var localizer = await GetLocalizerAsync(resourceName);
        var languageTextDto = new LanguageTextDto()
        {
            Name = name,
            ResourceName = resourceName,
            CultureName = cultureName,
            BaseCultureName = baseCultureName
        };

        using (CultureHelper.Use(CultureInfo.GetCultureInfo(baseCultureName)))
        {
            languageTextDto.BaseValue = baseCultureName != null
                ? (await localizer.GetAllStringsAsync(false)).FirstOrDefault(lt => lt.Name == name)?.Value ?? string.Empty
                : string.Empty;
        }

        using (CultureHelper.Use(CultureInfo.GetCultureInfo(cultureName)))
        {
            languageTextDto.Value = (await localizer.GetAllStringsAsync(false)).FirstOrDefault(lt => lt.Name == name)?.Value ?? string.Empty;
        }

        return languageTextDto;
    }

    [Authorize(AbpLanguageManagementPermissions.LanguageTexts.Edit)]
    public virtual async Task UpdateAsync(
      string resourceName,
      string cultureName,
      string name,
      string value)
    {
        value ??= string.Empty;

        using (CultureHelper.Use(CultureInfo.GetCultureInfo(cultureName)))
        {
            var localizerValue = (await (await GetLocalizerAsync(resourceName))
                                     .GetAllStringsAsync(false))
                                     .FirstOrDefault(lt => lt.Name == name)?.Value ?? string.Empty;

            if (localizerValue == value)
            {
                return;
            }
        }

        var languageText = (await LanguageTextRepository.GetListAsync()).FirstOrDefault(l =>
            l.CultureName == cultureName && l.ResourceName == resourceName &&
            l.Name == name);

        if (languageText == null)
        {
            await LanguageTextRepository.InsertAsync(
                new LanguageText(GuidGenerator.Create(), resourceName, cultureName, name, value, CurrentTenant?.Id));
        }
        else
        {
            languageText.Value = value;
            await LanguageTextRepository.UpdateAsync(languageText);
        }
    }

    public virtual async Task RestoreToDefaultAsync(string resourceName, string cultureName, string name)
    {
        var languageText = (await LanguageTextRepository.GetListAsync(false)).FirstOrDefault(l => l.CultureName == cultureName && l.ResourceName == resourceName && l.Name == name);
        if (languageText == null)
        {
        }
        else
        {
            await LanguageTextRepository.DeleteAsync(languageText, false);
        }
    }

    protected virtual async Task<List<string>> GetResourceNamesAsync(
      GetLanguagesTextsInput input)
    {
        var resourceNames = new List<string>();
        if (string.IsNullOrWhiteSpace(input.ResourceName))
        {
            resourceNames.AddRange(AbpLocalizationOptions.Resources.Values.Select(l => l.ResourceName));
            resourceNames.AddIfNotContains(await ExternalLocalizationStore.GetResourceNamesAsync());
        }
        else
        {
            resourceNames.Add(input.ResourceName);
        }

        resourceNames.Sort();

        return resourceNames;
    }

    protected virtual async Task<List<LanguageTextDto>> GetLocalizationsAsync(GetLanguagesTextsInput input, string resourceName)
    {
        var localizer = await GetLocalizerOrNullAsync(resourceName);
        if (localizer == null)
        {
            return new List<LanguageTextDto>();
        }

        var baseLocalizedStrings = new List<LocalizedString>();
        using (CultureHelper.Use(CultureInfo.GetCultureInfo(input.BaseCultureName), null))
        {
            baseLocalizedStrings = (await localizer.GetAllStringsAsync(true, false, true)).ToList();
        }

        using (CultureHelper.Use(CultureInfo.GetCultureInfo(input.TargetCultureName), null))
        {
            var list = (await localizer.GetAllStringsAsync(false, false, true)).ToList();
            var localizations = new List<LanguageTextDto>();
            foreach (var baseLocalizedString in baseLocalizedStrings)
            {
                var localizedString = list.FirstOrDefault(l => l.Name == baseLocalizedString.Name);
                if (!input.GetOnlyEmptyValues || string.IsNullOrEmpty(localizedString?.Value))
                {
                    var languageTextDto = new LanguageTextDto
                    {
                        BaseCultureName = input.BaseCultureName,
                        CultureName = input.TargetCultureName,
                        Name = baseLocalizedString.Name,
                        BaseValue = baseLocalizedString.Value,
                        ResourceName = resourceName
                    };
                    if (localizedString != null && localizedString.Value != null)
                    {
                        languageTextDto.Value = localizedString.Value;
                        localizations.Add(languageTextDto);
                    }
                }
            }

            return localizations;
        }
    }

    protected virtual Task<IStringLocalizer> GetLocalizerAsync(LocalizationResourceBase resource)
    {
        return LocalizerFactory.CreateByResourceNameAsync(resource.ResourceName);
    }

    protected virtual async Task<IStringLocalizer> GetLocalizerAsync(string resourceName)
    {
        return await GetLocalizerAsync(await GetLocalizationResourceAsync(resourceName));
    }

    protected virtual async Task<IStringLocalizer> GetLocalizerOrNullAsync(string resourceName)
    {
        var resource = await GetLocalizationResourceOrNullAsync(resourceName);
        return resource == null ? null : await GetLocalizerAsync(resource);
    }

    protected virtual async Task<LocalizationResourceBase> GetLocalizationResourceAsync(string resourceName)
    {
        return await GetLocalizationResourceOrNullAsync(resourceName) ?? throw new AbpException("Resource not found: " + resourceName);
    }

    protected virtual async Task<LocalizationResourceBase> GetLocalizationResourceOrNullAsync(string resourceName)
    {
        var resourceOrNullAsync = AbpLocalizationOptions.Resources.GetOrDefault(resourceName);
        resourceOrNullAsync ??= await ExternalLocalizationStore.GetResourceOrNullAsync(resourceName);

        return resourceOrNullAsync;
    }
}
