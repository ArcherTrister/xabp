(function ($) {

    var l = abp.localization.getResource('AbpIdentity');

    var _identityUserAppService = x.abp.identity.identityUser;
    var _claimTypeEditModal = new abp.ModalManager({ viewUrl: abp.appPath + 'Identity/Users/ClaimTypeEditModal', modalClass: 'claimTypeEdit' });
    var _editModal = new abp.ModalManager({ viewUrl: abp.appPath + 'Identity/Users/EditModal', modalClass: 'editUser' });
    var _createModal = new abp.ModalManager({ viewUrl: abp.appPath + 'Identity/Users/CreateModal', modalClass: 'createUser' });
    var _permissionsModal = new abp.ModalManager(abp.appPath + 'AbpPermissionManagement/PermissionManagementModal');
    var _changeUserPasswordModal = new abp.ModalManager({ viewUrl: abp.appPath + 'Identity/Users/SetPassword', modalClass: 'changeUserPassword' });
    var _lockoutModal = new abp.ModalManager({ viewUrl: abp.appPath + 'Identity/Users/Lock', modalClass: 'lock' });
    var _twoFactorModal = new abp.ModalManager({ viewUrl: abp.appPath + 'Identity/Users/TwoFactor', modalClass: 'twoFactor' });

    var _dataTable = null;

    abp.ui.extensions.entityActions.get("identity.user").addContributor(
        function (actionList) {
            return actionList.addManyTail(
                [
                    {
                        text: l('Edit'),
                        visible: abp.auth.isGranted('AbpIdentity.Users.Update'),
                        action: function (data) {
                            _editModal.open({
                                id: data.record.id
                            });
                        }
                    },
                    {
                        text: l('Claims'),
                        visible: abp.auth.isGranted('AbpIdentity.Users.Update'),
                        action: function (data) {
                            _claimTypeEditModal.open({
                                id: data.record.id
                            });
                        }
                    },
                    {
                        text: l('Lock'),
                        visible: function(data) {
                            return data.lockoutEnabled && abp.auth.isGranted('AbpIdentity.Users.Update');
                        },
                        action: function (data) {
                            _lockoutModal.open({
                                id: data.record.id
                            });
                        }
                    },
                    {
                        text: l('Unlock'),
                        visible: function (data) {
                            return data.isLockedOut &&
                                abp.auth.isGranted('AbpIdentity.Users.Update'); //TODO: New permission for lockout?
                        },
                        action: function (data) {
                            _identityUserAppService
                                .unlock(data.record.id)
                                .then(function () {
                                    abp.notify.success(l("UserUnlocked"));
                                    _dataTable.ajax.reload(null, false);
                                });
                        }
                    },
                    {
                        text: l('Permissions'),
                        visible: abp.auth.isGranted('AbpIdentity.Users.ManagePermissions'),
                        action: function (data) {
                            _permissionsModal.open({
                                providerName: 'U',
                                providerKey: data.record.id,
                                providerKeyDisplayName: data.record.userName
                            });
                        }
                    },
                    {
                        text: l('SetPassword'),
                        visible: abp.auth.isGranted('AbpIdentity.Users.Update'),
                        action: function (data) {
                            _changeUserPasswordModal.open({
                                id: data.record.id
                            });
                        }
                    },
                    {
                        text: l('TwoFactor'),
                        visible: function (data) {
                            return abp.auth.isGranted('AbpIdentity.Users.Update') && data.supportTwoFactor;
                        },
                        action: function (data) {
                            _twoFactorModal.open({
                                id: data.record.id
                            });
                        }
                    },
                    {
                        text: l('ChangeHistory'),
                        visible: abp.auditLogging !== undefined && abp.auth.isGranted('AuditLogging.ViewChangeHistory:Volo.Abp.Identity.IdentityUser'),
                        action: function (data) {
                            abp.auditLogging.openEntityHistoryModal(
                                "Volo.Abp.Identity.IdentityUser",
                                data.record.id
                            );
                        }
                    },
                    {
                        text: l('Delete'),
                        visible: function (data) {
                            return abp.auth.isGranted('AbpIdentity.Users.Delete') && abp.currentUser.id !== data.id;
                        },
                        confirmMessage: function (data) {
                            return l('UserDeletionConfirmationMessage', data.record.userName);
                        },
                        action: function (data) {
                            _identityUserAppService
                                .delete(data.record.id)
                                .then(function () {
                                    abp.notify.success(l("SuccessfullyDeleted"));
                                    _dataTable.ajax.reload(null, false);
                                });
                        }
                    }
                ]
            );
        }
    );

    abp.ui.extensions.tableColumns.get("identity.user").addContributor(
        function (columnList) {
            columnList.addManyTail(
                [
                    {
                        title: l('Actions'),
                        rowAction: {
                            items: abp.ui.extensions.entityActions.get("identity.user").actions.toArray()
                        }
                    },
                    {
                        title: l('UserName'),
                        autoWidth: false,
                        data: 'userName',
                        render: function (data, type, row) {

                            row.userName = $.fn.dataTable.render.text().display(row.userName);
                            var roleHtml = row.userName;
                            var lockedOutHtml = '';
                            var notActiveHtml = '';
                            if (row.isLockedOut) {
                                lockedOutHtml = '<i data-toggle="tooltip" data-placement="top" title="' +
                                    l('ThisUserIsLockedOutMessage') +
                                    '" class="fa fa-lock text-danger"></i> ';
                            }

                            if (!row.isActive) {
                                notActiveHtml = '<i data-toggle="tooltip" data-placement="top" title="' +
                                    l('ThisUserIsNotActiveMessage') +
                                    '" class="fa fa-ban text-danger"></i> ';
                            }

                            if (row.isLockedOut || !row.isActive) {
                                roleHtml = lockedOutHtml + notActiveHtml + '<span class="opc-65">' + row.userName + '</span>';
                            }

                            return roleHtml;
                        }
                    },
                    {
                        title: l('EmailAddress'),
                        data: "email",
                        render: function (data, type, row) {
                            if (!row.emailConfirmed) {
                                return data;
                            }

                            return data + ' <i class="text-success ms-1 fa fa-check" data-toggle="tooltip" data-placement="top" title="' + l('Verified') + '"></i>';
                        }
                    },
                    {
                        title: l("Roles"),
                        data: "roleNames",
                        render: function (data, type, row) {
                            if (!data || !Array.isArray(data)) {
                                return "";
                            }

                            return data.join(", ");
                        }
                    },
                    {
                        title: l('PhoneNumber'),
                        data: "phoneNumber",
                        render: function (data, type, row) {
                            if (!row.phoneNumberConfirmed) {
                                return data;
                            }

                            return data + ' <i class="text-success ms-1 fa fa-check" data-toggle="tooltip" data-placement="top" title="' + l('Verified') + '"></i>';
                        }
                    },
                    {
                        title: l('Name'),
                        data: "name",
                    },
                    {
                        title: l('Surname'),
                        data: "surname",
                    },
                    {
                        title: l('DisplayName:IsActive'),
                        data: "isActive",
                        render: function (data) {
                            if (data) {
                                return '<i class="fa fa-check"></i>';
                            } else {
                                return '<i class="fa fa-ban"></i>';
                            }
                        }
                    },
                    {
                        title: l('DisplayName:LockoutEnabled'),
                        data: "lockoutEnabled",
                        render: function (data) {
                            if (data) {
                                return '<i class="fa fa-check"></i>';
                            } else {
                                return '<i class="fa fa-ban"></i>';
                            }
                        }
                    },
                    {
                        title: l('DisplayName:EmailConfirmed'),
                        data: "emailConfirmed",
                        render: function (data) {
                            if (data) {
                                return '<i class="fa fa-check"></i>';
                            } else {
                                return '<i class="fa fa-ban"></i>';
                            }
                        }
                    },
                    {
                        title: l('DisplayName:TwoFactorEnabled'),
                        data: "twoFactorEnabled",
                        render: function (data) {
                            if (data) {
                                return '<i class="fa fa-check"></i>';
                            } else {
                                return '<i class="fa fa-ban"></i>';
                            }
                        }
                    },
                    {
                        title: l('DisplayName:AccessFailedCount'),
                        data: "accessFailedCount",
                    },
                    {
                        title: l('CreationTime'),
                        data: "creationTime",
                        dataFormat: "datetime"
                    },
                    {
                        title: l('LastModificationTime'),
                        data: "lastModificationTime",
                        dataFormat: "datetime"
                    }
                ]
            );
        },
        0 //adds as the first contributor
    );

    $(function () {

        var getFilter = function () {
            var roleId = $("#IdentityUsersWrapper select#IdentityRole").val();
            var organizationUnitId = $("#IdentityUsersWrapper select#OrganizationUnit").val();

            return {
                filter: $('#IdentityUsersWrapper input.page-search-filter-text').val(),
                roleId: !roleId ? null : roleId,
                organizationUnitId: !organizationUnitId ? null : organizationUnitId,
                userName: $("#IdentityUsersWrapper input#UserName").val(),
                phoneNumber: $("#IdentityUsersWrapper input#PhoneNumber").val(),
                emailAddress: $("#IdentityUsersWrapper input#EmailAddress").val(),
                isLockedOut: $("#IdentityUsersWrapper input#IsLockedOut").prop('checked') ? true : null,
                notActive: $("#IdentityUsersWrapper input#NotActive").prop('checked') ? true : null
            };
        };

        _dataTable = $('#IdentityUsersWrapper table').DataTable(abp.libs.datatables.normalizeConfiguration({
            order: [[2, "asc"]],
            processing: true,
            serverSide: true,
            searching: false,
            scrollX: true,
            paging: true,
            ajax: abp.libs.datatables.createAjax(_identityUserAppService.getList, getFilter),
            columnDefs: abp.ui.extensions.tableColumns.get("identity.user").columns.toArray()
        }));

        $('#IdentityUsersWrapper table').data('dataTable', _dataTable);

        _createModal.onResult(function () {
            _dataTable.ajax.reload(null, false);
        });

        _editModal.onResult(function () {
            _dataTable.ajax.reload(null, false);
        });

        _lockoutModal.onResult(function () {
            _dataTable.ajax.reload(null, false);
        });

        $('#AbpContentToolbar button[name=CreateUser]').click(function (e) {
            e.preventDefault();
            _createModal.open();
        });

        $('#IdentityUsersWrapper form.page-search-form').submit(function (e) {
            e.preventDefault();
            _dataTable.ajax.reload();
        });

        $("#IdentityUsersWrapper select#IdentityRole, #IdentityUsersWrapper select#OrganizationUnit").change(function (e) {
            _dataTable.ajax.reload();
        });

        $("#AdvancedFilterSectionToggler").click(function (e) {
            $("#AdvancedFilterSection").toggle();
        });

        $("#IdentityUsersWrapper input#UserName, #IdentityUsersWrapper input#EmailAddress, #IdentityUsersWrapper input#PhoneNumber").keypress(function (e) {
            if (e.which === 13) {
                _dataTable.ajax.reload();
            }
        });
    });

})(jQuery);
