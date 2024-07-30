// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace X.Abp.Account.Public.Web.Impersonation;

/// <summary>
/// Support get access_token from the query string, It will get from the Authorization request header by default.
/// </summary>
public class AbpAccountImpersonationJwtBearerConfigureOptions : IPostConfigureOptions<JwtBearerOptions>
{
    public void PostConfigure(string name, JwtBearerOptions options)
    {
        options.Events ??= new JwtBearerEvents();

        var previousOnMessageReceived = options.Events.OnMessageReceived;
        options.Events.OnMessageReceived = receivedContext =>
        {
            var token = receivedContext.Request.Query[AbpAccountImpersonationConsts.AccessToken].ToString();
            if (!token.IsNullOrWhiteSpace())
            {
                receivedContext.Token = token;
            }

            return previousOnMessageReceived != null
                ? previousOnMessageReceived(receivedContext)
                : Task.CompletedTask;
        };
    }
}
