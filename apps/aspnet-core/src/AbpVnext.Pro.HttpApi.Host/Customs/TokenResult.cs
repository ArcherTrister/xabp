// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using IdentityModel;

using IdentityServer4.Extensions;
using IdentityServer4.Hosting;
using IdentityServer4.ResponseHandling;

using Microsoft.AspNetCore.Http;

namespace AbpVnext.Pro.Customs;

internal class TokenResult : IEndpointResult
{
    public TokenResponse Response { get; set; }

    public TokenResult(TokenResponse response)
    {
        Response = response ?? throw new ArgumentNullException(nameof(response));
    }

    public async Task ExecuteAsync(HttpContext context)
    {
        context.Response.SetNoCache();

        var dto = new ResultDto
        {
            Id_token = Response.IdentityToken,
            Access_token = Response.AccessToken,
            Refresh_token = Response.RefreshToken,
            Expires_in = Response.AccessTokenLifetime,
            Token_type = OidcConstants.TokenResponse.BearerTokenType,
            Scope = Response.Scope,

            Custom = Response.Custom
        };

        await context.Response.WriteJsonAsync(dto);
    }

    internal class ResultDto
    {
        public string Id_token { get; set; }

        public string Access_token { get; set; }

        public int Expires_in { get; set; }

        public string Token_type { get; set; }

        public string Refresh_token { get; set; }

        public string Scope { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> Custom { get; set; }
    }
}
