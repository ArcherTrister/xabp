(function ($) {

    $(function () {
        var l = abp.localization.getResource('AbpIdentity');

        $('#SignIn_RequireConfirmedPhoneNumber').on('change', '', function (e) {
            if (this.checked) {
                $('#SignIn_EnablePhoneNumberConfirmation').prop('checked', true);
            }
        });

        $('#SignIn_EnablePhoneNumberConfirmation').on('change', '', function (e) {
            if (!this.checked && $('#SignIn_RequireConfirmedPhoneNumber')[0].checked) {
                $('#SignIn_RequireConfirmedPhoneNumber').prop('checked', false);
            }
        });

        $("#IdentitySettingsForm").on('submit', function (event) {
            event.preventDefault();
            var form = $(this).serializeFormToObject();

            x.abp.identity.identitySettings.update(form).then(function (result) {
                $(document).trigger("AbpSettingSaved");
            });
        });

        $("#IdentityLdapSettingsForm").on("submit", function (event) {
            event.preventDefault();
            var form = $(this).serializeFormToObject();

            x.abp.identity.identitySettings.updateLdap(form).then(function (result) {
                $(document).trigger("AbpSettingSaved");
            });
        });

        $("#IdentityOAuthSettingsForm").on("submit", function (event) {
            event.preventDefault();
            var form = $(this).serializeFormToObject();

            x.abp.identity.identitySettings.updateOAuth(form).then(function (result) {
                $(document).trigger("AbpSettingSaved");
            });
        });
    });

})(jQuery);