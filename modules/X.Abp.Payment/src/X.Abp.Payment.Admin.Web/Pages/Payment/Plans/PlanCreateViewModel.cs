// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;

using X.Abp.Payment.Plans;

namespace X.Abp.Payment.Admin.Web.Pages.Payment.Plans
{
    public class PlanCreateViewModel : ExtensibleObject
    {
        [Required]
        [DynamicMaxLength(typeof(PlanConsts), "MaxNameLength")]
        public string Name { get; set; }
    }
}
