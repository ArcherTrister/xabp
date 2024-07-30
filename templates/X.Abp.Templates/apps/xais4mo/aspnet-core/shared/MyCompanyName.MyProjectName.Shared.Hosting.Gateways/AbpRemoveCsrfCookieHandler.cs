using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;

namespace MyCompanyName.MyProjectName.Shared.Hosting.Gateways;

public class AbpRemoveCsrfCookieHandler : DelegatingHandler
{
    private const string CookieHeaderName = "Cookie";
    private readonly AbpAntiForgeryOptions _abpAntiForgeryOptions;

    public AbpRemoveCsrfCookieHandler(IOptions<AbpAntiForgeryOptions> abpAntiForgeryOptions)
    {
        _abpAntiForgeryOptions = abpAntiForgeryOptions.Value;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var authCookieName = _abpAntiForgeryOptions.AuthCookieSchemaName;
        var antiForgeryCookieName = _abpAntiForgeryOptions.TokenCookie.Name;

        if (request.Headers.TryGetValues(CookieHeaderName, out var cookies))
        {
            var newCookies = cookies.ToList();

            newCookies.RemoveAll(x =>
                !string.IsNullOrWhiteSpace(authCookieName) && x.Contains(authCookieName) ||
                !string.IsNullOrWhiteSpace(antiForgeryCookieName) && x.Contains(antiForgeryCookieName));

            request.Headers.Remove(CookieHeaderName);
            request.Headers.Add(CookieHeaderName, newCookies);

        }

        return base.SendAsync(request, cancellationToken);
    }
}
