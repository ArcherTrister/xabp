// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;

using Microsoft.Extensions.Localization;

using Volo.Abp;
using Volo.Abp.Text.Formatting;

using X.Abp.Identity;

namespace Microsoft.AspNetCore.Identity;

public static class AbpIdentityResultExtensions
{
    private static readonly Dictionary<string, string> IdentityStrings = new();

    static AbpIdentityResultExtensions()
    {
        var identityResourceManager = new ResourceManager("Microsoft.Extensions.Identity.Core.Resources", typeof(UserManager<>).Assembly);
        var resourceSet = identityResourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, false);
        if (resourceSet == null)
        {
#pragma warning disable CA1065 // 不要在意外的位置引发异常
            throw new AbpException("Can't get the ResourceSet of Identity.");
#pragma warning restore CA1065 // 不要在意外的位置引发异常
        }

        var iterator = resourceSet.GetEnumerator();
        while (true)
        {
            if (!iterator.MoveNext())
            {
                break;
            }

            var key = iterator.Key?.ToString();
            var value = iterator.Value?.ToString();
            if (key != null && value != null)
            {
                IdentityStrings.Add(key, value);
            }
        }

        if (IdentityStrings.Count == 0)
        {
#pragma warning disable CA1065 // 不要在意外的位置引发异常
            throw new AbpException("ResourceSet values of Identity is empty.");
#pragma warning restore CA1065 // 不要在意外的位置引发异常
        }
    }

    public static void CheckIdentityErrors(this IdentityResult identityResult)
    {
        if (identityResult.Succeeded)
        {
            return;
        }

        if (identityResult.Errors == null)
        {
            throw new ArgumentException("identityResult.Errors should not be null.");
        }

        throw new XAbpIdentityResultException(identityResult);
    }

    public static string[] GetValuesFromErrorMessage(this IdentityResult identityResult, IStringLocalizer localizer)
    {
        if (identityResult.Succeeded)
        {
            throw new ArgumentException(
                "identityResult.Succeeded should be false in order to get values from error.");
        }

        if (identityResult.Errors == null)
        {
            throw new ArgumentException("identityResult.Errors should not be null.");
        }

        var error = identityResult.Errors.First();
        var englishString = IdentityStrings.GetOrDefault(error.Code);

        return englishString == null
            ? Array.Empty<string>()
            : FormattedStringValueExtracter.IsMatch(error.Description, englishString, out var values) ? values : Array.Empty<string>();
    }

    public static string LocalizeIdentityErrors(this IdentityResult identityResult, IStringLocalizer localizer)
    {
        return identityResult.Succeeded
            ? throw new ArgumentException("identityResult.Succeeded should be false in order to localize errors.")
            : identityResult.Errors == null
            ? throw new ArgumentException("identityResult.Errors should not be null.")
            : identityResult.Errors.Select(err => LocalizeIdentityErrorMessage(err, localizer)).JoinAsString(", ");
    }

    public static string LocalizeIdentityErrorMessage(this IdentityError error, IStringLocalizer localizer)
    {
        var key = $"Volo.Abp.Identity:{error.Code}";

        var localizedString = localizer[key];

        if (!localizedString.ResourceNotFound)
        {
            var englishString = IdentityStrings.GetOrDefault(error.Code);
            if (englishString != null)
            {
                if (FormattedStringValueExtracter.IsMatch(error.Description, englishString, out var values))
                {
                    return string.Format(localizedString.Value, values.Cast<object>().ToArray());
                }
            }
        }

        // return localizer["Identity.Default"];
        return error.Description;
    }

    public static string GetResultAsString(this SignInResult signInResult)
    {
        return signInResult.Succeeded
            ? "Succeeded"
            : signInResult.IsLockedOut
            ? "IsLockedOut"
            : signInResult.IsNotAllowed ? "IsNotAllowed" : signInResult.RequiresTwoFactor ? "RequiresTwoFactor" : "Unknown";
    }
}
