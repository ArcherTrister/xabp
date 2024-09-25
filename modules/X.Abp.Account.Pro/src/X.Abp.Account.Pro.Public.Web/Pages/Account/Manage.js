(function ($) {
    $(function () {
        var currentTab = $(window.location.hash + "-tab");
        if (currentTab.length) {
            (new bootstrap.Tab(currentTab)).show()
            location.hash = "";
        }
    });
})(jQuery);
