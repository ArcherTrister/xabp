// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp;

namespace X.Abp.CmsKit.PageFeedbacks
{
    public class EntityCantHavePageFeedbackException : BusinessException
    {
        public string EntityType
        {
            get
            {
                return Data["EntityType"] as string;
            }
        }

        public EntityCantHavePageFeedbackException(string entityType)
            : base(CmsKitProErrorCodes.PageFeedbacks.EntityCantHavePageFeedback)
        {
            Check.NotNullOrEmpty(entityType, nameof(entityType), PageFeedbackConst.MaxEntityTypeLength);

            // TODO: WithData
            WithData("EntityType", EntityType);
        }
    }
}
