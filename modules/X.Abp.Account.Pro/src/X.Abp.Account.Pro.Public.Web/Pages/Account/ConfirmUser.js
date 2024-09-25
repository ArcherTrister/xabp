(function ($) {

    $(function () {
        var l = abp.localization.getResource("AbpAccount");
        var _accountService = volo.abp.account.account;

        $("#VerifyEmail").click(function (e) {
            _accountService
                .sendEmailConfirmationToken({
                    userId: $("#UserId").val(),
                    appName: "MVC",
                    returnUrl: $("#ReturnUrl").val(),
                    returnUrlHash: $("#ReturnUrlHash").val()
                })
                .then(function () {
                    abp.notify.success(
                        l(
                            "EmailConfirmationSentMessage",
                            $("#Email").val()
                        )
                    );
                    $("#VerifyEmail").hide();

                    setInterval(function () {
                        _accountService.getConfirmationState($("#UserId").val())
                            .then(function (data) {
                                if (data.emailConfirmed) {
                                    location.reload();
                                }
                            });
                    }, 3000);
                });
        });

        $("#VerifyPhoneNumber").click(function (e) {
            _accountService
                .sendPhoneNumberConfirmationToken({
                    userId: $("#UserId").val(),
                    phoneNumber: $("#PhoneNumber").val()
                })
                .then(function () {
                    $('#PhoneNumberModal').modal('show')
                });
        });

        $("#PhoneNumberForm").submit(function (e) {
            e.preventDefault();

            if (!$(this).valid()) {
                return false;
            }

            _accountService
                .confirmPhoneNumber({
                    userId: $("#UserId").val(),
                    token: $("#PhoneConfirmationToken").val()
                })
                .then(function () {
                    location.reload();
                });
        });

    });

})(jQuery);
