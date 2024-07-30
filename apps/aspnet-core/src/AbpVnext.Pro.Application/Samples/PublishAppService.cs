// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using AbpVnext.Pro.Samples.Dtos;

using Volo.Abp.Localization;

using X.Abp.Notification;

namespace AbpVnext.Pro.Samples
{
    /// <summary>
    /// Implements <see cref="PublishAppService"/>.
    /// </summary>
    public class PublishAppService : ProAppService
    {
        private readonly INotificationPublisher _notiticationPublisher;

        public PublishAppService(INotificationPublisher notiticationPublisher)
        {
            _notiticationPublisher = notiticationPublisher;
        }

        // 给特定的用户发送一个一般通知
        public async Task SentFrendshipRequestAsync(PublishSentFrendshipRequestInput input)
        {
            await _notiticationPublisher.PublishAsync("SentFrendshipRequest", new SentFrendshipRequestNotificationData(input.SenderUserName, input.FriendshipMessage), userIds: new UserIdentifier[] { new UserIdentifier(null, input.TargetUserId) });
        }

        // 给特定的用户发送一个实体通知
        public async Task CommentPhotoAsync(PublishCommentPhotoInput input)
        {
            await _notiticationPublisher.PublishAsync("CommentPhoto", new CommentPhotoNotificationData(input.CommenterUserName, input.Comment), new EntityIdentifier(typeof(Photo), input.PhotoId), userIds: new UserIdentifier[] { new UserIdentifier(null, input.PhotoOwnerUserId) });
        }

        // 给具特定严重级别程度的所有订阅用户发送一个一般通知
        public async Task LowDiskAsync(int remainingDiskInMb)
        {
            // 例如 "LowDiskWarningMessage"的英文内容 -> "Attention! Only {remainingDiskInMb} MBs left on the disk!"
            var data = new LocalizableMessageNotificationData(new LocalizableString("LowDiskWarningMessage", "MyLocalizationSourceName"));
            data["remainingDiskInMb"] = remainingDiskInMb;

            await _notiticationPublisher.PublishAsync("System.LowDisk", data, severity: NotificationSeverity.Warn);
        }
    }
}
