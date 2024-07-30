(function () {
    var l = abp.localization.getResource('Quartz');

    var _schedulerAppService = x.quartz.schedulers;

    var _dataTable = null;

    abp.ui.extensions.entityActions.get('quartz.schedulers').addContributor(
        function (actionList) {
            return actionList.addManyTail(
                [
                    {
                        text: l('Edit'),
                        visible: abp.auth.isGranted(
                            'AbpTenantManagement.Tenants.Update'
                        ),
                        action: function (data) {
                            //_editModal.open({
                            //    id: data.record.id,
                            //});
                        },
                    },
                    {
                        text: l('Delete'),
                        visible: abp.auth.isGranted(
                            'AbpTenantManagement.Tenants.Delete'
                        ),
                        confirmMessage: function (data) {
                            return l(
                                'TenantDeletionConfirmationMessage',
                                data.record.name
                            );
                        },
                        action: function (data) {
                            _schedulerAppService
                                .delete(data.record.id)
                                .then(function () {
                                    _dataTable.ajax.reload();
                                    abp.notify.success(l('SuccessfullyDeleted'));
                                });
                        },
                    }
                ]
            );
        }
    );
    
    abp.ui.extensions.tableColumns.get('quartz.schedulers').addContributor(
        function (columnList) {
            columnList.addManyTail(
                [
                    {
                        title: l("Actions"),
                        rowAction: {
                            items: abp.ui.extensions.entityActions.get('quartz.schedulers').actions.toArray()
                        }
                    },
                    {
                        title: l("SchedulerName"),
                        data: 'name',
                    },
                    {
                        title: l("SchedulerInstanceId"),
                        data: 'schedulerInstanceId',
                    },
                    {
                        title: l("Status"),
                        data: 'status',
                    }
                ]
            );
        },
        0 //adds as the first contributor
    );

    $(function () {
        var _$wrapper = $('#SchedulersWrapper');

        _dataTable = _$wrapper.find('table').DataTable(
            abp.libs.datatables.normalizeConfiguration({
                order: [[1, 'asc']],
                processing: true,
                paging: true,
                scrollX: true,
                serverSide: true,
                ajax: abp.libs.datatables.createAjax(_schedulerAppService.allSchedulers),
                columnDefs: abp.ui.extensions.tableColumns.get('quartz.schedulers').columns.toArray(),
            })
        );
    });
})();
