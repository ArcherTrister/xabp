var abp = abp || {};
$(function () {
    abp.modals.applicationCreate = function () {
        
       var $allowClientCredentialsFlowInput = $('#Application_AllowClientCredentialsFlow'); 
       var $allowAuthorizationCodeFlowInput = $('#Application_AllowAuthorizationCodeFlow');
       var $allowImplicitFlowInput = $('#Application_AllowImplicitFlow');
       var $allowHybridFlowInput = $('#Application_AllowHybridFlow');
       var $allowPasswordFlowInput = $('#Application_AllowPasswordFlow');
       var $allowDeviceEndpointFlowInput = $('#Application_AllowDeviceEndpoint');
       var _l = abp.localization.getResource("AbpOpenIddict");
        
        function showOrHideInputs(){
            var allowAuthorizationCodeFlow = $allowAuthorizationCodeFlowInput.is(':checked')
            var allowImplicitFlow = $allowImplicitFlowInput.is(':checked')
            var allowHybridFlowInput = $allowHybridFlowInput.is(':checked')
            
            if(allowAuthorizationCodeFlow || allowImplicitFlow || allowHybridFlowInput){
                $(".HideInputs").show();
            }else{
                $(".HideInputs").hide();
            }
        }
        
        function enableOrDisableAllowRefreshTokenFlowInput(){
            var allowAuthorizationCodeFlow = $allowAuthorizationCodeFlowInput.is(':checked')
            var allowHybridFlowInput = $allowHybridFlowInput.is(':checked')
            var $allowPasswordFlow = $allowPasswordFlowInput.is(':checked')
            
            if(allowAuthorizationCodeFlow || allowHybridFlowInput || $allowPasswordFlow){
                $('#Application_AllowRefreshTokenFlow').removeAttr('disabled');
            }else{
                $("#Application_AllowRefreshTokenFlow").prop('checked', false)
                $('#Application_AllowRefreshTokenFlow').attr('disabled', 'disabled');
            }
        }
        
        function showOrHideConfidentialFlowInputs(){
            var type = $('#Application_Type').val();
            if(type === "public") {
                $allowClientCredentialsFlowInput.prop('checked', false)
                $allowClientCredentialsFlowInput.attr('disabled', 'disabled');
                $allowDeviceEndpointFlowInput.prop('checked', false)
                $allowDeviceEndpointFlowInput.attr('disabled', 'disabled');
                showOrHideNotAvailableMessage(true, $allowClientCredentialsFlowInput.next());
                showOrHideNotAvailableMessage(true, $allowDeviceEndpointFlowInput.next());
            }else{
                $allowClientCredentialsFlowInput.removeAttr('disabled');
                $allowDeviceEndpointFlowInput.removeAttr('disabled');
                showOrHideNotAvailableMessage(false, $allowClientCredentialsFlowInput.next());
                showOrHideNotAvailableMessage(false, $allowDeviceEndpointFlowInput.next());
            }
        }
        
        function showOrHideNotAvailableMessage(show, label){
            var notAvailable = "<br>("+_l("NotAvailableForThisType")+")";
            var text = label.html();
            
            if(show){
                label.html(text + notAvailable)
            }else{
                label.html(text.replace(notAvailable, ""))
            }
        }
        
        var initModal = function (publicApi, args) {
            
            $allowAuthorizationCodeFlowInput.change(function(){
                enableOrDisableAllowRefreshTokenFlowInput();
                showOrHideInputs()
            });

            $allowImplicitFlowInput.change(function(){
                showOrHideInputs()
            });

            $allowHybridFlowInput.change(function(){
                enableOrDisableAllowRefreshTokenFlowInput();
                showOrHideInputs()
            });
            
            $allowPasswordFlowInput.change(function(){
                enableOrDisableAllowRefreshTokenFlowInput();
            });

            $("#Application_AllowLogoutEndpoint").change(function(){
                if($(this).is(':checked')){
                    $(".PostLogoutRedirectUrisInput").show();
                }else{
                    $(".PostLogoutRedirectUrisInput").hide();
                }
            })
            
            $("#Application_Type").change(function(){
                showOrHideConfidentialFlowInputs();
                if($(this).val() === "public"){
                    $("#Application_ClientSecret").parent().hide();
                }else{
                    $("#Application_ClientSecret").parent().show();
                }
            })
        };

        return {
            initModal: initModal
        }
    };
});