(function ($) {

    abp.modals.SaaSTenantEdit = function() {

        var initModal = function (publicApi, args) {

            var singleDatePickers = $('.singledatepicker');
            singleDatePickers.attr('autocomplete', 'off');
            moment()._locale.preparse = (string) => string;
            moment()._locale.postformat = (string) => string;

            singleDatePickers.each(function () {
                var $this = $(this);
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
                if($this.val()) {
                    var isoDate = moment($this.val());
                    $this.val(isoDate.format('YYYY-MM-DD HH:mm'));
                    $this.data('daterangepicker').setStartDate(isoDate);
                }
            });

            publicApi.getModal()
                .find('#Tenant_ActivationState')
                .change(function () {
                    var $this = $(this);
                    if($this.val() === '1') {
                        var endDate = $('#Tenant_ActivationEndDate');
                        endDate.attr("required", true);
                        endDate.parent().parent().show();
                    }
                    else {
                        var endDate = $('#Tenant_ActivationEndDate');
                        endDate.attr("required", false);
                        endDate.parent().parent().hide();
                    }
                }).change();
        };

        return {
            initModal: initModal
        };
    };

})(jQuery);
