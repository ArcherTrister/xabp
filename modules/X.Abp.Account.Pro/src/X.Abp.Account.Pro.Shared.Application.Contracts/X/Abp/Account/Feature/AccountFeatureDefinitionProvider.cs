// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Features;
using Volo.Abp.Localization;

using X.Abp.Account.Localization;

namespace X.Abp.Account.Feature;

public class AccountFeatureDefinitionProvider : FeatureDefinitionProvider
{
    public override void Define(IFeatureDefinitionContext context)
    {
        context.AddGroup(AccountFeature.GroupName, L("Feature:AccountGroup"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AccountResource>(name);
    }
}
