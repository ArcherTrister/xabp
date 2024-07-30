// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Packages.SignalR;
using Volo.Abp.Features;
using Volo.Abp.Modularity;

using X.Abp.Chat.Features;

namespace X.Abp.Chat.Web.Bundling;

[DependsOn(
    typeof(SignalRBrowserScriptContributor))]
public class ChatGlobalScriptContributor : BundleContributor
{
    public override async Task ConfigureBundleAsync(BundleConfigurationContext context)
    {
        var featureChecker = context.ServiceProvider.GetService<IFeatureChecker>();

        if (await featureChecker.IsEnabledAsync(AbpChatFeatures.Enable))
        {
            context.Files.AddIfNotContains("/Pages/Chat/chatMessageReceiving.js");
        }
    }
}
