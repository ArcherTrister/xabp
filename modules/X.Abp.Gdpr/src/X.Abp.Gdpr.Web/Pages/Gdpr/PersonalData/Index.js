$(function () {
    var l = abp.localization.getResource("AbpGdpr");

    var gdprRequestService = x.abp.gdpr.gdprRequest;

    var getFilter = function () {
        return {
            userId: abp.currentUser.id,
        }
    }

    var dataTable = $("#PersonalDataTable").DataTable(
        abp.libs.datatables.normalizeConfiguration({
            processing: true,
            serverSide: true,
            paging: true,
            scrollX: true,
            searching: false,
            autoWidth: false,
            scrollCollapse: true,
            ajax: abp.libs.datatables.createAjax(gdprRequestService.getList, getFilter),
            columnDefs: [
                {
                    title: l("Action"),
                    data: null,
                    render: function (data) {
                        var isDataPrepared = Date.now() > new Date(data.readyTime);
                        return isDataPrepared
                            ? '<button class="btn btn-primary personal-data-download-btn" data-id="' + data.id + '">' + l("Download") + '</button>'
                            : '<span class="badge bg-warning">' + l("Preparing") + '</span';
                    }
                },
                {
                    title: l("CreationTime"),
                    data: "creationTime",
                    dataFormat: "datetime"
                },
                {
                    title: l("ReadyTime"),
                    data: "readyTime",
                    dataFormat: "datetime"
                }
            ]
        })
    );

    $('#PersonalDataTable tbody').on('click', 'button', function () {
        var personalDataDownloadBtn = $(this);
        var requestId = personalDataDownloadBtn.data("id");

        gdprRequestService.getDownloadToken(requestId)
            .then(function (data) {
                window.location = abp.appPath + "api/gdpr/requests/data/" + requestId + "?token=" + data.token;
            });
    });

    $("#PersonalDataForm").submit(function (e) {
        e.preventDefault();

        gdprRequestService.prepareUserData()
            .then(function () {
                abp.message.success(l("PersonalDataPrepareRequestReceived"))
                    .then(function () {
                        dataTable.ajax.reload();
                        window.location.reload();
                    });
            });
    });

    $("#PersonalDataDeleteBtn").click(function (e) {
        e.preventDefault();

        abp.message.confirm(l("DeletePersonalDataWarning"))
            .then(function (confirmed) {
                if (confirmed) {
                    gdprRequestService.deleteUserData()
                        .then(function () {
                            abp.message.success(l("PersonalDataDeleteRequestReceived"))
                                .then(function () {
                                    dataTable.ajax.reload();
                                });
                        });
                }
            });
    });
});
