
$(function () {
    abp.modals.changeTenantPassword = function () {
        let initModal = function (publicApi, args) {
            let $newPasswordInput = $("#NewPasswordInput");
            let generateRandomPasswordButton = $("#GenerateRandomPasswordButton");
            let passwordVisibilityButton = $("#PasswordVisibilityButton");

            let specials = '!@#$%&*()_+{}<>?[]./';
            let lowercase = 'abcdefghijklmnopqrstuvwxyz';
            let uppercase = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ';
            let numbers = '0123456789';

            let all = specials + lowercase + uppercase + numbers;

            generateRandomPasswordButton.click(function () {
                let password = '';
                password += pickLetters(specials, 1);
                password += pickLetters(lowercase, 1);
                password += pickLetters(uppercase, 1);
                password += pickLetters(numbers, 1);
                password += pickLetters(all, 4, 4);
                password = shuffleString(password);

                $newPasswordInput.val(password);
                $newPasswordInput.attr("type", "text");

                let icon = $(this).find("i");
                if (icon) {
                    passwordVisibilityButton.find("i").removeClass("fa-eye-slash").addClass("fa-eye");
                }
            });

            passwordVisibilityButton.click(function(){
                let passwordInput = $(this).parent().find("input");
                if (!passwordInput) {
                    return;
                }

                if (passwordInput.attr("type") === "password") {
                    passwordInput.attr("type", "text");
                }
                else {
                    passwordInput.attr("type", "password");
                }

                let icon = $(this).find("i");
                if (icon) {
                    icon.toggleClass("fa-eye-slash").toggleClass("fa-eye");
                }
            });

            function pickLetters(text, min, max) {
                let n, chars = '';

                if (typeof max === 'undefined') {
                    n = min;
                } else {
                    n = min + Math.floor(Math.random() * (max - min + 1));
                }
                for (let i = 0; i < n; i++) {
                    chars += text.charAt(Math.floor(Math.random() * text.length));
                }

                return chars;
            }

            function shuffleString(text) {
                let array = text.split('');
                let tmp, current, top = array.length;

                if (top) while (--top) {
                    current = Math.floor(Math.random() * (top + 1));
                    tmp = array[current];
                    array[current] = array[top];
                    array[top] = tmp;
                }

                return array.join('');
            }
        };
        return {
            initModal: initModal
        };
    };
});
