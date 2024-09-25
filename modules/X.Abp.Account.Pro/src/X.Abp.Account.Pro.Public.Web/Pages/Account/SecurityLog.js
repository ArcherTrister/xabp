var _dataTable = null;

var l = abp.localization.getResource("AbpAccount");

var _account = volo.abp.account.account;

abp.ui.extensions.tableColumns.get("account.mySecurityLogs").addContributor(
  function (columnList) {
    columnList.addManyTail([
      {
        title: l("MySecurityLogs:Time"),
        data: "creationTime",
        dataFormat: "datetime",
      },
      {
        title: l("MySecurityLogs:Action"),
        data: "action",
        autoWidth: true,
        render: function (data) {
          data = $.fn.dataTable.render.text().display(data || "");
          return (
            '<span class="datatableCell" data-filter-field="Action">' +
            data +
            "</span>"
          );
        },
      },
      {
        title: l("MySecurityLogs:IpAddress"),
        data: "clientIpAddress",
        autoWidth: true,
        orderable: false,
      },
      {
        title: l("MySecurityLogs:Browser"),
        data: "browserInfo",
        autoWidth: true,
        orderable: false,
        render: function (data) {
          data = $.fn.dataTable.render.text().display(data || "");

          var maxChars = 20;

          if (data.length > maxChars) {
            return (
              '<span data-toggle="tooltip" title="' +
              data +
              '">' +
              data.substring(0, maxChars) +
              "..." +
              "</span>"
            );
          } else {
            return data;
          }
        },
      },
      {
        title: l("MySecurityLogs:Application"),
        data: "applicationName",
        autoWidth: true,
        render: function (data) {
          return $.fn.dataTable.render.text().display(data || "");
        },
      },
      {
        title: l("MySecurityLogs:Identity"),
        data: "identity",
        autoWidth: true,
        render: function (data) {
          return $.fn.dataTable.render.text().display(data || "");
        },
      },
      {
        title: l("MySecurityLogs:Client"),
        data: "clientId",
        autoWidth: true,
        render: function (data) {
          return $.fn.dataTable.render.text().display(data || "");
        },
      },
    ]);
  },
  0 //adds as the first contributor
);

$(function () {
  _dataTable = $("#MySecurityLogsTable").DataTable(
    abp.libs.datatables.normalizeConfiguration({
      processing: true,
      serverSide: true,
      paging: true,
      scrollX: true,
      searching: false,
      autoWidth: false,
      scrollCollapse: true,
      order: [[0, "desc"]],
      ajax: abp.libs.datatables.createAjax(
          _account.getSecurityLogList,
        function () {
          return $("#FilterFormId").serializeFormToObject();
        }
      ),
      columnDefs: abp.ui.extensions.tableColumns
        .get("account.mySecurityLogs")
        .columns.toArray(),
    })
  );

  $(document).on("click", ".datatableCell", function () {
    $("#" + $(this).attr("data-filter-field")).val($(this).text());
    $("#FilterFormId").submit();
  });

  $("#FilterFormId").submit(function (e) {
    e.preventDefault();
    _dataTable.ajax.reloadEx();
  });
  
  $("abp-date-picker").on("change", function () {
    _dataTable.ajax.reloadEx();
  });
});