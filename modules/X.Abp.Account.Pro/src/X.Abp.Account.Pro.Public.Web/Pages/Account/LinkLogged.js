(function ($) {

    $(function () {
        $('#ReturnToPreviousAccount').click(function () {
            volo.abp.account.identityLinkUser.generateLinkLoginToken().then(function (token) {
                $("#linkUserLoginForm input[name='SourceLinkToken']").val(token);
                $("#linkUserLoginForm").submit();
            });
        })
    });

})(jQuery);
