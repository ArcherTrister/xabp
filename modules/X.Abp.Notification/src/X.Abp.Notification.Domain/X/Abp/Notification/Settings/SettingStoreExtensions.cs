// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Volo.Abp.Settings;

namespace X.Abp.Notification.Settings;
public static class SettingStoreExtensions
{
    public static async Task<T> GetSettingValueForUserAsync<T>(this ISettingStore settingStore, string name, Guid userId)
           where T : struct
    {
        var value = await settingStore.GetOrNullAsync(name, UserSettingValueProvider.ProviderName, userId.ToString());
        if (value.IsNullOrWhiteSpace())
        {
            return default(T);
        }

        return value.To<T>();
    }
}
