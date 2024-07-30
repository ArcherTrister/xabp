(function ($) {

    let l = abp.localization.getResource('Saas');

    abp.modals.SaaSTenantCreate = function () {

        let togglePasswordVisibility = function () {
            $("#PasswordVisibilityButton").click(function (e) {
                let button = $(this);
                let passwordInput = button.parent().find("input");
                if (!passwordInput) {
                    return;
                }

                if (passwordInput.attr("type") === "password") {
                    passwordInput.attr("type", "text");
                }
                else {
                    passwordInput.attr("type", "password");
                }

                let icon = button.find("i");
                if (icon) {
                    icon.toggleClass("fa-eye-slash").toggleClass("fa-eye");
                }
            });
        }

        let initModal = function (publicApi, args) {

            let singleDatePickers = $('.singledatepicker');
            singleDatePickers.attr('autocomplete', 'off');
            moment()._locale.preparse = (string) => string;
            moment()._locale.postformat = (string) => string;

            singleDatePickers.each(function () {
                let $this = $(this);
                $this.daterangepicker({
                    "singleDatePicker": true,
                    "showDropdowns": true,
                    "autoUpdateInput": false,
                    "autoApply": true,
                    "opens": "center",
                    "drops": "auto",
                    "timePicker": true,
                }).on('apply.daterangepicker', function (ev, picker) {
                    $this.val(picker.startDate.format('YYYY-MM-DD HH:mm'));
                });
                if ($this.val()) {
                    let isoDate = moment($this.val());
                    $this.val(isoDate.format('YYYY-MM-DD HH:mm'));
                    $this.data('daterangepicker').setStartDate(isoDate);
                }
            });

            publicApi.getModal()
                .find('input[name="Tenant.UseSharedDatabase"]')
                .change(function () {
                    let $this = $(this);
                    $("#Tenant_ConnectionStrings_Wrap").toggleClass("d-none");
                    $this.val($this.prop("checked"));
                });

            publicApi.getModal()
                .find('input[name="Tenant.UseSpecificDatabase"]')
                .change(function () {
                    let $this = $(this);
                    $("#Tenant_SpecificDatabase_Wrap").toggleClass("d-none");
                    $this.val($this.prop("checked"));
                });

            publicApi.getModal()
                .find('#AddDatabaseConnectionString')
                .click(function () {
                    let DatabaseNameSelect = $("#DatabaseName");
                    let databaseName = DatabaseNameSelect.val();

                    let databaseConnectionStringInput = $("#ConnectionString");
                    let databaseConnectionString = databaseConnectionStringInput.val();
                    if (!databaseName || !databaseConnectionString) {
                        databaseConnectionStringInput.focus();
                        return;
                    }

                    let databaseNameElement = $("<td></td>").text(databaseName).append($("<input type='hidden'>").val(databaseName));
                    let databaseConnectionStringElement = $("<td></td>").text(databaseConnectionString).append($("<input type='hidden'>").val(databaseConnectionString))
                    let databaseDeleteElement = $('<td><button type="button" class="btn btn-danger btn-sm"><i class="fa fa-trash"></i></button></td>');

                    if ($("table > tbody > tr").length === 2 && $("table > tbody > tr").find("td:first").html().trim() === '') {
                        $("#ConnectionStringsTBody").children("tr").remove();
                    }

                    $("#ConnectionStringsTBody").append($('<tr></tr>').append(databaseNameElement).append(databaseConnectionStringElement).append(databaseDeleteElement))

                    databaseConnectionStringInput.val("");
                    DatabaseNameSelect.find("option[value=" + databaseName + "]").remove()

                    $.each(publicApi.getForm().find("tbody tr"), function (index, tr) {
                        let inputs = $(tr).find("input[type='hidden']");
                        inputs.first().attr("name", "Tenant.ConnectionStrings.Databases[" + index + "].DatabaseName");
                        inputs.last().attr("name", "Tenant.ConnectionStrings.Databases[" + index + "].ConnectionString");
                    });
                });

            publicApi.getModal()
                .find('#CheckDatabaseConnectionString')
                .click(function () {
                    let databaseConnectionStringInput = $("#ConnectionString");
                    let databaseConnectionString = databaseConnectionStringInput.val();
                    if (!databaseConnectionString) {
                        databaseConnectionStringInput.focus();
                        return;
                    }
                    x.saas.tenant.checkConnectionString(databaseConnectionString).then(function (result) {
                        if (result) {
                            abp.notify.success(l('ValidConnectionString'));
                        }
                        else {
                            abp.notify.error(l('InvalidConnectionString'));
                        }
                    });
                });

            publicApi.getModal()
                .find('#ConnectionStringsTBody')
                .on("click", "button", function () {
                    let vaule = $(this).parents("tr").find("input[type='hidden']")[0].value;
                    if ($("table > tbody > tr").length == 2) {
                        $("#ConnectionStringsTBody").append($('<tr><td></td><td><h6>' + l('NoDataAvailable') + '</h6></td><td></td></tr>'));
                    }
                    let option = $("<option></option>").val(vaule).text(vaule);
                    $("#DatabaseName").append(option);
                    $(this).parents("tr").remove();
                });


            $("#CheckConnectionString").click(function () {

                let connectionStringInput = $("#Tenant_ConnectionStrings_Default");
                let connectionString = connectionStringInput.val();
                if (!connectionString) {
                    connectionStringInput.focus();
                    return;
                }

                x.saas.tenant.checkConnectionString(connectionString).then(function (result) {
                    if (result) {
                        abp.notify.success(l('ValidConnectionString'));
                    }
                    else {
                        abp.notify.error(l('InvalidConnectionString'));
                    }
                });
            });

            publicApi.getModal()
                .find('#Tenant_ActivationState')
                .change(function () {
                    let $this = $(this);
                    if ($this.val() === '1') {
                        let endDate = $('#Tenant_ActivationEndDate');
                        endDate.attr("required", true);
                        endDate.parent().parent().show();
                    }
                    else {
                        let endDate = $('#Tenant_ActivationEndDate');
                        endDate.attr("required", false);
                        endDate.parent().parent().hide();
                    }
                }).change();
        };

        togglePasswordVisibility();

        return {
            initModal: initModal
        };
    };

})(jQuery);
