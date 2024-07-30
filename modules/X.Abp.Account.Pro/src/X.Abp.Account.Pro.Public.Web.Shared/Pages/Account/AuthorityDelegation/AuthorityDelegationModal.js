(function () {
    
    var l = abp.localization.getResource("AbpAccount");
    var _identityUserDelegation = x.abp.account.identityUserDelegation;
    var delegateNewUserModal = new abp.ModalManager(abp.appPath + "Account/AuthorityDelegation/DelegateNewUserModal");

    _dataTable = $("#DelegateNewUsersTable").DataTable(
        abp.libs.datatables.normalizeConfiguration({
            aLengthMenu: [5, 10, 25, 50, 100],
            order: [],
            ajax: abp.libs.datatables.createAjax(_identityUserDelegation.getDelegatedUsers),
            columnDefs: abp.ui.extensions.tableColumns
                .get("account.delegatedUsers")
                .columns.toArray()
        })
    );

    _myDelegatedUsersTable = $("#MyDelegatedUsersTable").DataTable(
        abp.libs.datatables.normalizeConfiguration({
            aLengthMenu: [5, 10, 25, 50, 100],
            order: [],
            ajax: abp.libs.datatables.createAjax(_identityUserDelegation.getMyDelegatedUsers),
            columnDefs: abp.ui.extensions.tableColumns
                .get("account.myDelegatedUsers")
                .columns.toArray()
        })
    );

    $("#DelegateNewUser").click(function () {
        abp.ui.block("#AuthorityDelegationModal");
        delegateNewUserModal.open();
    });

    delegateNewUserModal.onClose(function () {
        abp.ui.unblock();
        _dataTable.ajax.reload(null, false);
    });
})();
