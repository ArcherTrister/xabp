// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using IdentityModel;
using IdentityModel.Client;

using X.Abp.Account.Web.ExtensionGrantValidators;

namespace X.Abp.Account.Web.Extensions;

public static class HttpClientTokenRequestExtensions
{
    /// <summary>
    /// Sends a token request using the SpaExternalLogin grant type.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task<TokenResponse> RequestSpaExternalLoginTokenAsync(this HttpMessageInvoker client, SpaExternalLoginTokenRequest request, CancellationToken cancellationToken = default)
    {
        var clone = request.Clone();

        clone.Parameters.AddRequired(OidcConstants.TokenRequest.GrantType, SpaExternalLoginExtensionGrantValidator.ExtensionGrantType);
        clone.Parameters.AddOptional(OidcConstants.TokenRequest.Scope, request.Scope);
        clone.Parameters.AddRequired(ExternalLoginConsts.SpaExternalLoginProviderName, request.LoginProvider);
        clone.Parameters.AddRequired(ExternalLoginConsts.SpaExternalLoginProviderKey, request.ProviderKey);
        clone.Parameters.AddRequired(ExternalLoginConsts.SpaExternalLoginTenantId, request.TenantId?.ToString(), allowEmptyValue: true);

        return await client.RequestTokenAsync(clone, cancellationToken);
    }

    public static async Task<TokenResponse> RequestScanCodeLoginTokenAsync(this HttpMessageInvoker client, ScanCodeLoginTokenRequest request, CancellationToken cancellationToken = default)
    {
        var clone = request.Clone();

        clone.Parameters.AddRequired(OidcConstants.TokenRequest.GrantType, SpaExternalLoginExtensionGrantValidator.ExtensionGrantType);
        clone.Parameters.AddOptional(OidcConstants.TokenRequest.Scope, request.Scope);
        clone.Parameters.AddRequired(ExternalLoginConsts.ScanCodeLoginUserId, request.UserId.ToString());
        clone.Parameters.AddRequired(ExternalLoginConsts.ScanCodeLoginTenantId, request.TenantId?.ToString(), allowEmptyValue: true);

        return await client.RequestTokenAsync(clone, cancellationToken);
    }

    internal static async Task<TokenResponse> RequestTokenAsync(this HttpMessageInvoker client, ProtocolRequest request, CancellationToken cancellationToken = default)
    {
        request.Prepare();
        request.Method = HttpMethod.Post;

        HttpResponseMessage response;
        try
        {
            response = await client.SendAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            return ProtocolResponse.FromException<TokenResponse>(ex);
        }

        return await ProtocolResponse.FromHttpResponseAsync<TokenResponse>(response);
    }
}
