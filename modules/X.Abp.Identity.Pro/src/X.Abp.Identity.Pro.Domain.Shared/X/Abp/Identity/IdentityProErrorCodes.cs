// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Identity;

public class IdentityProErrorCodes
{
    public const string InvalidExternalLoginProvider = "Volo.Abp.Identity:010010";
    public const string ExternalLoginProviderAuthenticateFailed = "Volo.Abp.Identity:010011";
    public const string LocalUserAlreadyExists = "Volo.Abp.Identity:010012";
    public const string NoUserFoundInFile = "Volo.Abp.Identity:010013";
    public const string InvalidImportFileFormat = "Volo.Abp.Identity:010014";
    public const string MaximumUserCount = "Volo.Abp.Identity:MaximumUserCount";
    public const string PhoneNumberAlreadyInUse = "Volo.Abp.Identity:PhoneNumberAlreadyInUse";
}
