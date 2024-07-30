(function(){
    var l = abp.localization.getResource("AbpAccount");
    var startTime;
    var endTime;

    var $inputTime = $("#inputTime");
    $inputTime.daterangepicker({
        timePicker:true,
        autoUpdateInput: false,
        locale: {
            cancelLabel: 'Clear'
        }
    });

    $inputTime.on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('LLL') + ' - ' + picker.endDate.format('LLL'));
        startTime = picker.startDate.format('YYYY-MM-DD HH:mm');
        endTime = picker.endDate.format('YYYY-MM-DD HH:mm');
    });

    $inputTime.on('cancel.daterangepicker', function (ev, picker) {
        $(this).val('');
        startTime = null;
        endTime = null;
    });

    var $targetUserSelect = $("#TargetUserId");
    $targetUserSelect.data('autocompleteApiUrl', '/api/account/user-delegation/user-lookup');
    $targetUserSelect.data('autocompleteDisplayProperty', 'userName');
    $targetUserSelect.data('autocompleteValueProperty', 'id');
    $targetUserSelect.data('autocompleteItemsProperty', 'items');
    $targetUserSelect.data('autocompleteFilterParamName', 'filter');
    $targetUserSelect.data('autocompleteParentSelector', '#DelegateNewUserModal');

    abp.dom.initializers.initializeAutocompleteSelects($targetUserSelect);
   
    

    $("#DelegateNewUserForm").on("submit", function (event) {
        event.preventDefault();
        var input = $(this).serializeFormToObject();
        
        if(!input.targetUserId){
            abp.message.error(l("AuthorityDelegation:PleaseSelectUser"));
            return;
        }
        
        if(!startTime || !endTime){
            abp.message.error(l("AuthorityDelegation:PleaseSelectDelegationDateRange"));
            return;
        }
        
        input.startTime = new Date(startTime);
        input.endTime = new Date(endTime);

        x.abp.account.identityUserDelegation.delegateNewUser(input).then(function () {
            $("#DelegateNewUserForm").find(".modal").modal('hide');
        });
    });
    
})();
