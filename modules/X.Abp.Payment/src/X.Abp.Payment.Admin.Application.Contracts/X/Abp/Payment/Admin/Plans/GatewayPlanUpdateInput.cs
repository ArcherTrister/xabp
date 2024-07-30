// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;

using Volo.Abp.ObjectExtending;

namespace X.Abp.Payment.Admin.Plans
{
    [Serializable]
    public class GatewayPlanUpdateInput : ExtensibleObject
    {
        [Required]
        public string ExternalId { get; set; }
    }
}
