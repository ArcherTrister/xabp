(function ($) {

    $(function () {
        $('#SelectedProvider').change(function () {
            if (this.value == "Authenticator") {
                $("#SendSecurityCode_Information").hide();
                $("#loginWithRecoveryCode").show();
            } else {
                $("#SendSecurityCode_Information").show();
                $("#loginWithRecoveryCode").hide();
            }
        }).change();
    });

})(jQuery);
