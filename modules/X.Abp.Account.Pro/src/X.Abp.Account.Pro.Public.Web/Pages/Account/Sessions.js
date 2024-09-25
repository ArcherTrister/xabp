var _dataTable = null;

var l = abp.localization.getResource("AbpAccount");

var _accountSession = volo.abp.account.accountSession;

var _detailModal = new abp.ModalManager(abp.appPath + 'Account/SessionDetail');

abp.ui.extensions.entityActions.get("account.sessions").addContributor(
  function (actionList) {
    return actionList.addManyTail(
      [
        {
          text: l("Session:Detail"),
          action: function (data) {
            _detailModal.open({
              id: data.record.id
            });
          }
        },
        {
          text: l("Session:Revoke"),
          confirmMessage: function (data) {
            return l('SessionRevokeConfirmationMessage');
          },
          action: function (data) {
            _accountSession.revoke(data.record.id).then(function () {
              if (data.record.isCurrent) {
                location.assign("/");
              }
              else {
                _dataTable.ajax.reload();
              }
            });
          }
        }
      ]
    );
  }
);

abp.ui.extensions.tableColumns.get("account.sessions").addContributor(
  function (columnList) {
    columnList.addManyTail([
      {
        title: l("Actions"),
        rowAction:
        {
          items: abp.ui.extensions.entityActions.get("account.sessions").actions.toArray()
        }
      },
      {
        title: l("Session:Device"),
        data: "device",
        autoWidth: true,
        render: function (data, type, row) {
          var text = data;
          if (row.isCurrent) {
            text += ' <i class="fas fa-dot-circle" data-bs-toggle="tooltip" data-bs-placement="right" data-bs-title="' + l("Session:Current") + '"></i>';
          }
          return text;
        },
      },
      {
        title: l("Session:DeviceInfo"),
        data: "deviceInfo",
        autoWidth: true
      },
      {
        title: l("Session:SignedIn"),
        data: "signedIn",
        dataFormat: "datetime",
        autoWidth: true
      },
      {
        title: l("Session:LastAccessed"),
        data: "lastAccessed",
        dataFormat: "datetime",
        autoWidth: true
      }
    ]);
  },
  0 //adds as the first contributor
);

$(function () {
  _dataTable = $("#SessionsTable").DataTable(
    abp.libs.datatables.normalizeConfiguration({
      processing: true,
      serverSide: true,
      searching: false,
      scrollX: true,
      paging: true,
      order: [[4, "desc"]],
      ajax: abp.libs.datatables.createAjax(
        _accountSession.getList
      ),
      columnDefs: abp.ui.extensions.tableColumns
        .get("account.sessions")
        .columns.toArray(),
    })
  );
});
