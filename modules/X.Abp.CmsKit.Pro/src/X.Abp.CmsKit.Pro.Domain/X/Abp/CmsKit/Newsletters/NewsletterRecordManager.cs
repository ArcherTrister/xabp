// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Volo.Abp;

namespace X.Abp.CmsKit.Newsletters;

public class NewsletterRecordManager : CmsKitProDomainServiceBase
{
    protected INewsletterRecordRepository NewsletterRecordsRepository { get; }

    protected INewsletterPreferenceDefinitionStore NewsletterPreferenceDefinitionStore { get; }

    public NewsletterRecordManager(
      INewsletterRecordRepository newsletterRecordsRepository,
      INewsletterPreferenceDefinitionStore newsletterPreferenceDefinitionStore)
    {
        NewsletterRecordsRepository = newsletterRecordsRepository;
        NewsletterPreferenceDefinitionStore = newsletterPreferenceDefinitionStore;
    }

    public virtual async Task<List<NewsletterPreferenceDefinition>> GetNewsletterPreferencesAsync()
    {
        return await NewsletterPreferenceDefinitionStore.GetNewslettersAsync();
    }

    public virtual async Task<NewsletterRecord> CreateOrUpdateAsync(
      string emailAddress,
      string preference,
      string source,
      string sourceUrl,
      ICollection<string> additionalPreferences)
    {
        Check.NotNullOrWhiteSpace(emailAddress, nameof(emailAddress));
        Check.NotNullOrWhiteSpace(preference, nameof(preference));
        Check.NotNullOrWhiteSpace(source, nameof(source));
        Check.NotNullOrWhiteSpace(sourceUrl, nameof(sourceUrl));
        additionalPreferences ??= new List<string>();

        var newsletterRecord = await NewsletterRecordsRepository.FindByEmailAddressAsync(emailAddress);
        if (newsletterRecord == null)
        {
            newsletterRecord = new NewsletterRecord(GuidGenerator.Create(), emailAddress, CurrentTenant.Id);
            newsletterRecord.AddPreferences(new NewsletterPreference(GuidGenerator.Create(), newsletterRecord.Id, preference, source, sourceUrl, CurrentTenant.Id));
            foreach (var additionalPreference in additionalPreferences)
            {
                newsletterRecord.AddPreferences(new NewsletterPreference(GuidGenerator.Create(), newsletterRecord.Id, additionalPreference, source, sourceUrl, CurrentTenant.Id));
            }

            return await NewsletterRecordsRepository.InsertAsync(newsletterRecord, false);
        }

        if (newsletterRecord.Preferences.FirstOrDefault(x => x.Preference == preference) == null)
        {
            newsletterRecord = newsletterRecord.AddPreferences(new NewsletterPreference(GuidGenerator.Create(), newsletterRecord.Id, preference, source, sourceUrl, CurrentTenant.Id));
            foreach (var additionalPreference in additionalPreferences)
            {
                newsletterRecord.AddPreferences(new NewsletterPreference(GuidGenerator.Create(), newsletterRecord.Id, additionalPreference, source, sourceUrl, CurrentTenant.Id));
            }

            return await NewsletterRecordsRepository.UpdateAsync(newsletterRecord, false);
        }

        foreach (var additionalPreference in additionalPreferences)
        {
            if (!newsletterRecord.Preferences.Any(x => x.Preference == additionalPreference))
            {
                newsletterRecord.AddPreferences(new NewsletterPreference(GuidGenerator.Create(), newsletterRecord.Id, additionalPreference, source, sourceUrl, CurrentTenant.Id));
            }
        }

        return await NewsletterRecordsRepository.UpdateAsync(newsletterRecord, false);
    }
}
