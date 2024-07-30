// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Identity.Settings;

public static class IdentityProSettingNames
{
    private const string Prefix = "Abp.Identity";

    public const string EnableLdapLogin = "Abp.Account.EnableLdapLogin";

    public const string EnableOAuthLogin = Prefix + ".EnableOAuthLogin";

    public static class TwoFactor
    {
        private const string TwoFactorPrefix = Prefix + ".TwoFactor";

        public const string Behaviour = TwoFactorPrefix + ".Behaviour";

        public const string UsersCanChange = TwoFactorPrefix + ".UsersCanChange";
    }

    public static class OAuthLogin
    {
        private const string OAuthLoginPrefix = Prefix + ".OAuthLogin";

        public const string ClientId = OAuthLoginPrefix + ".ClientId";

        public const string ClientSecret = OAuthLoginPrefix + ".ClientSecret";

        public const string Scope = OAuthLoginPrefix + ".Scope";

        public const string Authority = OAuthLoginPrefix + ".Authority";

        public const string RequireHttpsMetadata = OAuthLoginPrefix + ".RequireHttpsMetadata";
    }
}
