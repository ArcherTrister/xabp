// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Account.Dtos;

public class TwoFactorAuthenticationDto
{
    public bool HasAuthenticator { get; set; }

    public bool Is2faEnabled { get; set; }

    public bool CanEnableTwoFactor { get; set; }

    /// <summary>
    /// RecoveryCodesLeft
    /// </summary>
    /// <remarks>
    /// if (Model.RecoveryCodesLeft == 0)
    /// {
    ///    <div class="alert alert-danger">
    ///        <strong>You have no recovery codes left.</strong>
    ///        <p>You must <a asp-page= "./GenerateRecoveryCodes" > generate a new set of recovery codes</a> before you can log in with a recovery code.</p>
    ///    </div>
    /// }
    /// else if (Model.RecoveryCodesLeft == 1)
    /// {
    ///    <div class="alert alert-danger">
    ///        <strong>You have 1 recovery code left.</strong>
    ///        <p>You can<a asp-page="./GenerateRecoveryCodes">generate a new set of recovery codes</a>.</p>
    ///    </div>
    /// }
    /// else if (Model.RecoveryCodesLeft %3C= 3)
    /// {
    ///     <div class= "alert alert-warning" >
    ///         <strong> You have @Model.RecoveryCodesLeft recovery codes left.</strong>
    ///         <p>You should <a asp-page="./GenerateRecoveryCodes">generate a new set of recovery codes</a>.</p>
    ///     </div>
    /// }
    /// </remarks>
    public int RecoveryCodesLeft { get; set; }
}
