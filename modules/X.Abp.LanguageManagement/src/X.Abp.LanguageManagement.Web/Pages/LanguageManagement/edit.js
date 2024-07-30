var abp = abp || {};
$(function () {
    abp.modals.languageEdit = function () {
        var initModal = function (publicApi, args) {
            var $form = publicApi.getForm();

            function addFlag(state) {
                var $state = $(
                    '<span class="fi fi-' + state.id + '  fis"> </span><span> ' + state.text + '</span>'
                );
                return $state;
            };

            $(".flag-select").select2({
                templateResult: addFlag,
                minimumResultsForSearch: -1
            });

            $form.find(".select2-container").addClass("d-block");
        };

        return {
            initModal: initModal
        };
    };
});