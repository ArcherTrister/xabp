// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;

using Microsoft.Extensions.Localization;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TextTemplating;
using Volo.Abp.UI.Navigation.Urls;

using X.Abp.Account.Emailing.Templates;
using X.Abp.Account.Localization;

namespace X.Abp.Account.Emailing;

public class AccountEmailer : IAccountEmailer, ITransientDependency
{
  protected ITemplateRenderer TemplateRenderer { get; }

  protected IEmailSender EmailSender { get; }

  protected IStringLocalizer<AccountResource> StringLocalizer { get; }

  protected IAppUrlProvider AppUrlProvider { get; }

  protected ICurrentTenant CurrentTenant { get; }

  public AccountEmailer(
      IEmailSender emailSender,
      ITemplateRenderer templateRenderer,
      IStringLocalizer<AccountResource> stringLocalizer,
      IAppUrlProvider appUrlProvider,
      ICurrentTenant currentTenant)
  {
    EmailSender = emailSender;
    StringLocalizer = stringLocalizer;
    AppUrlProvider = appUrlProvider;
    CurrentTenant = currentTenant;
    TemplateRenderer = templateRenderer;
  }

  public virtual async Task SendPasswordResetLinkAsync(
      IdentityUser user,
      string resetToken,
      string appName,
      string returnUrl = null,
      string returnUrlHash = null)
  {
    Debug.Assert(CurrentTenant.Id == user.TenantId, "This method can only work for current tenant!");

    var url = await AppUrlProvider.GetResetPasswordUrlAsync(appName);

    // TODO: Use AbpAspNetCoreMultiTenancyOptions to get the key
    var link = $"{url}?userId={user.Id}&{TenantResolverConsts.DefaultTenantKey}={user.TenantId}&resetToken={UrlEncoder.Default.Encode(resetToken)}";

    if (!returnUrl.IsNullOrEmpty())
    {
      link += "&returnUrl=" + NormalizeReturnUrl(returnUrl);
    }

    if (!returnUrlHash.IsNullOrEmpty())
    {
      link += "&returnUrlHash=" + returnUrlHash;
    }

    var emailContent = await TemplateRenderer.RenderAsync(
        AccountEmailTemplates.PasswordResetLink,
        new { link });

    await EmailSender.SendAsync(
        user.Email,
        StringLocalizer["PasswordReset"],
        emailContent);
  }

  public virtual async Task SendEmailConfirmationLinkAsync(
      IdentityUser user,
      string confirmationToken,
      string appName,
      string returnUrl = null,
      string returnUrlHash = null)
  {
    Debug.Assert(CurrentTenant.Id == user.TenantId, "This method can only work for current tenant!");

    var url = await AppUrlProvider.GetEmailConfirmationUrlAsync(appName);

    // TODO: Use AbpAspNetCoreMultiTenancyOptions to get the key
    var link = $"{url}?userId={user.Id}&{TenantResolverConsts.DefaultTenantKey}={user.TenantId}&confirmationToken={UrlEncoder.Default.Encode(confirmationToken)}";

    if (!returnUrl.IsNullOrEmpty())
    {
      link += "&returnUrl=" + NormalizeReturnUrl(returnUrl);
    }

    if (!returnUrlHash.IsNullOrEmpty())
    {
      link += "&returnUrlHash=" + returnUrlHash;
    }

    var emailContent = await TemplateRenderer.RenderAsync(
        AccountEmailTemplates.EmailConfirmationLink,
        new { link });

    await EmailSender.SendAsync(
        user.Email,
        StringLocalizer["EmailConfirmation"],
        emailContent);
  }

  public virtual async Task SendEmailSecurityCodeAsync(IdentityUser user, string code)
  {
    var emailContent = await TemplateRenderer.RenderAsync(
        AccountEmailTemplates.EmailSecurityCode,
        new { code });

    await EmailSender.SendAsync(
        user.Email,
        StringLocalizer["EmailSecurityCodeSubject"],
        emailContent);
  }

  private static string NormalizeReturnUrl(string returnUrl)
  {
    if (returnUrl.IsNullOrEmpty())
    {
      return returnUrl;
    }

    // Handling openid connect login
    if (returnUrl.StartsWith("/connect/authorize/callback", StringComparison.InvariantCultureIgnoreCase))
    {
      if (returnUrl.Contains('?', StringComparison.Ordinal))
      {
        var queryPart = returnUrl.Split('?')[1];
        var queryParameters = queryPart.Split('&');
        foreach (var queryParameter in queryParameters)
        {
          if (queryParameter.Contains('=', StringComparison.Ordinal))
          {
            var queryParam = queryParameter.Split('=');
            if (queryParam[0] == "redirect_uri")
            {
              return HttpUtility.UrlDecode(queryParam[1]);
            }
          }
        }
      }
    }

    return returnUrl.StartsWith("/connect/authorize?", StringComparison.InvariantCultureIgnoreCase)
        ? HttpUtility.UrlEncode(returnUrl)
        : returnUrl;
  }
}
