(function ($) {
  $(function () {
    var l = abp.localization.getResource("AbpAccount");
    var _profileService = x.abp.account.profile;

    $("#PersonalTwoFactorForm").submit(function (e) {
      e.preventDefault();
      _profileService
        .setTwoFactorEnabled(
          $("#PersonalTwoFactorForm").serializeFormToObject().twoFactorEnabled
        )
        .then(function (result) {
          abp.notify.success(l("TwoFactorChanged"));
        });
    });
  });
})(jQuery);
