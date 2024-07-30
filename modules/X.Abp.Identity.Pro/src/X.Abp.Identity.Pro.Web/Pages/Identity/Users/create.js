var abp = abp || {};
$(function () {
    abp.modals.createUser = function () {
        var togglePasswordVisibility = function () {
            $("#PasswordVisibilityButton").click(function (e) {
                var button = $(this);
                var passwordInput = button.parent().find("input");
                if(!passwordInput) {
                    return;
                }

                if(passwordInput.attr("type") === "password") {
                    passwordInput.attr("type", "text");
                }
                else {
                    passwordInput.attr("type", "password");
                }

                var icon = button.find("i");
                if(icon) {
                    icon.toggleClass("fa-eye-slash").toggleClass("fa-eye");
                }
            });
        }
        
        var initModal = function (publicApi, args) {
            var $form = publicApi.getForm();

            $("#JsTreeCheckable").on("check_node.jstree uncheck_node.jstree", function (e, data) {
                $('#' + data.node.a_attr["checkbox-id"]).prop("checked", (data.node.state.checked));
                $('#' + data.node.a_attr["checkbox-id"]).val(data.node.state.checked ? "True" : "False");
            });

            $('#JsTreeCheckable').jstree({
                "checkbox": {
                    "keep_selected_style": false,
                    "three_state": false,
                    "tie_selection": false
                },
                "plugins": ["checkbox"]
            });

            togglePasswordVisibility();
        };
        return {
            initModal: initModal
        };
    };
});
