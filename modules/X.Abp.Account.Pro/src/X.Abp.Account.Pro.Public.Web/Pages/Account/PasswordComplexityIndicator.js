(function ($) {

    var l = abp.localization.getResource("AbpAccount");
    var defaultOptions = {
        texts: ['Weak', 'Fair', 'Normal', 'Good', 'Strong'],
        colors:  ['#B0284B', '#F2A34F', '#5588A4', '#3E5CF6', '#6EBD70'],
        insideParent: false
    }
    var progress = '<div class="progress transition mx-3" style="height: 4px;border-radius: 3px 3px 0 0;margin-top: -5px;z-index: 1;position: relative;" id="progress"> <div class="progress-bar transition"></div></div>';
    var progressText = '<div class="align-items-center order-2 mt-1 progress-text" style="display: none; font-weight: 500;padding-left: 1.25rem;" id="progressText"> '+l("Strength")+' &nbsp;<span></span></div>';
    
    $.fn.passwordComplexityIndicator = function(options){
        var $passwordInput = $(this);
        options = $.extend(defaultOptions, options);
        
        if(options.insideParent){
            $passwordInput.parent().append(progress);
            $passwordInput.parent().append(progressText);
        }else{
            $passwordInput.parent().after(progressText);
            $passwordInput.parent().after(progress);
        }
        
        var $progressBar = $("#progress .progress-bar"),
            $progressText = $("#progressText");
        
        $progressBar.parent().hide();
        $passwordInput.on("keyup change",function(){
            var passwordValue = $passwordInput.val();
            
            if(!zxcvbn){
                abp.log.error("zxcvbn is not defined. Please include zxcvbn library.");
                return;
            }
            if(passwordValue){
                var strength = zxcvbn(passwordValue),
                    score = strength.score,
                    scorePercentage = (score + 1) * 20;
                $progressBar.css("width", scorePercentage + "%");
                $progressBar.css("background-color", options.colors[score]);
                $progressBar.parent().show();
                $progressText.show();
                $progressText.find("span").text(l(options.texts[score]));
                $progressText.find("span").css("color", options.colors[score]);
            }else{
                $progressBar.css("width", "0%");
                $progressBar.parent().hide();
                $progressText.hide();
            }
        });
    };
})(jQuery);
