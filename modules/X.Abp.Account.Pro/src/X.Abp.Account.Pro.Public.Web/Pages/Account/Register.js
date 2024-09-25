$(function () {

    var isRecaptchaEnabled = typeof grecaptcha !== 'undefined';
    if (isRecaptchaEnabled) {
        grecaptcha.ready(function () {
            $("#registerForm button[type=submit]").removeAttr("disabled");
        });
    } else {
        $("#registerForm button[type=submit]").removeAttr("disabled");
    }

    $("#registerForm button[type=submit]").click(function (e) {
        e.preventDefault();
        var form = $("#registerForm");
        if (form.valid() && isRecaptchaEnabled && abp.utils.isFunction(grecaptcha.reExecute)) {
            grecaptcha.reExecute(function (token) {
                form.find("input[type=hidden][data-captcha=true]").val(token);
                form.submit();
            })
        } else {
            form.submit();
        }
    });

    let button = $(this);
    let passwordInput = $('#password-input');

    passwordInput.passwordComplexityIndicator({
        insideParent: true
    });

    $("#PasswordVisibilityButton").click(function (e) {
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
            passwordMsg.style = e.getModifierState('CapsLock') ? 'display: inline' : 'display: none';
        });
    }
});
