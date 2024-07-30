(function () {
    let l = abp.localization.getResource('Saas');
    let _tenantAppService = x.saas.tenant;

    let _editModal = new abp.ModalManager({
        viewUrl: abp.appPath + 'Saas/Tenants/EditModal',
        modalClass: 'SaaSTenantEdit',
    });
    let _createModal = new abp.ModalManager({
        viewUrl: abp.appPath + 'Saas/Tenants/CreateModal',
        modalClass: 'SaaSTenantCreate',
    });
    let _featuresModal = new abp.ModalManager(
        abp.appPath + 'FeatureManagement/FeatureManagementModal'
    );
    let _changeTenantPasswordModal = new abp.ModalManager({
        viewUrl: abp.appPath + 'Saas/Tenants/SetPassword',
        modalClass: 'changeTenantPassword',
    });

    let _connectionStringsModal = new abp.ModalManager({
        viewUrl: abp.appPath + 'Saas/Tenants/ConnectionStringsModal',
        modalClass: 'TenantConnectionStringManagement',
    });

    let _dataTable = null;
    abp.ui.extensions.entityActions
        .get('saas.tenant')
        .addContributor(function (actionList) {
            return actionList.addManyTail([
                {
                    text: l('Edit'),
                    visible: abp.auth.isGranted('Saas.Tenants.Update'),
                    action: function (data) {
                        _editModal.open({
                            id: data.record.id,
                        });
                    },
                },
                {
                    text: l('ConnectionStrings'),
                    visible: abp.auth.isGranted(
                        'Saas.Tenants.ManageConnectionStrings'
                    ),
                    action: function (data) {
                        _connectionStringsModal.open({
                            id: data.record.id,
                        });
                    },
                },
                {
                    text: l('ApplyDatabaseMigrations'),
                    visible: function (record) {
                        return (
                            record.hasDefaultConnectionString &&
                            abp.auth.isGranted(
                                'Saas.Tenants.ManageConnectionStrings'
                            )
                        );
                    },
                    action: function (data) {
                        _tenantAppService
                            .applyDatabaseMigrations(data.record.id)
                            .then(function () {
                                abp.notify.info(
                                    l('DatabaseMigrationQueuedAndWillBeApplied')
                                );
                            });
                    },
                },
                {
                    text: l('Features'),
                    visible: abp.auth.isGranted('Saas.Tenants.ManageFeatures'),
                    action: function (data) {
                        _featuresModal.open({
                            providerName: 'T',
                            providerKey: data.record.id,
                        });
                    },
                },
                {
                    text: l('SetPassword'),
                    visible: abp.auth.isGranted('Saas.Tenants.SetPassword'),
                    action: function (data) {
                        _changeTenantPasswordModal.open({
                            id: data.record.id,
                        });
                    },
                },
                {
                    text: l('ChangeHistory'),
                    visible:
                        abp.auditLogging &&
                        abp.auth.isGranted(
                            'AuditLogging.ViewChangeHistory:Volo.Saas.Tenant'
                        ),
                    action: function (data) {
                        abp.auditLogging.openEntityHistoryModal(
                            'Volo.Saas.Tenants.Tenant',
                            data.record.id
                        );
                    },
                },
                {
                    text: l('Delete'),
                    visible: abp.auth.isGranted('Saas.Tenants.Delete'),
                    confirmMessage: function (data) {
                        return l(
                            'TenantDeletionConfirmationMessage',
                            data.record.name
                        );
                    },
                    action: function (data) {
                        _tenantAppService
                            .delete(data.record.id)
                            .then(function () {
                                _dataTable.ajax.reload(null, false);
                                abp.notify.success(l('SuccessfullyDeleted'));
                            });
                    },
                },
            ]);
        });

    abp.ui.extensions.tableColumns.get('saas.tenant').addContributor(
        function (columnList) {
            columnList.addManyTail([
                {
                    title: l('Actions'),
                    rowAction: {
                        items: abp.ui.extensions.entityActions
                            .get('saas.tenant')
                            .actions.toArray(),
                    },
                },
                {
                    title: l('TenantName'),
                    data: 'name',
                },
                {
                    title: l('Edition'),
                    data: 'editionName',
                    orderable: false,
                },
                {
                    title: l('EditionEndDateUtc'),
                    data: 'editionEndDateUtc',
                    orderable: false,
                    render: function (data, type, row) {
                        if (data < new Date().toISOString()) {
                            return (
                                '<span class="text-danger">' + data + '</span>'
                            );
                        } else {
                            return data;
                        }
                    },
                },
                {
                    title: l('ActivationState'),
                    data: 'activationState',
                    render: function (data, type, row) {
                        switch (data) {
                            case 0:
                                return l('Enum:TenantActivationState.Active');
                            case 1:
                                return (
                                    l(
                                        'Enum:TenantActivationState.ActiveWithLimitedTime'
                                    ) +
                                    ' (' +
                                    new Date(
                                        Date.parse(row.activationEndDate)
                                    ).toLocaleString(
                                        abp.localization.currentCulture.name
                                    ) +
                                    ')'
                                );
                            case 2:
                                return l('Enum:TenantActivationState.Passive');
                            default:
                                return null;
                        }
                    },
                },
            ]);
        },
        0 //adds as the first contributor
    );

    $(function () {
        let _$wrapper = $('#TenantsWrapper');

        let getFilter = function () {
            let editionId = _$wrapper
                .find('select#tenant-edition-option')
                .val();
            let activationState = _$wrapper
                .find('select#ActivationState')
                .val();

            return {
                filter: _$wrapper.find('input.page-search-filter-text').val(),
                editionId: !editionId ? null : editionId,
                expirationDateMax: minDate,
                expirationDateMin: maxDate,
                activationState: activationState,
            };
        };

        _dataTable = _$wrapper.find('table').DataTable(
            abp.libs.datatables.normalizeConfiguration({
                processing: true,
                serverSide: true,
                paging: true,
                scrollX: true,
                searching: false,
                scrollCollapse: true,
                order: [[1, 'asc']],
                ajax: abp.libs.datatables.createAjax(
                    _tenantAppService.getList,
                    getFilter
                ),
                columnDefs: abp.ui.extensions.tableColumns
                    .get('saas.tenant')
                    .columns.toArray(),
            })
        );

        _createModal.onResult(function () {
            _dataTable.ajax.reload(null, false);
        });

        _editModal.onResult(function () {
            _dataTable.ajax.reload(null, false);
        });

        $('#AbpContentToolbar button[name=CreateTenant]').click(function (e) {
            e.preventDefault();
            _createModal.open();
        });

        _$wrapper.find('form.page-search-form').submit(function (e) {
            e.preventDefault();
            _dataTable.ajax.reload();
        });

        $('select#tenant-edition-option').change(function (e) {
            _dataTable.ajax.reload(null, false);
        });

        $('input#ExpirationDateMax').change(function (e) {
            _dataTable.ajax.reload(null, false);
        });

        $('input#ExpirationDateMin').change(function (e) {
            _dataTable.ajax.reload(null, false);
        });

        $('select#ActivationState').change(function (e) {
            _dataTable.ajax.reload(null, false);
        });

        $('#AdvancedFilterSectionToggler').click(function (e) {
            $('#AdvancedFilterSection').toggle();
        });

        var minDate = '';
        var maxDate = '';

        var $inputTime = $('#inputTime');
        $inputTime.daterangepicker({
            autoUpdateInput: false,
            locale: {
                cancelLabel: 'Clear',
            },
        });

        $inputTime.on('apply.daterangepicker', function (ev, picker) {
            $(this).val(
                picker.startDate.format('MM/DD/YYYY') +
                    ' - ' +
                    picker.endDate.format('MM/DD/YYYY')
            );
            minDate = picker.startDate.format('MM/DD/YYYY');
            maxDate = picker.endDate.format('MM/DD/YYYY');
            _dataTable.ajax.reload(null, false);
        });

        $inputTime.on('cancel.daterangepicker', function (ev, picker) {
            $(this).val('');
            minDate = '';
            maxDate = '';
            _dataTable.ajax.reload(null, false);
        });

        $inputTime.on('change', function () {
            var dates = $(this).val().split(' - ');
            if (dates.length === 2) {
                minDate = dates[0];
                maxDate = dates[1];
            } else if (dates.length === 1) {
                minDate = dates[0];
                maxDate = '';
            } else {
                minDate = '';
                maxDate = '';
            }
            _dataTable.ajax.reload(null, false);
        });
    });
})();
