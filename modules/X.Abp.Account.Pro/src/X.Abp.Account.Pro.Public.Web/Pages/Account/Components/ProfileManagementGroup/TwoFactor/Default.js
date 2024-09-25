(function ($) {
    $(function () {

        var l = abp.localization.getResource("AbpAccount");
        var _profileService = volo.abp.account.profile;
        $("#TwoFactorEnabled").change(function () {
            var isChecked = $(this).is(':checked');
            _profileService
                .setTwoFactorEnabled(isChecked)
                .then(function () {
                    abp.notify.success(l("TwoFactorChanged"));
                });
        });

    });
})(jQuery);
