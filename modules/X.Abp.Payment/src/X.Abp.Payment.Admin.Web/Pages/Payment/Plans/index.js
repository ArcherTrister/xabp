$(function () {
    var l = abp.localization.getResource("Payment");

    var createModal = new abp.ModalManager({ viewUrl: abp.appPath + "Payment/Plans/CreateModal", modalClass: 'createPlan' });
    var updateModal = new abp.ModalManager({ viewUrl: abp.appPath + "Payment/Plans/UpdateModal", modalClass: 'updatePlan' });

    var service = x.payment.admin.plans.planAdmin;

    abp.ui.extensions.entityActions.get("payment.plan").addContributor(
        function(actionList) {
            actionList.addManyTail(
                [
                    {
                        text: l('GatewayPlans'),
                        visible: abp.auth.isGranted('Payment.Plans.GatewayPlans'),
                        action: function (data) {
                            location.href = 'plans/'+ data.record.id + '/external-plans';
                        }
                    },
                    {
                        text: l('Edit'),
                        visible: abp.auth.isGranted('Payment.Plans.Update'),
                        action: function (data) {
                            updateModal.open({ id: data.record.id });
                        }
                    },
                    {
                        text: l('Delete'),
                        visible: abp.auth.isGranted('Payment.Plans.Delete'),
                        confirmMessage: function (data) {
                            return l("PlanDeletionConfirmationMessage")
                        },
                        action: function (data) {
                            service
                                .delete(data.record.id)
                                .then(function () {
                                    dataTable.ajax.reload();
                                });
                        }
                    }
                ]
            )
        }
    )
    
    abp.ui.extensions.tableColumns.get("payment.plan").addContributor(
        function(columnList) {
            columnList.addManyTail(
                [
                    {
                        title: l("Actions"),
                        targets: 0,
                        rowAction: {
                            items: abp.ui.extensions.entityActions.get("payment.plan").actions.toArray()
                        }
                    },
                    {
                        title: l("DisplayName:Name"),
                        orderable: true,
                        data: "name"
                    }
                ]
            );
        },
        0 //adds as the first contributor
    );

    var dataTable = $("#PlansTable").DataTable(abp.libs.datatables.normalizeConfiguration({
        processing: true,
        serverSide: true,
        paging: true,
        searching: false,
        scrollCollapse: true,
        scrollX: true,
        ordering: true,
        order: [[1, "desc"]],
        ajax: abp.libs.datatables.createAjax((input, ajaxParams) =>
        {
            input.filter = getFilter();
            return service.getList(input, ajaxParams);
        }),
        columnDefs: abp.ui.extensions.tableColumns.get("payment.plan").columns.toArray()
    }));

    function getFilter() {
        return  $('#PlansSearchWrapper input.page-search-filter-text').val();
    }

    $('#PlansSearchWrapper form.page-search-form').submit(function (e) {
        e.preventDefault();
        dataTable.ajax.reload();
    });

    $('#AbpContentToolbar button[name=CreatePlan]').on('click', function (e) {
        e.preventDefault();
        createModal.open();
    });

    createModal.onResult(function () {
        dataTable.ajax.reload();
    });

    updateModal.onResult(function () {
        dataTable.ajax.reload();
    });
});
