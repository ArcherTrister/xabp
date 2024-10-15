// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace X.Abp.CmsKit.PageFeedbacks
{
    public class PageFeedback : AggregateRoot<Guid>, IMultiTenant, IHasCreationTime
    {
        public virtual string EntityType { get; protected set; }

        public virtual string EntityId { get; protected set; }

        public virtual string Url { get; protected set; }

        public virtual bool IsUseful { get; protected set; }

        public virtual string UserNote { get; protected set; }

        public virtual string AdminNote { get; protected set; }

        public virtual bool IsHandled { get; set; }

        public virtual Guid? TenantId { get; protected set; }

        public virtual DateTime CreationTime { get; protected set; }

        protected PageFeedback()
        {
        }

        internal PageFeedback(
          Guid id,
          string entityType,
          string entityId,
          string url,
          bool isUseful,
          string userNote,
          string adminNote = null,
          bool isHandled = false,
          Guid? tenantId = null)
          : base(id)
        {
            SetEntityType(entityType);
            SetEntityId(entityId);
            SetUrl(url);
            IsUseful = isUseful;
            SetUserNote(userNote);
            SetAdminNote(adminNote);
            IsHandled = isHandled;
            TenantId = tenantId;
        }

        protected virtual PageFeedback SetEntityType(string entityType)
        {
            EntityType = Check.NotNullOrWhiteSpace(entityType, nameof(entityType), PageFeedbackConst.MaxEntityTypeLength);
            return this;
        }

        protected virtual PageFeedback SetEntityId(string entityId)
        {
            EntityId = Check.Length(entityId, nameof(entityId), PageFeedbackConst.MaxEntityIdLength);
            return this;
        }

        protected virtual PageFeedback SetUrl(string url)
        {
            Url = Check.Length(url, nameof(url), PageFeedbackConst.MaxUrlLength);
            return this;
        }

        protected virtual PageFeedback SetUserNote(string userNote)
        {
            UserNote = Check.Length(userNote, nameof(userNote), PageFeedbackConst.MaxUserNoteLength);
            return this;
        }

        public virtual PageFeedback SetAdminNote(string adminNote)
        {
            AdminNote = Check.Length(adminNote, nameof(adminNote), PageFeedbackConst.MaxAdminNoteLength);
            return this;
        }
    }
}
