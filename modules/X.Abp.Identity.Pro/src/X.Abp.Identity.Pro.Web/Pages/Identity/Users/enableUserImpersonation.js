(function ($) {

    var l = abp.localization.getResource('AbpIdentity');

    abp.ui.extensions.entityActions.get("identity.user").addContributor(
        function(actionList) {
            return actionList.addBefore(
            {
                    text: l('LoginWithThisUser'),
                    visible: function (data) {
                        return abp.auth.isGranted('AbpIdentity.Users.Impersonation') &&
                            abp.currentUser.id != data.id &&
                            abp.currentUser.impersonatorUserId == null;
                    },
                    action: function (data) {
                        $("#ImpersonationForm input[name='UserId']").val(data.record.id);
                        $("#ImpersonationForm").submit();
                    }
                },
                l('Delete'),
                (value, searchedValue) => value.text === searchedValue
            );
        }
    );

})(jQuery);
