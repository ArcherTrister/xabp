# abp-cms-kit-pro

CmsKit Pro module for ABP framework.

Domain.Shared

XXXGlobalFeatureConfigurator

            GlobalFeatureManager.Instance.Modules.CmsKit(cmsKit =>
            {
                cmsKit.EnableAll();
            });
            GlobalFeatureManager.Instance.Modules.CmsKitPro(cmsKitPro =>
            {
                cmsKitPro.EnableAll();
            });

Domain

            Configure<NewsletterOptions>(options =>
            {
                options.AddPreference(
                    "Newsletter_Default",
                    new NewsletterPreferenceDefinition(
                        LocalizableString.Create<XXXResource>("NewsletterPreference_Default"),
                        privacyPolicyConfirmation: LocalizableString.Create<XXXResource>("NewsletterPrivacyAcceptMessage")
                    )
                );
            });
