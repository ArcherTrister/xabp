(function () {
    var l = abp.localization.getResource('Quartz');
    var _jobsAppService = x.quartz.jobs;
    
    abp.ui.extensions.tableColumns.get('quartz.jobs').addContributor(
        function (columnList) {
            columnList.addManyTail(
                [
                    {
                        title: l("JobName"),
                        data: 'jobName',
                    },
                    {
                        title: l("JobGroup"),
                        data: 'JobGroup',
                    },
                    {
                        title: l("TriggerName"),
                        data: 'TriggerName',
                    },
                    {
                        title: l("TriggerGroup"),
                        data: 'TriggerGroup',
                    },
                    {
                        title: l("FiredTime"),
                        data: 'FiredTime',
                    },
                    {
                        title: l("ScheduledTime"),
                        data: 'ScheduledTime',
                    },
                    {
                        title: l("RunTime"),
                        data: 'RunTime',
                    },
                    {
                        title: l("Error"),
                        data: 'Error',
                    },
                    {
                        title: l("ErrorMessage"),
                        data: 'ErrorMessage',
                    }
                ]
            );
        },
        0 //adds as the first contributor
    );

    $(function () {
        var _$wrapper = $('#JobsWrapper');

        _$wrapper.find('table').DataTable(
            abp.libs.datatables.normalizeConfiguration({
                order: [[1, 'asc']],
                processing: true,
                paging: true,
                scrollX: true,
                serverSide: true,
                ajax: abp.libs.datatables.createAjax(_jobsAppService.getList),
                columnDefs: abp.ui.extensions.tableColumns.get('quartz.jobs').columns.toArray(),
            })
        );
    });
})();
