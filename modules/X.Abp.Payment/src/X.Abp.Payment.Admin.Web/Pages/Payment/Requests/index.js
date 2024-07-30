$(function () {
    var l = abp.localization.getResource("Payment");

    var service = x.payment.admin.requests.paymentRequestAdmin;
	
    var minDate = "";
    var maxDate = "";

    var $inputTime = $("#inputTime");
    $inputTime.daterangepicker({
        autoUpdateInput: false,
        locale: {
            cancelLabel: 'Clear'
        }
    });

    $inputTime.on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('MM/DD/YYYY') + ' - ' + picker.endDate.format('MM/DD/YYYY'));
        minDate = picker.startDate.format('MM/DD/YYYY')
        maxDate = picker.endDate.format('MM/DD/YYYY')
    });

    $inputTime.on('cancel.daterangepicker', function (ev, picker) {
        $(this).val('');
        minDate = ""
        maxDate = ""
    });

    var dataTable = $("#PaymentRequestsTable").DataTable(abp.libs.datatables.normalizeConfiguration({
        processing: true,
        serverSide: true,
        paging: true,
        searching: false,
        scrollCollapse: true,
        scrollX: true,
        ordering: true,
        order: [[1, "desc"]],
        ajax: abp.libs.datatables.createAjax((input, ajaxParams) => {
            return service.getList(getFilter(input), ajaxParams);
        }),
        columnDefs: [
            {
                title: l("Actions"),
                targets: 0,
                rowAction: {
                    items: [
                        {
                            text: l('Products'),
                            visible: abp.auth.isGranted('Payment.PaymentRequests'),
                            action: function (data) {
                                location.href = 'Requests/' + data.record.id + '/products';
                            }
                        }
                    ]
                }
            },
            {
                title: l("DisplayName:CreationTime"),
                orderable: true,
                data: "creationTime",
                dataFormat: "datetime"
            },
            {
                title: l("DisplayName:TotalPrice"),
                orderable: false,
                data: "totalPrice",
                render: function (data, opts, dto){
                    if (dto.externalSubscriptionId)
                    {
                        return '<i class="fa fa-info" title="' + l("PriceSubscriptionTooltip") +'" data-toggle="tooltip"></i>'
                    }
                    
                    return data;
                }
            },
            {
                title: l("DisplayName:Currency"),
                orderable: true,
                data: "currency"
            },
            {
                title: l("DisplayName:State"),
                orderable: true,
                render: function (data) {
                    return l("Enum:PaymentRequestState:" + data)
                },
                data: "state"
            },
            {
                title: l("DisplayName:Gateway"),
                orderable: true,
                data: "gateway"
            },
            {
                title: l("DisplayName:ExternalSubscriptionId"),
                orderable: true,
                data: "externalSubscriptionId"
            }
        ]
    }));

    function getFilter(input) {
        let form = $("#PaymentRequestFilterForm").serializeFormToObject();
        input.filter = $('#PaymentRequestFilterForm input.page-search-filter-text').val();
        input.creationDateMax = minDate;
        input.creationDateMin = maxDate;
        input.paymentType = form.paymentType;
        input.status = form.status;
        
        return input;
    }

    $("#PaymentRequestFilterForm input,select").change(function (e) {
        dataTable.ajax.reload();
    });

    $("#PaymentRequestFilterForm").submit(function (e) {
        e.preventDefault();
        dataTable.ajax.reload(null, false);
    });
});
