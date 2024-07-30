// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.AuditLogging.Dtos;

public class AuditLogDtoCommonConsts
{
    /// <summary>
    /// Default value: 512
    /// </summary>
    public static int UrlFilterMaxLength { get; set; } = 512;

    /// <summary>
    /// Default value: 128
    /// </summary>
    public static int UserNameFilterMaxLength { get; set; } = 128;

    /// <summary>
    /// Default value: 16
    /// </summary>
    public static int HttpMethodFilterMaxLength { get; set; } = 16;
}
