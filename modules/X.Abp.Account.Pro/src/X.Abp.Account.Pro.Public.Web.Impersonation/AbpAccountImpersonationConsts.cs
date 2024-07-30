// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Account.Public.Web.Impersonation;

public static class AbpAccountImpersonationConsts
{
    public const string Impersonation = "Impersonation";

    public const string ImpersonateUserEndpoint = "Account/ImpersonateUser";

    public const string ImpersonateTenantEndpoint = "Account/ImpersonateTenant";

    public const string BackToImpersonator = "BackToImpersonator";

    public const string BackToImpersonatorEndpoint = "Account/BackToImpersonator";

    public const string AccessToken = "access_token";

    public const string UserId = "userid";

    public const string TenantId = "tenantid";

    public const string DelegatedImpersonate = "DelegatedImpersonate";
    public const string DelegatedImpersonateEndpoint = "Account/DelegatedImpersonate";

    public const string TenantUserName = "tenantusername";
    public const string UserDelegationId = "userdelegationid";
    public const string ReturnUrl = "returnurl";
}
