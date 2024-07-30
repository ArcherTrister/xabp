// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Volo.Abp.Application.Services;
using Volo.Abp.Security.Encryption;

namespace X.Abp.Account.ExternalProviders;

public class AccountExternalProviderAppService : ApplicationService, IAccountExternalProviderAppService
{
    protected ExternalProviderSettingsHelper ExternalProviderSettingsHelper { get; }

    protected IStringEncryptionService StringEncryptionService { get; }

    public AccountExternalProviderAppService(
        ExternalProviderSettingsHelper externalProviderSettingsHelper,
        IStringEncryptionService stringEncryptionService)
    {
        ExternalProviderSettingsHelper = externalProviderSettingsHelper;
        StringEncryptionService = stringEncryptionService;
    }

    public virtual async Task<ExternalProviderDto> GetAllAsync()
    {
        return new ExternalProviderDto
        {
            Providers = ObjectMapper.Map<List<ExternalProviderSettings>, List<ExternalProviderItemDto>>(await ExternalProviderSettingsHelper.GetAllAsync())
        };
    }

    public virtual async Task<ExternalProviderItemWithSecretDto> GetByNameAsync(GetByNameInput input)
    {
        using (CurrentTenant.Change(input.TenantId))
        {
            var definition = ExternalProviderSettingsHelper.GetDefinitionsByNameOrNull(input.Name);
            if (definition != null)
            {
                var settings = await ExternalProviderSettingsHelper.GetByNameAsync(definition.Name, fallBackToHost: true);

                // Encrypt the secret values.
                settings.SecretProperties = settings.SecretProperties
                    .Select(secretValue => new ExternalProviderSettingsProperty(secretValue.Name, StringEncryptionService.Encrypt(secretValue.Value)))
                    .ToList();

                return ObjectMapper.Map<ExternalProviderSettings, ExternalProviderItemWithSecretDto>(settings);
            }

            return new ExternalProviderItemWithSecretDto
            {
                Success = false
            };
        }
    }
}
