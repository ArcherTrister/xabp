(function () {
    var l = abp.localization.getResource("CmsKit");

    var newsletterService = x.cmsKit.public.newsletters.newsletterRecordPublic;

    var urlParams = new URLSearchParams(window.location.search);

    var preferenceSelectors = document.querySelectorAll('.newsletter-preference-check');

    $('#selectAll').click(function (event) {
        if (checked) {
            $('.newsletter-preference-check').each(function () {
                $(this).prop('checked', true);
            });
        } else {
            $('.newsletter-preference-check').each(function () {
                $(this).prop('checked', false);
            });
        }
    });

    $(window).on("load", "", function () {
        if ($('.newsletter-preference-check:checked').length === $('.newsletter-preference-check').length) {
            $('#selectAll').prop('checked', true);
        } else {
            $('#selectAll').prop('checked', false);
        }
    });

    $(".newsletter-preference-check").on("change", "", function () {
        if ($('.newsletter-preference-check:checked').length === $('.newsletter-preference-check').length) {
            $('#selectAll').prop('checked', true);
        } else {
            $('#selectAll').prop('checked', false);
        }
    });

    $('form#newsletterUpdate').submit(function (e) {
        e.preventDefault();
        var source = window.location.pathname;
        var sourceUrl = window.location.href;
        var emailAddress = $('#newsletter-manage').data('emailaddress');
        var securityCode = urlParams.get('securityCode');

        var preferenceDetails = [];
        preferenceSelectors.forEach(function (value) {
            preferenceDetails.push({preference: value.getAttribute('data-preference'), isEnabled: value.checked});
        });

        newsletterService.updatePreferences(
            {
                emailAddress: emailAddress,
                preferenceDetails: preferenceDetails,
                source: source,
                sourceUrl: sourceUrl,
                securityCode: securityCode
            })
            .then(function (r) {
                abp.message.success(l("UpdatePreferenceSuccessMessage"))
                    .then(function () {
                        window.location.reload();
                    });
            });
    });
})(jQuery);
