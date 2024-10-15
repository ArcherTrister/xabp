// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Volo.CmsKit;

namespace X.Abp.CmsKit.PageFeedbacks
{
    public class PageFeedbackEntityTypeDefinition : PolicySpecifiedDefinition, IEquatable<PageFeedbackEntityTypeDefinition>
    {
        public PageFeedbackEntityTypeDefinition(
          string entityType,
          IEnumerable<string> createPolicies = null,
          IEnumerable<string> updatePolicies = null,
          IEnumerable<string> deletePolicies = null)
          : base(entityType, createPolicies, updatePolicies, deletePolicies)
        {
        }

        public bool Equals(PageFeedbackEntityTypeDefinition other) => other?.EntityType == EntityType;
    }
}
