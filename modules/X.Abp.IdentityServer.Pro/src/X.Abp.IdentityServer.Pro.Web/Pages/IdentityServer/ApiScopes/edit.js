var abp = abp || {};
$(function () {
    abp.modals.apiScopeUpdate = function () {
        var initModal = function (publicApi, args) {
            var $form = publicApi.getForm();
            var l = abp.localization.getResource('AbpIdentityServer');

            $(document).on('click', '.otherClaim', function (event) {

                event.preventDefault();

                var otherId = $(this).attr("id");
                var claimName = $(this).attr("claim-name");
                var name = otherId.substring(0, otherId.length - "OtherId".length);
                var inputId = name + "InputId";
                var ownedId = name + "OwnedId";

                $("#" + ownedId).show();
                $("#" + otherId).hide();
                $("#" + inputId).val(claimName);
            });

            $(document).on('click', '.ownedClaim', function (event) {

                event.preventDefault();

                var ownedId = $(this).attr("id");
                var name = ownedId.substring(0, ownedId.length - "OwnedId".length);
                var inputId = name + "InputId";
                var otherId = name + "OtherId";

                $("#" + otherId).show();
                $("#" + ownedId).hide();
                $("#" + inputId).val("");
            });


            //Properties

            var propertyIndex = $('#PropertyStartIndex').val();
            var propertyCount = $('#PropertyStartIndex').val();

            var getPropertyTableRow = function (key, value) {
                return "<tr>\r\n<td>\r\n" +
                    "                                        " + key + " </td><td>\r\n" +
                    "                                        " + value + "\r\n </td> <td>" +
                    "                                       <input type=\"text\" hidden name=\"ApiScope.Properties[" + propertyIndex + "].Key\"/ value=\"" + key + "\" id=\"" + key + "PropertyInput\">\r\n " +
                    "                                       <input type=\"text\" hidden name=\"ApiScope.Properties[" + propertyIndex + "].Value\" value=\"" + value + "\"/>\r\n " +
                    "                                       <button type=\"button\" class=\"btn btn-outline-danger float-end deletePropertyButton\"><i class=\"fa fa-trash\"></i></button>" +
                    "</td></tr>";
            }

            $("#AddNewPropertyButton").on('click', function (event) {

                event.preventDefault();

                var key = $("#SampleProperty_Key").val();
                var value = $("#SampleProperty_Value").val();

                if (!key) {
                    abp.message.warn(abp.utils.formatString(l("MissingRequiredField"), l("Key")));
                    return;
                }

                if (!value) {
                    abp.message.warn(abp.utils.formatString(l("MissingRequiredField"), l("Value")));
                    return;
                }

                if ($('#' + key + 'PropertyInput').length > 0) {
                    //TODO: notification?
                    return;
                }

                $("#SampleProperty_Value").val("");
                $("#SampleProperty_Key").val("");

                var html = getPropertyTableRow(key, value);

                $("#PropertyTableBodyId").append(html);

                propertyIndex++;
                propertyCount++;
                $("#PropertyTableId").show();
            });

            $(document).on('click', '.deletePropertyButton', function (event) {

                event.preventDefault();

                var tag = $(this).parent().parent();

                var inputs = tag.find("input");
                $(inputs).each(function (i) {
                    $(this).val("");
                });

                tag.hide();
                propertyCount--;
            });
        };

        return {
            initModal: initModal
        };
    };
});
