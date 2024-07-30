var abp = abp || {};

abp.modals.newsletterSuccessModal = function () {
    var initModal = function (publicApi, args) {

        var l = abp.localization.getResource("CmsKit");
        
        var $form = $('#newsletterSuccessForm');
        
        var isAdditionalPreferenceLater = $form.attr("data-additional");
        
        $('#newsletterSuccessBtn').on('click', function (e) {
            if(!isAdditionalPreferenceLater){
                abp.event.trigger('newsletterEmailSubmitted');
                publicApi.close();
            }else{
                var additionalPreferences = [];
                var selectedPreferences = $('#additional-preferences').find('.additional-checkbox:checked');

                for (var i = 0; i < selectedPreferences.length; i++) {
                    additionalPreferences[i] = $(selectedPreferences[i]).attr('data-additional-preference');
                }
                
                var payload = {
                    emailAddress: $form.attr('data-email'),
                    preference: $form.attr('data-preference'),
                    source: $form.attr('data-source'),
                    sourceUrl: $form.attr('data-sourceurl'),
                    additionalPreferences: additionalPreferences
                };
                
                x.cmsKit.public.newsletters.newsletterRecordPublic.create(payload).then(function () {
                    abp.event.trigger('newsletterEmailSubmitted');
                    publicApi.close();
                });
            }
        });

    };

    return {
        initModal: initModal
    };
};
