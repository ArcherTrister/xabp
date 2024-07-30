// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

namespace X.Abp.Account.Dtos;
public class RemoveLoginInput
{
    [Required]
    public string LoginProvider { get; set; }

    [Required]
    public string ProviderKey { get; set; }
}
