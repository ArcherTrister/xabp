(function ($) {
    
    var l = abp.localization.getResource('AbpIdentity');
    var _identityUserAppService = x.abp.identity.identityUser;

    var _externalUserModal = new abp.ModalManager({ viewUrl: abp.appPath + 'Identity/Users/ImportToolbar/ExternalUserModal'});
    var _dataTable = null;
    var _externalLoginProviders = null;
    
    $(function(){

        _dataTable = $('#IdentityUsersWrapper table').data('dataTable');
        
        $('#ExternalUserButton').click(function (e) {
            e.preventDefault();

            _identityUserAppService.getExternalLoginProviders().done(function (providers) {

                _externalLoginProviders = providers;
                
                if(providers.length === 0) {
                    abp.message.warn(l('NoExternalLoginProviderAvailable'));
                    return;
                }
                _externalUserModal.open();
            });
            
            _externalUserModal.onOpen(function () {
                $("#ExternalUser_Provider").change(function () {
                    var provider = _externalLoginProviders.find((p) => {
                        return p.name === $(this).val();
                    })
                    showOrHidePasswordInput(provider.canObtainUserInfoWithoutPassword);
                });
            });

            _externalUserModal.onResult(function () {
                _dataTable.ajax.reload(null, false);
            });
        });
        
        function showOrHidePasswordInput(canObtainUserInfoWithoutPassword){
            if(canObtainUserInfoWithoutPassword){
                $('#ExternalUser_Password').parent().hide();
            }
            else{
                $('#ExternalUser_Password').parent().show();
            }
        }
    })
})(jQuery);