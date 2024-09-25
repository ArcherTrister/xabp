(function ($) {
    $(function () {

        $("#ChatSettingsForm").on("submit", function (event) {
            event.preventDefault();
            var form = $(this).serializeFormToObject();

            volo.chat.settings.settings.update(form).then(function (result) {
                $(document).trigger("AbpSettingSaved");
            });
        });


        var chatSettingPage = {

            $deletingConversationsFormGroup: $('select[name=DeletingConversations]').parent(),

            $messageDeletionPeriodFormGroup: $('input[name=MessageDeletionPeriod]').parent(),

            $deletingMessagesSelect: $('select[name=DeletingMessages]'),

            onDeletingMessagesChange: function () {
                chatSettingPage.$deletingMessagesSelect.change(() => {
                    chatSettingPage.showOrHideFormGroup();
                })
            },

            showOrHideFormGroup: function () {
                var value = chatSettingPage.$deletingMessagesSelect.val();

                if (value === '1') {
                    chatSettingPage.$deletingConversationsFormGroup.show();
                    chatSettingPage.$messageDeletionPeriodFormGroup.hide();
                }

                if (value === '2') {
                    chatSettingPage.$deletingConversationsFormGroup.hide();
                    chatSettingPage.$messageDeletionPeriodFormGroup.hide();
                }

                if (value === '3') {
                    chatSettingPage.$deletingConversationsFormGroup.hide();
                    chatSettingPage.$messageDeletionPeriodFormGroup.show();
                }
            },

            init: function () {
                chatSettingPage.showOrHideFormGroup();
                chatSettingPage.onDeletingMessagesChange();
            }
        };

        chatSettingPage.init();

    });
})(jQuery);
