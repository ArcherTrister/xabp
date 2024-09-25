// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Localization;

using Volo.Abp.OpenIddict;

namespace X.Abp.Account.Web.Pages.Account;

public class OpenIddictReturnUrlRequestCultureProvider : RequestCultureProvider
{
    private const string ReturnUrl = nameof(ReturnUrl);

    private const string QueryStringKey = "culture";

    private const string UIQueryStringKey = "ui-culture";

    public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext, nameof(httpContext));

        var request = httpContext.Request;
        if (request.QueryString.HasValue)
        {
            string text = request.Query[ReturnUrl];
            if (!text.IsNullOrWhiteSpace())
            {
                var openIddictRequestHelper = httpContext.RequestServices.GetService<AbpOpenIddictRequestHelper>();
                if (openIddictRequestHelper == null)
                {
                    return await NullProviderCultureResult;
                }

                var openIddictRequest = await openIddictRequestHelper.GetFromReturnUrlAsync(text);
                if (openIddictRequest != null)
                {
                    var queryCulture = openIddictRequest.GetParameter(QueryStringKey).ToString();
                    var queryUICulture = openIddictRequest.GetParameter(UIQueryStringKey).ToString();
                    if (queryCulture != null || queryUICulture != null)
                    {
                        if (queryCulture != null && queryUICulture == null)
                        {
                            queryUICulture = queryCulture;
                        }
                        else if (queryCulture == null && queryUICulture != null)
                        {
                            queryCulture = queryUICulture;
                        }

                        return new ProviderCultureResult(queryCulture, queryUICulture);
                    }

                    return await NullProviderCultureResult;
                }

                return await NullProviderCultureResult;
            }

            return await NullProviderCultureResult;
        }

        return await NullProviderCultureResult;
    }
}
