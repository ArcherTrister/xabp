(function ($) {
    var l = abp.localization.getResource("CmsKit");

    var additionalPreferences = [];

    var successModal = new abp.ModalManager({
        viewUrl: abp.appPath + "Public/Newsletters/SuccessModal",
        scriptUrl: "/Pages/Public/Newsletters/successModal.js",
        modalClass: "newsletterSuccessModal"
    });
    
    abp.widgets.CmsNewsletter = function ($widget) {
        var widgetManager = $widget.data("abp-widget-manager");
        var $newsletterArea = $widget.find(".cms-newsletter-area");
        var $additionalPreferenceSelector = $widget.find('#additional-preference');
        var isRequestSent = false;

        var dataPreferenceValue = $newsletterArea.attr("data-preference");
        var dataSourceValue = $newsletterArea.attr("data-source");
        var normalizedDataSource = dataSourceValue.replace('.', '_');
        
        function getFilters() {
            return {
                preference: dataPreferenceValue,
                source: dataSourceValue,
            };
        }

        var form = "#newsletter-form-" + normalizedDataSource;

        $('#newsletter-submit-button-' + normalizedDataSource).on('click', function () {
            isRequestSent = false; 
        });                                                                  
        
        function init() {
            $widget.find(form).on('submit', '', function (e) {
                e.preventDefault();
                var formAsObject = $(this).serializeFormToObject();
                
                var preference = dataPreferenceValue;
                var sourceUrl = window.location.href;

                if (formAsObject === undefined || !formAsObject.newsletterEmail) {
                    formAsObject = {
                        newsletterEmail: $('#newsletter-email-input-' + normalizedDataSource).val()
                    }
                }

                if ($additionalPreferenceSelector.length) {
                    for (var i = 0; i < $additionalPreferenceSelector[0].children.length; i++) {
                        var additionalPreferenceCheckBox = "#additional-" + $additionalPreferenceSelector[0].children[i].attributes[1].value + "-" + normalizedDataSource;
                        if ($(additionalPreferenceCheckBox).is(":checked")) {
                            additionalPreferences.push($additionalPreferenceSelector[0].children[i].attributes[1].value);
                        }
                    }
                }
                                
                var createPayload = {
                    emailAddress: formAsObject.newsletterEmail,
                    preference: preference,
                    source: dataSourceValue,
                    sourceUrl: sourceUrl,
                    additionalPreferences: additionalPreferences
                };
                
                x.cmsKit.public.newsletters.newsletterRecordPublic.create(createPayload).then(function () {
                    successModal.open({
                        emailAddress: formAsObject.newsletterEmail,
                        preference: preference,
                        source: dataSourceValue,
                        sourceUrl: sourceUrl,
                        requestAdditionalPreferences: $newsletterArea.attr('data-get-preferences-later')
                    });
                    
                });
            });
        }

        return {
            init: init,
            getFilters: getFilters
        }
    };

})(jQuery);
