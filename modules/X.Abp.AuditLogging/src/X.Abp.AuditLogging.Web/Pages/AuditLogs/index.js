var _detailModal = new abp.ModalManager(abp.appPath + 'AuditLogs/Detail');
var _dataTable = null;

var _entityDetailModal = new abp.ModalManager(abp.appPath + 'AuditLogs/EntityChangeDetail');
var _entityChangeDataTable = null;

_entityDetailModal.onOpen(function () {
    return new bootstrap.Popover(_entityDetailModal.getModal().find("time"))
});

var l = abp.localization.getResource("AbpAuditLogging");

function getHttpStatusCodeBadgeClass(code) {
    if (code.toString().startsWith("2")) { return 'bg-success'; }
    else if (code.toString().startsWith("3")) { return 'bg-warning'; }
    else if (code.toString().startsWith("4") || code.toString().startsWith("5")) { return 'bg-danger'; }
    else { return 'bg-primary'; }
}

function getHttpMethodBadgeClass(code) {
    switch (code) {
    case 'GET': return 'bg-info';
    case 'POST': return 'bg-success';
    case 'DELETE': return 'bg-danger';
    case 'PUT': return 'bg-warning';
    default: return '';
    }
}

abp.ui.extensions.entityActions.get('auditLogging.auditLog').addContributor(
    function (actionList) {
        return actionList.addManyTail(
            [
                {
                    text: l('Detail'),
                    icon: 'search',
                    action: function (data) {
                        _detailModal.open({
                            id: data.record.id
                        });
                    }
                }
            ]
        );
    }
);

abp.ui.extensions.tableColumns.get("auditLogging.auditLog").addContributor(
    function (columnList) {
        columnList.addManyTail(
            [
                {
                    title: l("Actions"),
                    rowAction:
                    {
                        items: abp.ui.extensions.entityActions.get("auditLogging.auditLog").actions.toArray()
                    }
                },
                {
                    title: l("HttpRequest"),
                    orderable: false,
                    autoWidth: false,
                    data: { httpStatusCode: "httpStatusCode", httpMethod: "httpMethod", url: "url" },
                    render: function (data, type, full) {
                        var httpStatusCodeSpan = data.httpStatusCode != null ? 
                            '<span class="badge ' + getHttpStatusCodeBadgeClass(data.httpStatusCode) + '">' + data.httpStatusCode + '</span>&nbsp; ' :
                            '<span data-filter-field="HttpStatusCode" class="datatableCell"></span>'; 

                        var httpMethodSpan = data.httpMethod != null ?
                            '<span data-filter-field="HttpMethod" class="datatableCell badge ' + getHttpMethodBadgeClass(data.httpMethod) + '">' + data.httpMethod + '</span>&nbsp;' :
                            '<span data-filter-field="HttpMethod" class="datatableCell badge"></span>';

                        return httpStatusCodeSpan + httpMethodSpan + '<span data-filter-field="UrlFilter" class="datatableCell">' + data.url + '</span>';
                    }
                },
                {
                    title: l("UserName"),
                    data: "userName",
                    render: function (data, type, row) {
                        data = data || "";
                        var impersonator = (row.impersonatorTenantName ? row.impersonatorTenantName + "\\" : "") + (row.impersonatorUserName ? row.impersonatorUserName : "");
                        if (impersonator) {
                            impersonator = " (" + impersonator + ")";
                        }
                        var tenant = row.tenantName ? (row.tenantName + "\\") : "";
                        return tenant + '<span class="datatableCell" data-filter-field="UserName">' + data + '</span>' + impersonator;
                    }
                },
                {
                    title: l("IpAddress"),
                    data: "clientIpAddress",
                    render: function (data) {
                        if (data !== null) {
                            return '<span class="datatableCell" data-filter-field="ClientIpAddress">' + data + '</span>';
                        }
                        return "";
                    }
                },
                {
                    title: l("Date"),
                    data: "executionTime",
                    dataFormat: "datetime"
                },
                {
                    title: l("DurationMs"),
                    data: "executionDuration"
                },
                {
                    title: l("ApplicationName"),
                    data: "applicationName",
                    render: function (data) {
                        if (data !== null) {
                            return '<span class="datatableCell" data-filter-field="ApplicationName">' + data + '</span>';
                        }
                        return "";
                    }
                },
                {
                    title: l("CorrelationId"),
                    data: "correlationId",
                    render: function (data) {

                        return '<span class="datatableCell" data-filter-field="CorrelationId">' + data + '</span>';
                    }
                },
                {
                    title: l("Url"),
                    data: "url",
                    render: function (data) {
                        return '<span class="datatableCell" data-filter-field="Url">' + data + '</span>';
                    }
                },
                {
                    title: l("HasException"),
                    data: "exceptions",
                    render: function (data) {
                        if (data != null && data.length > 0) {
                            return '<span class="datatableCell fa fa-check"></span>';
                        }
                        return '';
                    }
                }
            ]
        );
    },
    0 //adds as the first contributor
);

abp.ui.extensions.tableColumns.get("auditLogging.entityChange").addContributor(
    function (columnList) {
        columnList.addManyTail(
            [
                {
                    title: l("Actions"),
                    rowAction: {
                        items: [
                            {
                                text: l('ChangeDetails'),
                                icon: 'search',
                                action: function (data) {
                                    _entityDetailModal.open({
                                        auditLogId: data.record.auditLogId,
                                        entityChangeId: data.record.id
                                    });
                                }
                            },
                            {
                                text: l('FullChangeHistory'),
                                icon: 'history',
                                action: function (data) {
                                    abp.auditLogging.openEntityHistoryModal(
                                        data.record.entityTypeFullName,
                                        data.record.entityId
                                    );
                                }
                            }
                        ]
                    }
                },
                {
                    title: l("Date"),
                    orderable: true,
                    autoWidth: true,
                    data: "changeTime",
                    dataFormat: "datetime"
                },
                {
                    title: l("ChangeType"),
                    orderable: false,
                    autoWidth: true,
                    data: "changeType",
                    render: function (data) {
                        if (data == 0) {
                            return "Created";
                        } else if (data == 1) {
                            return "Updated";
                        } else if (data == 2) {
                            return "Deleted";
                        } else {
                            return "";
                        }
                    }
                },
                {
                    title: l("TenantId"),
                    orderable: false,
                    autoWidth: true,
                    data: "tenantId",
                    render: function (data) {
                        return data || "null";
                    }
                },
                {
                    title: l("EntityTypeFullName"),
                    orderable: false,
                    autoWidth: true,
                    data: "entityTypeFullName"
                }
            ]
        );
    },
    0 //adds as the first contributor
);

$('#EntityChanges-tab').on('shown.bs.tab', function () {
    if (_entityChangeDataTable) {
        _entityChangeDataTable.ajax.reload();
    }
})

$(function () {
    var dateFormat = moment.localeData().longDateFormat("L") + ' HH:mm:ss';
    moment.localeData().preparse = (s) => s;
    moment.localeData().postformat = (s) => s;
    $('.singleDatePicker').daterangepicker({
        autoUpdateInput: false,
        singleDatePicker: true,
        showDropdowns: true,
        timePicker: true,
        timePicker24Hour: true,
        timePickerSeconds: true,
        locale: {
            cancelLabel: 'Clear'
        }
    });

    $('.singleDatePicker').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format(dateFormat));
        $(this).trigger("change");
    });

    $('.singleDatePicker').on('cancel.daterangepicker', function (ev, picker) {
        $(this).val('');
        $(this).trigger("change");
    });
    
    function getFormattedDate(date) {
        var format = "YYYY-MM-DD HH:mm:ss";
        date = moment(date, dateFormat);
        if(date.isValid()) {
            return date.locale('en').format(format);
        }
        return null;
    }
    
    _dataTable = $('#AuditLogsTable').DataTable(abp.libs.datatables.normalizeConfiguration({
        processing: true,
        serverSide: true,
        paging: true,
        scrollX: true,
        searching: false, 
        scrollCollapse: true,
        order: [[4, "desc"]],
        ajax: abp.libs.datatables.createAjax(x.abp.auditLogging.auditLogs.getList,
            function () {
                var form = $("#FilterFormId").serializeFormToObject();
                form.url = form.urlFilter;

                form.startTime = getFormattedDate($('#StartTime').val());
                form.endTime = getFormattedDate($('#EndTime').val());
                
                return form;
            }
        ),
        columnDefs: abp.ui.extensions.tableColumns.get("auditLogging.auditLog").columns.toArray()
    }));

    _entityChangeDataTable = $('#EntityChangesTable').DataTable(abp.libs.datatables.normalizeConfiguration(
        {
            deferLoading: true,
            processing: true,
            serverSide: true,
            paging: true,
            scrollX: false,
            searching: false,
            autoWidth: true,
            scrollCollapse: true,
            order: [[1, "desc"]],
            ajax: abp.libs.datatables.createAjax(x.abp.auditLogging.auditLogs.getEntityChanges,
                function() {
                    var form = $('#EntityChangesFilterFormId').serializeFormToObject();

                    form.startDate = getFormattedDate($('#EntityChangeStartTime').val());
                    form.endDate = getFormattedDate($('#EntityChangeEndTime').val());
                    
                    return form;
                }
            ),
            columnDefs: abp.ui.extensions.tableColumns.get("auditLogging.entityChange").columns.toArray()
        }));

    var entityChangeTableFetched = false;
    $('a[data-toggle="tab"]').on('shown.bs.tab',
        function () {
            if (!entityChangeTableFetched && $('#EntityChanges-tab').hasClass('active')) {
                entityChangeTableFetched = true;
                _entityChangeDataTable.ajax.reload(null, false);
            }
        });

    $("#FilterFormId").submit(function (e) {
        e.preventDefault();
        _dataTable.ajax.reload(null, false);
    });

    $("#EntityChangesFilterFormId").submit(function (e) {
        e.preventDefault();
        _entityChangeDataTable.ajax.reload(null, false);
    });

    $('#FilterFormId').on('change', function () {
        $('#FilterFormId').submit();
    });

    $('#FilterFormId').keypress(function (e) {
        if (e.which == 13) {
            $('#FilterFormId').submit();
        }
    });

    $('#EntityChangesFilterFormId').keypress(function (e) {
        if (e.which == 13) {
            $('#EntityChangesFilterFormId').submit();
        }
    });

});

$("#ClearFilterButton").click(function (e) {
    e.preventDefault();

    $("#StartTime").val("");
    $("#EndTime").val("");
    $("#UserName").val("");
    $("#UrlFilter").val("");
    $("#MinExecutionDuration").val("");
    $("#MaxExecutionDuration").val("");
    $("#HttpMethod").val("");
    $("#HttpStatusCode").val("");
    $("#ApplicationName").val("");
    $("#ClientIpAddress").val("");
    $("#CorrelationId").val("");
    $("#HasException").val("");
    
    _dataTable.ajax.reload(null, false);
});
