(function ($) {
    $(function () {

        var l = abp.localization.getResource("AbpAccount");
        var _profileService = volo.abp.account.profile;
        var isQrCodeInitialized = false;

        $("#verifyQRCode").click(function (e) {
            volo.abp.account.account.verifyAuthenticatorCode({ "code": $("#Code").val() }).then(function (result) {
                $("#recoveryCodes").show();

                var recoveryCodesList = $("#recoveryCodesList");
                recoveryCodesList.empty();
                for (var i = 0; i < result.recoveryCodes.length; i++) {
                    recoveryCodesList.append("<code>" + result.recoveryCodes[i] + "</code><br />");
                }

                $("#verifyQRCode").text(l("Verified"));
                $("#next-btn-step-2").removeClass("disabled");
            });
        });

        $("#resetAuthenticator").click(function (e) {
            abp.message.confirm(
                l('ResetAuthenticatorWarningMessage'),
                l('Are you sure?'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        volo.abp.account.account.resetAuthenticator().then(function (result) {
                            location.hash = "#Volo-Abp-Account-TwoFactor";
                            location.reload();
                        });
                    }
                }
            );
        });

        $("#copySharedKey").click(function (e) {
            navigator.clipboard.writeText($('#sharedKey code').html());
            abp.notify.success("Copied to the clipboard.");
        });

        $("#copyRecoverCodes").click(function (e) {
            navigator.clipboard.writeText($("#recoveryCodesList").text().match(/.{1,11}/g).join('\n'));
            abp.notify.success("Copied to the clipboard.");
        });

        $("#printRecoverCodes").click(function (e) {
            var printWindow = window.open();
            printWindow.document.write('<html><body>');
            printWindow.document.write(document.getElementById("recoveryCodesList").innerHTML);
            printWindow.document.write('</body></html>');
            printWindow.document.close();
            printWindow.print();
        });

        $("#recoveryCodeOk").click(function (e) {
            window.location.reload();
        });

        $("#next-btn-step-1").click(function () {
            $("#pills-second-step-tab").removeClass("disabled");
            $("#pills-second-step-tab").click();
        });

        $("#next-btn-step-2").click(function () {
            $("#pills-third-step-tab").removeClass("disabled");
            $("#pills-third-step-tab").click();
        });

        function initQrCode() {
            if (isQrCodeInitialized) {
                return;
            }

            var qrCodeData = document.getElementById("qrCodeData");
            if (!qrCodeData || qrCodeData.length <= 0) {
                return;
            }

            const uri = qrCodeData.getAttribute('data-url');
            new QRCode(document.getElementById("qrCode"), {
                text: uri,
                width: 150,
                height: 150
            });
            isQrCodeInitialized = true;
        }

        initQrCode();
    });
})(jQuery);
