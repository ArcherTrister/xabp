(function ($) {

    var l = abp.localization.getResource('AbpOpenIddict');

    var _createModal = new abp.ModalManager({
        viewUrl: abp.appPath + 'OpenIddict/Scopes/CreateModal',
        modalClass: 'scopeCreate'
    });
    var _editModal = new abp.ModalManager({
        viewUrl: abp.appPath + 'OpenIddict/Scopes/EditModal',
        modalClass: 'scopeUpdate'
    });

    var _dataTable = null;

    abp.ui.extensions.entityActions.get("openIddict.Scope").addContributor(
        function (actionList) {
            return actionList.addManyTail(
                [
                    {
                        text: l('Edit'),
                        visible: abp.auth.isGranted('OpenIddictPro.Scope.Update'),
                        action: function (data) {
                            _editModal.open({
                                id: data.record.id
                            });
                        }
                    },
                    {
                        text: l('ChangeHistory'),
                        visible: abp.auditLogging && abp.auth.isGranted('AuditLogging.ViewChangeHistory:Volo.Abp.OpenIddict.Pro.Scopes.Scope'),
                        action: function (data) {
                            abp.auditLogging.openEntityHistoryModal(
                                "Volo.Abp.OpenIddict.Scopes.OpenIddictScope",
                                data.record.id
                            );
                        }
                    },
                    {
                        text: l('Delete'),
                        visible: abp.auth.isGranted('OpenIddictPro.Scope.Delete'),
                        confirmMessage: function (data) {
                            return l('ScopeDeletionWarningMessage', data.record.name);
                        },
                        action: function (data) {
                            x.abp.openIddict.scope
                                .delete(data.record.id)
                                .then(function () {
                                    _dataTable.ajax.reload(null, false);
                                    abp.notify.success(l('SuccessfullyDeleted'));
                                });
                        }
                    }
                ]
            );
        }
    );

    abp.ui.extensions.tableColumns.get("openIddict.Scope").addContributor(
        function (columnList) {
            columnList.addManyTail(
                [
                    {
                        title: l("Actions"),
                        rowAction: {
                            items: abp.ui.extensions.entityActions.get("openIddict.Scope").actions.toArray()
                        }
                    },
                    {
                        title: l("Name"),
                        data: "name"
                    },
                    {
                        title: l("DisplayName"),
                        data: "displayName"
                    },
                    {
                        title: l("Description"),
                        data: "description"
                    }
                ]
            );
        },
        0 //adds as the first contributor
    );

    $(function () {

        var _$wrapper = $('#ScopesWrapper');

        var getFilter = function () {
            return {
                filter: _$wrapper.find('input.page-search-filter-text').val()
            };
        };

        _dataTable = $('#ScopesTable').DataTable(abp.libs.datatables.normalizeConfiguration({
            processing: true,
            serverSide: true,
            paging: true,
            scrollX: true,
            searching: false,
            scrollCollapse: true,
            order: [[1, "desc"]],
            ajax: abp.libs.datatables.createAjax(x.abp.openIddict.scope.getList, getFilter),
            columnDefs: abp.ui.extensions.tableColumns.get("openIddict.Scope").columns.toArray()
        }));

        $("#CreateNewButtonId").click(function () {
            _createModal.open();
        });

        _createModal.onClose(function () {
            _dataTable.ajax.reload(null, false);
        });

        _editModal.onResult(function () {
            _dataTable.ajax.reload(null, false);
        });

        _$wrapper.find('form.page-search-form').submit(function (e) {
            e.preventDefault();
            _dataTable.ajax.reload();
        });
    });
})(jQuery);
