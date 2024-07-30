// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using IdentityServer4.Extensions;
using IdentityServer4.Hosting;
using IdentityServer4.ResponseHandling;

using Microsoft.AspNetCore.Http;

namespace AbpVnext.Pro.Customs;

internal class TokenErrorResult : IEndpointResult
{
    public TokenErrorResponse Response { get; }

    public TokenErrorResult(TokenErrorResponse error)
    {
        if (error.Error.IsMissing())
        {
            throw new ArgumentNullException(nameof(error.Error), "Error must be set");
        }

        Response = error;
    }

    public async Task ExecuteAsync(HttpContext context)
    {
        context.Response.StatusCode = 400;
        context.Response.SetNoCache();

        var dto = new ResultDto
        {
            Error = Response.Error,
            Error_description = Response.ErrorDescription,

            Custom = Response.Custom
        };

        await context.Response.WriteJsonAsync(dto);
    }

    internal class ResultDto
    {
        public string Error { get; set; }

        public string Error_description { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> Custom { get; set; }
    }
}
