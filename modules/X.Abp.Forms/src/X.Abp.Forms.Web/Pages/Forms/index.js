(function () {
    var l = abp.localization.getResource('Forms');

    var _formAppService = x.forms.forms.form;

    var _createModal = new abp.ModalManager(abp.appPath + 'Forms/CreateModal');
    var _sendModal = new abp.ModalManager(abp.appPath + 'Forms/SendModal');

    var _dataTable = null;

    abp.ui.extensions.entityActions.get("forms.form").addContributor(
        function (actionList) {
            return actionList.addManyTail(
                [
                    {
                        text: l('View'),
                        action: function (data) {
                            location.href = `/Forms/${data.record.id}/Questions`;
                        }
                    },
                    {
                        text: l('Menu:Send'),
                        action: function (data) {
                            _sendModal.open({
                                id: data.record.id
                            });
                        }
                    },
                    {
                        text: l('Delete'),
                        visible: abp.auth.isGranted('Forms.Form.Delete'),
                        confirmMessage: function (data) {
                            return l('FormDeletionConfirmationMessage', data.record.name);
                        },
                        action: function (data) {
                            _formAppService
                                .delete(data.record.id)
                                .then(function () {
                                    abp.notify.success(l("DeletedSuccessfully"));
                                    _dataTable.ajax.reload(null, false);
                                });
                        }
                    }
                ]
            );
        }
    );

    abp.ui.extensions.tableColumns.get("forms.form").addContributor(
        function (columnList) {
            columnList.addManyTail(
                [
                    {
                        title: l('Actions'),
                        rowAction: {
                            items: abp.ui.extensions.entityActions.get("forms.form").actions.toArray()
                        }
                    },
                    {
                        title: l('Title'),
                        data: "title"
                    },
                    {
                        title: l('Description'),
                        data: "description",
                        render: function (data, type, row) {
                            return $.fn.dataTable.render.text().display(data);
                        }
                    },
                    {
                        title: l('UpdatedAt'),
                        data: "lastModificationTime",
                        render: function (data, type, row) {
                            if (data == null) {
                                return "";
                            }
                            const date = new Date(data);
                            const dateTimeFormatOptions = {
                                year: 'numeric',
                                month: 'short',
                                day: '2-digit',
                                hour: '2-digit',
                                minute: '2-digit'
                            }
                            return new Intl.DateTimeFormat(Intl.DateTimeFormat().resolvedOptions().locale, dateTimeFormatOptions).format(date);
                        }
                    },
                    {
                        title: l('CreatedAt'),
                        data: "creationTime",
                        render: function (data, type, row) {
                            const date = new Date(data);
                            const dateTimeFormatOptions = {
                                year: 'numeric',
                                month: 'short',
                                day: '2-digit',
                                hour: '2-digit',
                                minute: '2-digit'
                            }
                            return new Intl.DateTimeFormat(Intl.DateTimeFormat().resolvedOptions().locale, dateTimeFormatOptions).format(date);
                        }
                    }
                ]
            );
        },
        0 //adds as the first contributor
    );

    $(function () {

        var getFilter = function () {
            return {
                filter: $('#FormsWrapper input.page-search-filter-text').val()
            };
        };

        _dataTable = $('#FormsWrapper table')
            .DataTable(abp.libs.datatables.normalizeConfiguration({
                order: [[3, "desc"]],
                searching: true,
                processing: true,
                scrollX: true,
                serverSide: true,
                paging: true,
                ajax: abp.libs.datatables.createAjax(_formAppService.getList, getFilter),
                columnDefs: abp.ui.extensions.tableColumns.get("forms.form").columns.toArray()
            }));

        $('#NewFormButton').click(function (e) {
            e.preventDefault();
            _createModal.open();
        });

        _createModal.onResult(function (event, args) {
            const newQuestionUrl = new URL(args.responseText, location.href);            
            location.href = newQuestionUrl.href;
            // _dataTable.ajax.reload(null, false);
        });
    })

})();
