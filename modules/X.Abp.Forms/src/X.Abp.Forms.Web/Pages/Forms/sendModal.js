(function () {
    var l = abp.localization.getResource('Forms');
        
    // hide/show copy and send buttons 
    $(document).on('click', 'ul#SendFormTabs a.nav-link', function (e) {
        if (!(e.currentTarget.id === 'v-pills-tab-email-tab')) {
            $("#sendBtn").hide();
            $("#copyBtn").show();
        } else {
            $("#sendBtn").show();
            $("#copyBtn").hide();
        }
    });

    $(document).on('click', '#copyBtn', function () {
        const activeTabs = $("ul#SendFormTabs a.active");
        if (activeTabs.length > 0 && activeTabs[0].id === "v-pills-tab-link-tab") {
            copyText("linkInput");
        } else if (activeTabs.length > 0 && activeTabs[0].id === "v-pills-tab-eHtml-tab") {
            copyText("eHtmlInput");
        }
    });

    $(document).on('submit', '#inviteEmailForm', function (e){
       e.preventDefault();
       
       var $form = $('#inviteEmailForm');
       
       if(!$form.valid()){
           return;
       }

       x.forms.forms.form.sendInviteEmail($form.serializeFormToObject().form).then(result => {
           
       });
    });
    
    $(document).on('click', '#sendBtn', function (){
        $('#inviteEmailForm').submit();
    });
    
    $(document).on('click', '#linkInput', function () {
        copyText("linkInput");
    });

    $(document).on('click', '#eHtmlInput', function () {
        copyText("eHtmlInput");
    });

    function copyText(inputId) {
        const copyText = document.getElementById(inputId);

        copyText.select();
        copyText.setSelectionRange(0, 99999); /*For mobile devices*/

        /* Copy the text inside the text field */
        document.execCommand("copy");
        abp.notify.info(l('Form:SendFormCopiedToClipboard'));
    }
})();