(function ($) {

    abp.multiTenancy = abp.multiTenancy || {};

    var l = abp.localization.getResource("AbpAccount");
    var authorityDelegation = new abp.ModalManager(abp.appPath + "Account/AuthorityDelegation/AuthorityDelegationModal");
    authorityDelegation.onOpen(function () {
        _dataTable.columns.adjust();

        $('#AuthorityDelegationModal a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
            _myDelegatedUsersTable.columns.adjust();
        });
    });

    $(function() {
        $('#MenuItem_Account\\.AuthorityDelegation')
            .removeClass("d-none")
            .click(function() {
                authorityDelegation.open();
            });
    });

    var getStatus = function (data) {
        var status = '';
        var curr = new Date().getTime();
        var beg = new Date(data.startTime).getTime();
        var end = new Date(data.endTime).getTime();
        if (beg > curr) {
            status = 'Future';
        } else if (curr > end) {
            status = 'Expired';
        } else if (beg < curr && curr < end) {
            status = 'Active';
        }

        return status;
    }

    var getStatusBadge = function (status) {
        var badge = '';
        if (status == 'Future') {
            badge = 'warning';
        } else if (status == 'Expired') {
            badge = 'danger';
        } else if (status == 'Active') {
            badge = 'success';
        }
        return badge;
    }
    
    function parseDateTimeToLocaleString(dateTime) {
        return new Date(dateTime+ "+00:00").toLocaleString(abp.localization.currentCulture.name);
    }

    abp.ui.extensions.entityActions
        .get("account.delegatedUsers")
        .addContributor(function (actionList) {
            return actionList.addManyTail([
                {
                    text: l("Delete"),
                    confirmMessage: function (data) {
                        return l(
                            'DeleteUserDelegationConfirmationMessage',
                            data.record.userName
                        );
                    },
                    action: function (data) {
                        x.abp.account.identityUserDelegation
                            .deleteDelegation(data.record.id)
                            .then(function () {
                                _dataTable.ajax.reload(null, false);
                            });
                    },
                },
            ]);
        });

    abp.ui.extensions.tableColumns.get("account.delegatedUsers").addContributor(
        function (columnList) {
            columnList.addManyTail([
                {
                    title: l("Actions"),
                    rowAction: {
                        items: abp.ui.extensions.entityActions
                            .get("account.delegatedUsers")
                            .actions.toArray(),
                    },
                },
                {
                    title: l("Status"),
                    render: function (data, type, row) {
                        var status = getStatus(row);
                        var badge = getStatusBadge(status);
                        return '<span class="badge bg-' + badge + '" >' + l('Status' + status) + '</span>';
                    }
                },
                {
                    title: l("UserName"),
                    data: "userName",
                    orderable: true
                },
                {
                    title: l("StartTime"),
                    data: "startTime",
                    render : function(time){
                        return parseDateTimeToLocaleString(time);
                    }
                },
                {
                    title: l("EndTime"),
                    data: "endTime",
                    render : function(time){
                        return parseDateTimeToLocaleString(time);
                    }
                }
            ]);
        },
        0
    );

    abp.ui.extensions.tableColumns.get("account.myDelegatedUsers").addContributor(
        function (columnList) {
            columnList.addManyTail([
                {
                    title: l("Actions"),
                    rowAction: {
                        items: [
                            {
                                text: l('Login'),
                                enabled: function (data) {
                                    return getStatus(data.record) == 'Active';
                                },
                                action: function (data) {
                                    if (getStatus(data.record) == 'Active') {
                                        $("#DelegatedImpersonationForm input[name='UserDelegationId']").val(data.record.id);
                                        $("#DelegatedImpersonationForm").submit();
                                    }
                                }
                            }
                        ]
                    }
                },
                {
                    title: l("Status"),
                    render: function (data, type, row) {
                        var status = getStatus(row);
                        var badge = getStatusBadge(status);
                        return '<span class="badge bg-' + badge + '" >' + l('Status' + status) + '</span>';
                    }
                },
                {
                    title: l("UserName"),
                    data: "userName",
                    orderable: true
                },
                {
                    title: l("StartTime"),
                    data: "startTime",
                    render : function(time){
                        return parseDateTimeToLocaleString(time);
                    }
                },
                {
                    title: l("EndTime"),
                    data: "endTime",
                    render : function(time){
                        return parseDateTimeToLocaleString(time);
                    }
                }
            ]);
        },
        0
    );

})(jQuery);
