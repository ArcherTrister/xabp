$(function () {

    var isRecaptchaEnabled = typeof grecaptcha !== 'undefined';
    if (isRecaptchaEnabled) {
        grecaptcha.ready(function () {
            $("#loginForm button[type=submit]").removeAttr("disabled");
        });
    } else {
        $("#loginForm button[type=submit]").removeAttr("disabled");
    }

    $("#loginForm button[type=submit]").click(function (e) {
        e.preventDefault();
        var form = $("#loginForm");
        if (form.valid() && isRecaptchaEnabled && abp.utils.isFunction(grecaptcha.reExecute)) {
            grecaptcha.reExecute(function (token) {
                form.find("input[type=hidden][data-captcha=true]").val(token);
                form.submit();
            })
        } else {
            form.submit();
        }
    });

    $("#PasswordVisibilityButton").click(function (e) {
        let button = $(this);
        let passwordInput = $('#password-input');
        if (!passwordInput) {
            return;
        }

        if (passwordInput.attr("type") === "password") {
            passwordInput.attr("type", "text");
        }
        else {
            passwordInput.attr("type", "password");
        }

        let icon = $("#PasswordVisibilityButton");
        if (icon) {
            icon.toggleClass("bi-eye-slash").toggleClass("bi-eye");
        }
    });

    // CAPS LOCK CONTROL
    const password = document.getElementById('password-input');
    const passwordMsg = document.getElementById('capslockicon');
    if (password && passwordMsg) {
        password.addEventListener('keyup', e => {
            if (typeof e.getModifierState === 'function') {
                passwordMsg.style = e.getModifierState('CapsLock') ? 'display: inline' : 'display: none';
            }
        });
    }
});
