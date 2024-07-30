(function () {
    var button = document.querySelector("#cookieConsent button.accept-cookie[data-cookie-string]");

    if (button) {
        button.addEventListener("click", function (event) {
            document.cookie = button.dataset.cookieString;
            var cookieContainer = document.querySelector("#cookieConsent");
            cookieContainer.remove();
        }, false);
    }
})();
