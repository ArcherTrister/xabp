(function ($) {

    $(function () {
        $('#ReturnToPreviousAccount').click(function () {
            x.abp.account.identityLinkUser.generateLinkLoginToken().then(function (token) {
                $("#linkUserLoginForm input[name='SourceLinkToken']").val(token);
                $("#linkUserLoginForm").submit();
            });
        })
    });

})(jQuery);
