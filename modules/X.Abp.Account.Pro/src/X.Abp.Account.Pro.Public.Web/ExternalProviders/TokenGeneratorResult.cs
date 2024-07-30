// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Account.Public.Web.ExternalProviders;

public class TokenGeneratorResult
{
    public string AccessToken { get; protected set; }

    public int ExpiresIn { get; protected set; }

    public string TokenType { get; protected set; }

    public string RefreshToken { get; protected set; }

    public string Scope { get; protected set; }

    public bool IsError { get; protected set; }

    public string Error { get; protected set; }

    protected TokenGeneratorResult()
    {
    }

    public TokenGeneratorResult(string error)
    {
        Error = error;
        IsError = true;
    }

    public TokenGeneratorResult(string accessToken, int expiresIn, string tokenType, string scope, string refreshToken)
    {
        AccessToken = accessToken;
        ExpiresIn = expiresIn;
        TokenType = tokenType;
        Scope = scope;
        RefreshToken = refreshToken;
        IsError = false;
    }
}
