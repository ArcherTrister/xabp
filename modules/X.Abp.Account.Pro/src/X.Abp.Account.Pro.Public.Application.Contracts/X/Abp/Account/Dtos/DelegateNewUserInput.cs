// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;

namespace X.Abp.Account.Dtos;
public class DelegateNewUserInput
{
    public Guid TargetUserId
    {
        get;
        set;
    }

    [Required]
    public DateTime StartTime
    {
        get;
        set;
    }

    [Required]
    public DateTime EndTime
    {
        get;
        set;
    }
}
