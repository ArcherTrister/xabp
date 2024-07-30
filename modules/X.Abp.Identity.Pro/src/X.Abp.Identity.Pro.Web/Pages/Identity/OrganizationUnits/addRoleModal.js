(function ($) {

    var l = abp.localization.getResource('AbpIdentity');
    var _organizationoUntAppService =  x.abp.identity.organizationUnit;

    var _modal = null;

    abp.ui.extensions.tableColumns.get("organization.unit.roles").addContributor(
        function (columnList) {
            columnList.addManyTail(
                [
                    {
                        title: ou.checkbox.getSelectAllHeaderTitle(),
                        data: null,
                        orderable: false,
                        autoWidth: false,
                        defaultContent: '',
                        className: 'text-center',
                        render: function (data) {
                            return '<div class="form-check no-height">' +
                                `<input type="checkbox" id="${data.id}" name="select-role-cb" class="form-check-input">` +
                                `<label class="form-check-label" for="${data.id}"></label>` +
                                '</div >';
                        }
                    },
                    {
                        title: l('RoleName'),
                        data: "name",
                        render: function (data, type, row) {
                            var name = '<span>' + $.fn.dataTable.render.text().display(data) + '</span>'; //prevent against possible XSS

                            if (row.isDefault) {
                                name += '<span class="badge rounded-pill bg-success ms-1">' + l('DisplayName:IsDefault') + '</span>';
                            }

                            if (row.isPublic) {
                                name += '<span class="badge rounded-pill bg-info ms-1">' + l('DisplayName:IsPublic') + '</span>';
                            }

                            return name;
                        }
                    }
                ]
            );
        }
    );

    abp.modals.AddRoleModalManagement = function () {

        var initModal = function (publicApi, args) {
            _modal = publicApi;

            _dataTable = $('#IdentityRolesModalWrapper table')
                .DataTable(abp.libs.datatables.normalizeConfiguration({
                    order: [[1, "asc"]],
                    searching: false,
                    processing: true,
                    scrollX: true,
                    serverSide: true,
                    paging: true,
                    ajax: abp.libs.datatables.createAjax(_organizationoUntAppService.getAvailableRoles, () => {
                        return {
                            id: args.organizationUnitId
                        }
                    }),
                    columnDefs: abp.ui.extensions.tableColumns.get("organization.unit.roles").columns.toArray(),
                    "initComplete": function (settings, json) {
                        // _dataTable.columns().draw();
                        setTimeout(function () {
                            _dataTable.columns().draw();
                        }, 250);
                    },
                    "fnRowCallback": function (nRow, aData) {
                        let $cb = $(nRow).find('input');
                        if (ou.checkbox.getSelectedIds().includes(aData.id)) {
                            $cb.prop("checked", true);
                        }
                        $(nRow).addClass('selectable');
                    }
                }));

            ou.checkbox.initialize("select-role-cb");
        };

        return {
            initModal: initModal
        };
    };

    $(document).on('click', '#AddRolesToOuButton', function () {
        ou.roles.add(ou.checkbox.getSelectedIds());
        _modal.close();
    });

})(jQuery);
