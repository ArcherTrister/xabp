(function ($) {

    abp.multiTenancy = abp.multiTenancy || {};

    var l = abp.localization.getResource("AbpAccount");

    var linkedAccounts = new abp.ModalManager(abp.appPath + "Account/LinkUsers/LinkUsersModal");
    linkedAccounts.onOpen(function () {
        $("#linkUserLoginForm").data("dataTable").columns.adjust();

        $(".loginAsLinkedUser").on('click', function(){
            var userId = $(this).attr("data-userId");
            var tenantId = $(this).attr("data-tenantId");
            linkUserPage.loginAsLinkedUser(userId, tenantId,);
        })
    });

    $(function() {
        $('#MenuItem_Account\\.LinkedAccounts')
            .removeClass("d-none")
            .click(function() {
                linkedAccounts.open();
            });
    });

    var linkUserPage = {
        loginAsLinkedUser: function (userId, tenantId) {
            x.abp.account.identityLinkUser.generateLinkLoginToken().then(function (token) {
                $("#linkUserLoginForm input[name='SourceLinkToken']").val(
                    token
                );
                $("#linkUserLoginForm input[name='TargetLinkUserId']").val(
                    userId
                );
                if (tenantId && tenantId !== "null") {
                    $("#linkUserLoginForm input[name='TargetLinkTenantId']").val(
                        tenantId
                    );
                }
                $("#linkUserLoginForm").submit();
            });
        }
    }

    abp.ui.extensions.entityActions
        .get("account.linkUsers")
        .addContributor(function (actionList) {
            return actionList.addManyTail([
                {
                    text: l("LoginAsThisAccount"),
                    action: function (data) {
                        linkUserPage.loginAsLinkedUser(data.record.targetUserId, data.record.targetTenantId);
                    },
                },
                {
                    text: l("Delete"),
                    confirmMessage: function (data) {
                        return l(
                            'DeleteLinkAccountConfirmationMessage',
                            data.record.targetTenantName ?
                                data.record.targetTenantName + "\\" + data.record.targetUserName :
                                data.record.targetUserName
                        );
                    },
                    visible: function (data) {
                        return data.directlyLinked
                    },
                    action: function (data) {
                        x.abp.account.identityLinkUser
                            .unlink({
                                UserId: data.record.targetUserId,
                                TenantId: data.record.targetTenantId,
                            })
                            .then(function () {
                                _dataTable.ajax.reload(null, false);
                            });
                    },
                },
            ]);
        });

    abp.ui.extensions.tableColumns.get("account.linkUsers").addContributor(
        function (columnList) {
            columnList.addManyTail([
                {
                    title: l("Actions"),
                    rowAction: {
                        items: abp.ui.extensions.entityActions
                            .get("account.linkUsers")
                            .actions.toArray(),
                    },
                },
                {
                    title: l("TenantAndUserName"),
                    render: function (data, type, row) {

                        let tenantAndUserName;

                        row.targetUserName = $.fn.dataTable.render.text().display(row.targetUserName);

                        if (row.targetTenantName) {
                            tenantAndUserName = $.fn.dataTable.render.text().display(row.targetTenantName) + "\\" + row.targetUserName;
                        } else {
                            tenantAndUserName = row.targetUserName;
                        }

                        return `<span class="loginAsLinkedUser" data-userId="${row.targetUserId}" data-tenantId="${row.targetTenantId}" style='cursor:pointer'>${tenantAndUserName}</span>`;
                    },
                    orderable: true
                },
                {
                    title: l("DirectlyLinked"),
                    data: "directlyLinked",
                    dataFormat: "boolean",
                    orderable: false
                }
            ]);
        },
        0 //adds as the first contributor
    );

})(jQuery);
