(function () {
    var l = abp.localization.getResource("AbpAccount");

    var _identityLinkUser = x.abp.account.identityLinkUser;

    _dataTable = $("#MyLinkUsersTable").DataTable(
        abp.libs.datatables.normalizeConfiguration({
            aLengthMenu: [ 5, 10, 25, 50, 100 ],
            order: [],
            ajax: abp.libs.datatables.createAjax(_identityLinkUser.getAllList),
            columnDefs: abp.ui.extensions.tableColumns
                .get("account.linkUsers")
                .columns.toArray()
        })
    );

    $("#linkUserLoginForm").data("dataTable", _dataTable);

    $("#CreateLinkUser").click(function () {
        abp.message.confirm(l("NewLinkAccountWarning"), l("AreYouSure"), function (result) {
            if(result) {
                _identityLinkUser.generateLinkToken().then(function (token) {
                    var url =
                        abp.appPath +
                        "Account/Login?handler=CreateLinkUser&" +
                        "LinkUserId=" +
                        abp.currentUser.id +
                        "&LinkToken=" +
                        encodeURIComponent(token);
                    if (abp.currentTenant.id) {
                        url += "&LinkTenantId=" + abp.currentTenant.id;
                    }
                    location.href = url;
                });
            }
        });
    });

})();
